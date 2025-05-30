using SestWeb.Domain.DTOs;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Importadores.Deep;
using SestWeb.Domain.Importadores.SestTR1.Utils;
using System.Collections.Generic;
using System.Linq;

namespace SestWeb.Domain.Importadores.SestTR1.Deep
{
    public class LeitorDeepSestTR1: LeitorDeepArquivo
    {
        public LeitorDeepTrajetóriaTR1 LeitorTrajetória;
        public LeitorDeepDadosGeraisTR1 LeitorDadosGerais { get; private set; }
        public LeitorDeepRegistrosTR1 LeitorRegistros { get; private set; }

        public LeitorDeepLitologiaTR1 LeitorLitologias = new LeitorDeepLitologiaTR1();
        public LeitorDeepPerfilTR1 LeitorPerfis = new LeitorDeepPerfilTR1();

        private string _trajetóriaSelecionada = string.Empty;
        private List<string> _nomesLitologiasSelecionadas = new List<string>();
        private List<string> _nomesPerfisSelecionados = new List<string>();

        public LeitorDeepSestTR1(string caminhoDoArquivo, List<DadosSelecionadosEnum> dadosSelecionados,
            List<PerfilParaImportarDTO> perfisSelecionados, List<LitologiaParaImportarDTO> litologiasSelecionadas, Dictionary<string, object> dadosExtras)
            : base(caminhoDoArquivo, dadosSelecionados, perfisSelecionados, litologiasSelecionadas, dadosExtras)
        {

        }

        public override bool TryGetTrajetória(out TrajetóriaDTO trajetória)
        {
            trajetória = LeitorTrajetória != null && LeitorTrajetória.Trajetória != null ? LeitorTrajetória.Trajetória : new TrajetóriaDTO();
            return true;
        }

        public override bool TryGetLitologias(out List<LitologiaDTO> litologias)
        {
            var nomesLitologiasSelecionadas = LitologiasSelecionadas.Select(lito => lito.Nome);
            litologias = LeitorLitologias.Litologias.Where(lito => nomesLitologiasSelecionadas.Contains(lito.Nome)).ToList();
            return true;
        }

        public override bool TryGetPerfis(out List<PerfilDTO> perfis)
        {
            var nomesPerfisSelecionados = PerfisSelecionados.Select(perfil => perfil.Nome);
            perfis = LeitorPerfis.Perfis.Where(perfil => nomesPerfisSelecionados.Contains(perfil.Nome)).ToList();
            return true;
        }

        public override bool TryGetDadosGerais(out DadosGeraisDTO dadosGerais) {
            dadosGerais = LeitorDadosGerais != null && LeitorDadosGerais.DadosGerais != null ? LeitorDadosGerais.DadosGerais : new DadosGeraisDTO();
            return true;
        }

        public override void FazerAçõesIniciais()
        {
            var streamReader = LeitorHelperTR1.AbrirArquivoSestTR1(CaminhoOuDadoDoArquivo);
            var buffer = new List<string>();

            _trajetóriaSelecionada = DadosExtras.ContainsKey("TrajetóriaSelecionada") ? DadosExtras["TrajetóriaSelecionada"].ToString() : string.Empty;
            _nomesLitologiasSelecionadas = LitologiasSelecionadas.Select(lito => lito.Nome).ToList();
            _nomesPerfisSelecionados = PerfisSelecionados.Select(lito => lito.Nome).ToList();

            ModoLeituraArquivoTR1 modoLeitura = ModoLeituraArquivoTR1.Nenhum;

            while (!streamReader.EndOfStream)
            {
                var linha = streamReader.ReadLine();
                if (LeitorHelperTR1.ÉDataset(linha))
                {
                    if (LeitorHelperTR1.ÉLinhaDeTrajetória(linha))
                    {
                        if (_trajetóriaSelecionada != string.Empty && LeitorHelperTR1.ObterAtributo(linha, "Name") == _trajetóriaSelecionada)
                        {
                            modoLeitura = ModoLeituraArquivoTR1.Trajetória;
                            buffer.Add(linha);
                        }
                    }
                    else if (LeitorHelperTR1.ÉLinhaDeLitologia(linha))
                    {
                        if (_nomesLitologiasSelecionadas.Contains(LeitorHelperTR1.ObterAtributo(linha, "Name")))
                        {
                            modoLeitura = ModoLeituraArquivoTR1.Litologias;
                            buffer.Add(linha);
                        }
                    }
                    else
                    {
                        if (_nomesPerfisSelecionados.Contains(LeitorHelperTR1.ObterAtributo(linha, "Name")))
                        {
                            modoLeitura = ModoLeituraArquivoTR1.Perfis;
                            buffer.Add(linha);
                        }
                    }
                }
                else if (LeitorHelperTR1.ÉDadosGerais(linha) && DadosSelecionados.Contains(DadosSelecionadosEnum.DadosGerais))
                {
                    modoLeitura = ModoLeituraArquivoTR1.DadosGerais;
                    buffer.Add(linha);
                }
                else if (LeitorHelperTR1.ÉFimDeDadosGerais(linha) && DadosSelecionados.Contains(DadosSelecionadosEnum.DadosGerais))
                {
                    buffer.Add(linha);
                    LeitorDadosGerais = new LeitorDeepDadosGeraisTR1(buffer);
                    buffer.Clear();
                    modoLeitura = ModoLeituraArquivoTR1.Nenhum;
                }
                else if (LeitorHelperTR1.ÉFimDeRegistros(linha) && (DadosSelecionados.Contains(DadosSelecionadosEnum.Registros) || DadosSelecionados.Contains(DadosSelecionadosEnum.Eventos)))
                {
                    buffer.Add(linha);
                    LeitorRegistros = new LeitorDeepRegistrosTR1(buffer);
                    buffer.Clear();
                    modoLeitura = ModoLeituraArquivoTR1.Nenhum;
                }
                else if (modoLeitura == ModoLeituraArquivoTR1.Registros ||  (LeitorHelperTR1.ÉRegistros(linha) && (DadosSelecionados.Contains(DadosSelecionadosEnum.Registros) || DadosSelecionados.Contains(DadosSelecionadosEnum.Eventos))))
                {
                    modoLeitura = ModoLeituraArquivoTR1.Registros;
                    buffer.Add(linha);
                }               
                else if (LeitorHelperTR1.ÉFimDeDataset(linha))
                {
                    switch (modoLeitura)
                    {
                        case ModoLeituraArquivoTR1.Trajetória: LeitorTrajetória = new LeitorDeepTrajetóriaTR1(buffer); break;
                        case ModoLeituraArquivoTR1.Litologias: LeitorLitologias.AdicionarLitologia(buffer); break;
                        case ModoLeituraArquivoTR1.Perfis: LeitorPerfis.AdicionarPerfil(buffer); break;
                        default: continue;
                    }

                    buffer.Clear();
                    modoLeitura = ModoLeituraArquivoTR1.Nenhum;
                }
                else if (modoLeitura != ModoLeituraArquivoTR1.Nenhum)
                {
                    buffer.Add(linha);
                }
            }
        }

        public override bool TryGetSapatas(out List<SapataDTO> sapatas) {
            sapatas = null;
            return true;
        }

        public override bool TryGetObjetivos(out List<ObjetivoDTO> objetivos) {
            objetivos = null;
            return true;
        }

        public override bool TryGetEstratigrafia(out EstratigrafiaDTO estratigrafia) {
            estratigrafia = null;
            return true;
        }

        public override bool TryGetRegistros(out List<RegistroDTO> registrosDePerfuração) {
            registrosDePerfuração = LeitorRegistros != null && LeitorRegistros.Registros != null ? LeitorRegistros.Registros : null;
            return true;
        }

        public override bool TryGetEventos(out List<RegistroDTO> registrosDePerfuração)
        {
            registrosDePerfuração = null;
            return true;
        }
    }
}
