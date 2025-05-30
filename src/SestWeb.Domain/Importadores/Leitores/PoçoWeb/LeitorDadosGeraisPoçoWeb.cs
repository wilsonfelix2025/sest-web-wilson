using SestWeb.Domain.DTOs;
using SestWeb.Domain.DTOs.Importação;

namespace SestWeb.Domain.Importadores.Leitores.PoçoWeb
{
    public class LeitorDadosGeraisPoçoWeb
    {
        public DadosGeraisDTO DadosGerais { get; private set; }
        private readonly PoçoWebDto _poçoWeb;

        public LeitorDadosGeraisPoçoWeb(PoçoWebDto poçoWeb)
        {
            _poçoWeb = poçoWeb;
            DadosGerais = new DadosGeraisDTO();
            GetDadosGerais();
        }

        private void GetDadosGerais()
        {
            DadosGerais.Identificação.NomePoçoWeb = _poçoWeb.Name;
            DadosGerais.Identificação.NomePoço = _poçoWeb.Name;
            DadosGerais.Identificação.Finalidade = _poçoWeb.Revision.Content.Input.WellPurpose;
            DadosGerais.Identificação.ClassificaçãoPoço = TraduzirClassificação(_poçoWeb.Revision.Content.Input.WellClassification);
            DadosGerais.Identificação.ComplexidadePoço = _poçoWeb.Revision.Content.Input.WellComplexity?.ToString(); 
            DadosGerais.Identificação.NívelProteção = _poçoWeb.Revision.Content.Input.ProtectionLevel != null ? TraduzirNivelProtação(_poçoWeb.Revision.Content.Input.ProtectionLevel.ToString()) : string.Empty;
            DadosGerais.Identificação.TipoCompletação = TraduzirCompletação(_poçoWeb.Revision.Content.Input.CompletionType);
            DadosGerais.Identificação.VidaÚtilPrevista = _poçoWeb.Revision.Content.Input.Lifespan?.ToString();
            DadosGerais.Identificação.PoçoWebUrl = _poçoWeb.Revision.Content.SourceUrl?.AbsoluteUri;
            DadosGerais.Identificação.PoçoWebDtÚltimaAtualização = _poçoWeb.LastUpdatedAt?.DateTime.ToShortDateString();
            DadosGerais.Identificação.PoçoWebUrlRevisões = _poçoWeb.Revisions?.AbsoluteUri;
            DadosGerais.Identificação.PoçoWebRevisãoUrl = _poçoWeb.Url?.AbsoluteUri;

            //esses dados somente aparecem para arquivos asbuilt
            DadosGerais.Identificação.CriticidadePoço = _poçoWeb.Revision.Content.Input.WellCriticalness;
            DadosGerais.Identificação.IntervençãoWorkover = _poçoWeb.Revision.Content.Input.WorkoverIntervention;

            DadosGerais.Geometria.AtualizaMesaRotativaComElevação = false;
            DadosGerais.Geometria.MesaRotativa = _poçoWeb.Revision.Content.Input.Airgap?.ToString();
            DadosGerais.Geometria.OffShore.LaminaDagua = _poçoWeb.Revision.Content.Input.Lda?.ToString();
            DadosGerais.Geometria.OnShore.AlturaDeAntePoço = _poçoWeb.Revision.Content.Input.Altitude;
            DadosGerais.Geometria.OnShore.LençolFreático = "";
            DadosGerais.Geometria.OnShore.Elevação = "";

            if (_poçoWeb.Revision.Content.Input.Location != null)
            {
                DadosGerais.Geometria.Coordenadas.UtMx = _poçoWeb.Revision.Content.Input.Location.Wellhead.X?.ToString();
                DadosGerais.Geometria.Coordenadas.UtMy = _poçoWeb.Revision.Content.Input.Location.Wellhead.Y?.ToString();
            }
        }

        private string TraduzirCompletação(string completion)
        {
            var completação = string.Empty;

            switch (completion)
            {
                case "FracPack":
                    completação = "Frac-Pack";
                    break;
                case "SPR":
                    completação = "Simples Poço Revestido";
                    break;
                case "CI":
                    completação = "Completação Inteligente (2 zonas)";
                    break;
                case "CI3":
                    completação = "Completação Inteligente (3 zonas)";
                    break;
                case "GPH":
                    completação = "Gravel Pack";
                    break;
                case "StandAlone":
                    completação = "Stand Alone";
                    break;
                case "SPA":
                    completação = "Simples Poço Aberto";
                    break;
                case "TempAbandon":
                    completação = "Abandono temporário";
                    break;
                case "PACI":
                    completação = "PACI - Poço Aberto Completação Inteligente (2 zonas)";
                    break;
                case "PACI3":
                    completação = "PACI 2 + 1 - Poço Aberto Completação Inteligente (3 zonas)";
                    break;
            }

            return completação;
        }

        private string TraduzirClassificação(string classification)
        {
            var classificação = string.Empty;

            switch (classification)
            {
                case "development":
                    classificação = "Desenvolvimento";
                    break;
                case "extension":
                    classificação = "Extensão";
                    break;
                case "exploration":
                    classificação = "Pioneiro";
                    break;
            }

            return classificação;
        }

        private string TraduzirNivelProtação(string protectionLevel)
        {
            var proteção = string.Empty;
            switch (protectionLevel)
            {
                case "1":
                    proteção = "NP-1";
                    break;
                case "2":
                    proteção = "NP-2";
                    break;
                case "3":
                    proteção = "NP-3";
                    break;
                case "4":
                    proteção = "NP-4";
                    break;
            }

            return proteção;
        }

    }
}
