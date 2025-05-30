using SestWeb.Domain.DTOs;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Importadores.Deep;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using SestWeb.Domain.Entities.Poço;

namespace SestWeb.Domain.Importadores.Leitores.PoçoWeb
{
    public class LeitorDeepPoçoWeb : LeitorDeepArquivo
    {
        public PoçoWebDto PoçoWeb;
        private Poço PoçoAtual;
        private double baseSedimentos = 0.0;

        public LeitorDeepPoçoWeb(string dadoDoArquivo, List<DadosSelecionadosEnum> dadosSelecionados
            , List<PerfilParaImportarDTO> perfisSelecionados, List<LitologiaParaImportarDTO> litologiasSelecionadas
            ,Dictionary<string, object> dadosExtras)
            : base(dadoDoArquivo, dadosSelecionados, perfisSelecionados, litologiasSelecionadas, dadosExtras)
        {
            
        }

        public override void FazerAçõesIniciais()
        {
            PoçoWeb = JsonConvert.DeserializeObject<PoçoWebDto>(DadosExtras["poçoWebJson"].ToString());
            PoçoAtual = DadosExtras["poço"] as Poço;
        }

        public override bool TryGetDadosGerais(out DadosGeraisDTO dadosGerais)
        {
            dadosGerais = null;

            if (DadosSelecionados.Contains(DadosSelecionadosEnum.DadosGerais))
            {
                var leitor = new LeitorDadosGeraisPoçoWeb(PoçoWeb);
                dadosGerais = leitor.DadosGerais;
                if (dadosGerais.Geometria.OffShore != null)
                {
                    baseSedimentos= double.Parse(dadosGerais.Geometria.MesaRotativa, NumberStyles.Any, CultureInfo.InvariantCulture) + double.Parse(dadosGerais.Geometria.OffShore.LaminaDagua, NumberStyles.Any,CultureInfo.InvariantCulture);
                } else if (dadosGerais.Geometria.OnShore != null)
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
                var leitor = new LeitorEstratigrafiaPoçoWeb(PoçoWeb, false, PoçoAtual, baseSedimentos);
                estratigrafia = leitor.Estratigrafia;
            }

            return true;
        }

        public override bool TryGetLitologias(out List<LitologiaDTO> litologias)
        {
            litologias = new List<LitologiaDTO>();
            var leitor = new LeitorLitologiaPoçoWeb(PoçoWeb, false, PoçoAtual, baseSedimentos);
            litologias.Add(leitor.Litologia);
            return true;
        }

        public override bool TryGetObjetivos(out List<ObjetivoDTO> objetivos)
        {
            objetivos = new List<ObjetivoDTO>();

            if (DadosSelecionados.Contains(DadosSelecionadosEnum.Objetivos))
            {
                var leitor = new LeitorObjetivosPoçoWeb(PoçoWeb, false, PoçoAtual);
                objetivos = leitor.Objetivos;
            }

            return true;
        }

        public override bool TryGetPerfis(out List<PerfilDTO> perfis)
        {
            perfis = new List<PerfilDTO>();
            var leitor = new LeitorPerfisPoçoWeb(PoçoWeb, false, PoçoAtual, PerfisSelecionados);
            perfis = leitor.ListaPerfil;
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

        public override bool TryGetSapatas(out List<SapataDTO> sapatas)
        {
            sapatas = new List<SapataDTO>();

            if (DadosSelecionados.Contains(DadosSelecionadosEnum.Sapatas))
            {
                var leitor = new LeitorSapatasPoçoWeb(PoçoWeb, false);
                sapatas = leitor.Sapatas;
            }

            return true;
        }

        public override bool TryGetTrajetória(out TrajetóriaDTO trajetória)
        {
            trajetória = new TrajetóriaDTO();

            if (DadosSelecionados.Contains(DadosSelecionadosEnum.Trajetória))
            {
                var leitor = new LeitorTrajetoriaPoçoWeb(PoçoWeb, false);
                trajetória = leitor.Trajetória;
            }

            return true;
        }
    }
}
