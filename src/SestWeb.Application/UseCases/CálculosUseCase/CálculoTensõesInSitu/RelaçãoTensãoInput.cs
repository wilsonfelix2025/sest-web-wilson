namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu
{
    public class RelaçãoTensãoInput
    {
        public string PerfilRelaçãoTensãoId { get; set; }
        public string TipoRelação { get; set; }
        public double? AZTHMenor { get; set; }
        public string PerfilTVERTId { get; set; }
        public string PerfilGPOROId { get; set; }
    }
}