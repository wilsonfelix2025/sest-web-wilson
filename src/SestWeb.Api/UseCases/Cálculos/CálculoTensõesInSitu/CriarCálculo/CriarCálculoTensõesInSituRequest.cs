using System.Collections.Generic;

namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu.CriarCálculo
{
    public class CriarCálculoTensõesInSituRequest
    {
        public string IdPoço { get; set; }
        public string NomeCálculo { get; set; }
        public string PerfilTensãoVerticalId { get; set; }
        public string PerfilGPOROId { get; set; }
        public string PerfilPoissonId { get; set; }
        public string PerfilTHORminId { get; set; }
        public double? Coeficiente { get; set; }
        public List<LotRequest> ListaLot { get; set; }
        public DepleçãoRequest Depleção { get; set; }
        public string TensãoHorizontalMaiorMetodologiaCálculo { get; set; }
        public string TensãoHorizontalMenorMetodologiaCálculo { get; set; }
        public RelaçãoTensãoRequest RelaçãoTensão { get; set; }
        public BreakoutRequest Breakout { get; set; }
        public FraturasTrechosVerticaisRequest FraturasTrechosVerticais { get; set; }
    }
}