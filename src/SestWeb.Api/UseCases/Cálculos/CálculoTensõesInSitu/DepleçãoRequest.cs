namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu
{
    public class DepleçãoRequest
    {
        public string GPOROOriginalId { get; set; }
        public string GPORODepletadaId { get; set; }
        public string PoissonId { get; set; }
        public string BiotId { get; set; }
    }
}
