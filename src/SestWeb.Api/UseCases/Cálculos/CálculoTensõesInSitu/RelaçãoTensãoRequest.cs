namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu
{
    public class RelaçãoTensãoRequest
    {
        public string PerfilRelaçãoTensãoId { get; set; }
        public string TipoRelação { get; set; }
        public double? AZTHMenor { get; set; }
        public string PerfilTVERTId { get; set; }
        public string PerfilGPOROId { get; set; }

    }
}
