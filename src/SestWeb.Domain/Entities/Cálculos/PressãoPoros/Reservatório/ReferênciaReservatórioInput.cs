namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.Reservatório
{
    public class ReferênciaReservatório
    {
        public string Poco { get; set; }
        public double Cota { get; set; }
        public double Pp { get; set; }
        public GradienteReservatório Gradiente { get; set; }
        public ContatosReservatório Contatos { get; set; }
    }
}
