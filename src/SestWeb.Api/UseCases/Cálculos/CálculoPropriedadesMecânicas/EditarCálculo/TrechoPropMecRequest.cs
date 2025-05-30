namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.EditarCálculo
{
    public class TrechoPropMecRequest : RegiãoRequest
    {
        public double PmTopo { get; set; }
        public double PmBase { get; set; }
    }
}