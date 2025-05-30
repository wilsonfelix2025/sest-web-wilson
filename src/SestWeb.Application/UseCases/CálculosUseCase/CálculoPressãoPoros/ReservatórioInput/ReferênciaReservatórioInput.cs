namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.ReservatórioInput
{
    public class ReferênciaReservatórioInput
    {
        public string Poco { get; set; }
        public double Cota { get; set; }
        public double Pp { get; set; }
        public GradienteReservatórioInput Gradiente { get; set; }
        public ContatosReservatórioInput Contatos { get; set; }
    }
}
