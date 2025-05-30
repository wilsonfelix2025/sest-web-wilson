using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Importadores.Shallow.Utils;
using SestWeb.Domain.Importadores.Utils;

namespace SestWeb.Domain.Importadores.Deep.Las
{
    public class LeitorDeepLas : LeitorDeepArquivo
    {
        public string[] Linhas { get; set; }

        private LeitorSeçõesLas _leitorSeções;

        private LeitorCurvasLas _leitorCurvas;

        private LeitorValoresLas _leitorValores;

        private LeitorPoçoLas _leitorPoço;

        public LeitorDeepLas(string caminhoDoArquivo, List<DadosSelecionadosEnum> dadosSelecionados,
            List<PerfilParaImportarDTO> perfisSelecionados, List<LitologiaParaImportarDTO> litologiasSelecionadas, 
            Dictionary<string, object> dadosExtras)
            : base(caminhoDoArquivo, dadosSelecionados, perfisSelecionados, litologiasSelecionadas, dadosExtras)
        {
            
        }

        public override bool TryGetDadosGerais(out DadosGeraisDTO dadosGerais)
        {
            dadosGerais = null;
            return true;
        }               

        public override bool TryGetTrajetória(out TrajetóriaDTO trajetória)
        {
            trajetória = null;

            if (_leitorCurvas.TemTrajetória)
            {
                // Obtem as coleções de dados referentes à trajetória
                var listaPm = _leitorValores.Valores[_leitorCurvas.ÍndiceColunaPm].ToArray();
                var listaInclinação = _leitorValores.Valores[_leitorCurvas.ÍndiceColunaInclinação].ToArray();
                var listaAzimute = _leitorValores.Valores[_leitorCurvas.ÍndiceColunaAzimute].ToArray();

                trajetória = new TrajetóriaDTO
                {
                    Pontos = new List<PontoTrajetóriaDTO>()
                };

                for (var índiceLinha = 0; índiceLinha < listaPm.Count(); índiceLinha++)
                {
                    trajetória.Pontos.Add(new PontoTrajetóriaDTO
                    {
                        Pm = listaPm[índiceLinha],
                        Inclinação = listaInclinação[índiceLinha],
                        Azimute = listaAzimute[índiceLinha]
                    });
                }
            }

            return true;
        }

        public override bool TryGetLitologias(out List<LitologiaDTO> litologias)
        {            
            litologias = new List<LitologiaDTO>();

            if (LitologiasSelecionadas.Count() == 0)
            {
                return true;
            }

            // Obtem os valores de PM
            var índiceColunaPm = _leitorCurvas.ÍndiceColunaPm;
            var listaPm = _leitorValores.Valores[índiceColunaPm];

            // Obtém os mnemônicos de todas as curvas lidas
            var mnemônicosLitologiasLidas = _leitorCurvas.Litologias.ToArray();
            // Obtém os mnemônicos de todas as litologias que o usuário quer importar
            var mnemônicosLitologiasSelecionadas = LitologiasSelecionadas.Select(lito => lito.Nome).ToList();

            // Itera sobre as litologias lidas
            for (var índiceLitologia = 0; índiceLitologia < mnemônicosLitologiasLidas.Length; índiceLitologia++)
            {
                var mnemônicoLitologiaAtual = mnemônicosLitologiasLidas[índiceLitologia];
                var índiceColunaLitologiaAtual = _leitorCurvas.ObterÍndiceDoMnemônico(mnemônicoLitologiaAtual);

                // Se o usuário selecionou a litologia atual
                if (mnemônicosLitologiasSelecionadas.Contains(mnemônicoLitologiaAtual))
                {
                    List<PontoLitologiaDTO> pontosLitologia = new List<PontoLitologiaDTO>();

                    // Itera sobre os pontos da litologia selecionada
                    for (var índicePonto = 0; índicePonto < listaPm.Count; índicePonto++)
                    {
                        var nomeRocha = string.Empty;

                        try
                        {
                            var tipoRocha = TipoRocha.FromNumero(int.Parse(_leitorValores.Valores[índiceColunaLitologiaAtual][índicePonto]));
                            if (tipoRocha == null)
                                nomeRocha = TipoRocha.OUT.Nome;
                            else
                                nomeRocha = tipoRocha.Nome;
                        }
                        catch (ArgumentException)
                        {
                            nomeRocha = TipoRocha.OUT.Nome;
                        }

                        // Adiciona cada ponto à lista de pontos
                        pontosLitologia.Add(new PontoLitologiaDTO
                        {
                            Pm = listaPm[índicePonto],
                            TipoRocha = nomeRocha
                        });
                    }

                    // Adiciona a nova litologia à lista de litologias
                    litologias.Add(new LitologiaDTO
                    {
                        Classificação = LitologiasSelecionadas[índiceLitologia].Tipo,
                        Nome = LitologiasSelecionadas[índiceLitologia].Nome,
                        Pontos = pontosLitologia
                    });
                }
            }

            return true;
        }

        public override bool TryGetPerfis(out List<PerfilDTO> perfis)
        {
            perfis = new List<PerfilDTO>();
            // Obtem a coluna de profundidade medida para registrar os pontos de perfil no PM correspondente
            var listaPm = _leitorValores.Valores[_leitorCurvas.ÍndiceColunaPm];

            foreach (var perfil in PerfisSelecionados)
            {
                var nomePerfil = perfil.Nome;
                // Encontra o índice do mnemônico na lista de perfis
                var índicePerfil = _leitorCurvas.ObterÍndiceDoMnemônico(nomePerfil);
                if (índicePerfil < 0)
                {
                    throw new ArgumentException($"O seguinte perfil não consta na lista de perfis lidos do arquivo: {perfil}");
                }

                // Busca os valores do perfil com base no índice do mnemônico
                var valoresPerfil = _leitorValores.Valores[índicePerfil];
                var listaValoresPerfil = new List<PontoDTO>();

                for (var i = 0; i < valoresPerfil.Count(); i++)
                {
                    // Para cada ponto, monta um objeto PontoDTO e adiciona à lista de pontos
                    if (!ÉValorInválido(valoresPerfil[i]))
                    {
                        var ponto = new PontoDTO
                        {
                            Pm = listaPm[i],
                            Origem = OrigemPonto.Importado,
                            Pv = null,
                            Valor = valoresPerfil[i]
                        };
                        listaValoresPerfil.Add(ponto);
                    }
                }

                // No final, adiciona o perfil à lista de perfis do PoçoDTO
                perfis.Add(new PerfilDTO
                {
                    Nome = perfil.Nome,
                    Mnemonico = perfil.Tipo,
                    Grupo = "",
                    Descrição = "",
                    Unidade = perfil.Unidade,
                    PontosDTO = listaValoresPerfil
                });
            }

            return true;
        }

        public override void FazerAçõesIniciais()
        {
            // Carrega apenas as linhas com conteúdo no arquivo
            Linhas = CarregarArquivo(CaminhoOuDadoDoArquivo).Where(linha => StringUtils.HasContent(linha)).ToArray();
                        
            _leitorSeções = new LeitorSeçõesLas();
            _leitorSeções.SepararEmSeções(Linhas);

            _leitorCurvas = new LeitorCurvasLas(_leitorSeções.ObterSeção("~C"));
            _leitorValores = new LeitorValoresLas(_leitorSeções.ObterSeção("~A"), _leitorCurvas.Informações.Count);
            _leitorPoço = new LeitorPoçoLas(_leitorSeções.ObterSeção("~W"));
        }

        public override bool TryGetEstratigrafia(out EstratigrafiaDTO estratigrafia)
        {
            estratigrafia = null;
            return true;
        }

        public override bool TryGetObjetivos(out List<ObjetivoDTO> objetivos)
        {
            objetivos = null;
            return true;
        }        

        public override bool TryGetSapatas(out List<SapataDTO> sapatas)
        {
            sapatas = new List<SapataDTO>();
            return true;
        }

        public override bool TryGetRegistros(out List<RegistroDTO> registrosDePerfuração)
        {
            registrosDePerfuração = null;
            return true;
        }

        public override bool TryGetEventos(out List<RegistroDTO> eventos)
        {
            eventos = null;
            return true;
        }

        /// <summary>
        /// Método alias para simplificar e esclarecer a checagem de valores inválidos no método principal.
        /// </summary>
        /// <param name="valor">O valor a ser testado.</param>
        /// <returns>True se o valor é um double válido, false caso contrário.</returns>
        private bool ÉValorInválido(string valor)
        {
            var conseguiuParsear = double.TryParse(valor, NumberStyles.Float, CultureInfo.InvariantCulture, out double valorDouble);
            return !conseguiuParsear || ReferênciaParaImportação.ÉSemValor(valorDouble);
        }
    }
}
