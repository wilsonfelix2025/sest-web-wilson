namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.CriarCálculo
{
    public class TrechoPropMecRequest : RegiãoRequest
    {
        public double PmTopo { get; set; }
        public double PmBase { get; set; }
    }
}