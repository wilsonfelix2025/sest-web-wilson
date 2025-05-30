using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Importadores.Deep;

namespace SestWeb.Domain.Importadores.Leitores.SIGEO
{
    public class LeitorDeepSIGEO : LeitorDeepArquivo
    {
        public string[] Linhas { get; set; }
        private double baseSedimentos = 0.0;


        public LeitorDeepSIGEO(string caminhoDoArquivo, List<DadosSelecionadosEnum> dadosSelecionados,
            List<PerfilParaImportarDTO> perfisSelecionados,List<LitologiaParaImportarDTO> litologiasSelecionadas,
            Dictionary<string, object> dadosExtras)
            :base(caminhoDoArquivo, dadosSelecionados, perfisSelecionados, litologiasSelecionadas, dadosExtras)
        {
        }

        public override void FazerAçõesIniciais()
        {
            Linhas = CarregarArquivo(CaminhoOuDadoDoArquivo).Where(linha => !string.IsNullOrEmpty(linha) && !string.IsNullOrWhiteSpace(linha)).ToArray();
        }

        public override bool TryGetDadosGerais(out DadosGeraisDTO dadosGerais)
        {
            dadosGerais = null;

            if (DadosSelecionados.Contains(DadosSelecionadosEnum.DadosGerais))
            {
                var leitor = new LeitorDadosGeraisSIGEO(Linhas);
                dadosGerais = leitor.DadosGerais;
                if (dadosGerais.Geometria.OffShore != null)
                {
                    baseSedimentos = double.Parse(dadosGerais.Geometria.MesaRotativa, NumberStyles.Any, CultureInfo.InvariantCulture) + double.Parse(dadosGerais.Geometria.OffShore.LaminaDagua, NumberStyles.Any, CultureInfo.InvariantCulture);
                }
                else if (dadosGerais.Geometria.OnShore != null)
                {
                    baseSedimentos = double.Parse(dadosGerais.Geometria.MesaRotativa, NumberStyles.Any, CultureInfo.InvariantCulture) + double.Parse(dadosGerais.Geometria.OnShore.AlturaDeAntePoço, NumberStyles.Any, CultureInfo.InvariantCulture);
                }
            }

            return true;
        }

        public override bool TryGetEstratigrafia(out EstratigrafiaDTO estratigrafia)
        {
            estratigrafia = null;

            if (DadosSelecionados.Contains(DadosSelecionadosEnum.Estratigrafia))
            {
                var leitor = new LeitorEstratigrafiaSIGEO(Linhas, false);
                estratigrafia = leitor.Estratigrafia;
            }

            return true;
        }

        public override bool TryGetLitologias(out List<LitologiaDTO> litologias)
        {
            litologias = new List<LitologiaDTO>();
            var leitor = new LeitorLitologiaInterpretadaSIGEO(Linhas, LitologiasSelecionadas, false);
            litologias.Add(leitor.Litologia);
            return true;
        }

        public override bool TryGetObjetivos(out List<ObjetivoDTO> objetivos)
        {
            objetivos = new List<ObjetivoDTO>();

            if (DadosSelecionados.Contains(DadosSelecionadosEnum.Objetivos))
            {
                var leitor = new LeitorObjetivosSIGEO(Linhas, false);
                objetivos = leitor.Objetivos;
            }

            return true;
        }

        public override bool TryGetPerfis(out List<PerfilDTO> perfis)
        {
            perfis = new List<PerfilDTO>();
            var leitor = new LeitorPerfisSIGEO(Linhas, false, PerfisSelecionados, baseSedimentos);
            perfis.Add(leitor.Perfil);
            return true;
        }

        public override bool TryGetSapatas(out List<SapataDTO> sapatas)
        {
            sapatas = new List<SapataDTO>();

            if (DadosSelecionados.Contains(DadosSelecionadosEnum.Sapatas))
            {
                var leitor = new LeitorSapataSIGEO(Linhas, false);
                sapatas = leitor.Sapatas;
            }

            return true;
        }

        public override bool TryGetTrajetória(out TrajetóriaDTO trajetória)
        {
            trajetória = new TrajetóriaDTO();

            if (DadosSelecionados.Contains(DadosSelecionadosEnum.Trajetória))
            {
                var leitor = new LeitorTrajetoriaSIGEO(Linhas);
                trajetória = leitor.Trajetória;
            }

            return true;
        }

        public override bool TryGetRegistros(out List<RegistroDTO> registrosDePerfuração)
        {
            registrosDePerfuração = new List<RegistroDTO>();

            if (DadosSelecionados.Contains(DadosSelecionadosEnum.Registros))
            {
                var leitor = new LeitorRegistrosDePressãoSIGEO(Linhas, false);
                registrosDePerfuração.Add(leitor.RegistroDePerfuração);
            }

            return true;
        }


        public override bool TryGetEventos(out List<RegistroDTO> eventos)
        {
            eventos = new List<RegistroDTO>();
            
            return true;
        }
        protected string[] CarregarArquivo(string caminhoDoArquivo)
        {
            try
            {
                return File.ReadAllLines(caminhoDoArquivo);
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException($"Arquivo não encontrado: {caminhoDoArquivo}");
            }
            catch (IOException e)
            {
                throw new IOException(e.Message);
            }
        }
    }
}
