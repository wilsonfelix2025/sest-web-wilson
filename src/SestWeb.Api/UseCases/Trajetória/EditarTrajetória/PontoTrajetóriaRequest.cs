using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Api.UseCases.Trajetória.EditarTrajetória
{
    public class PontoTrajetóriaRequest
    {
        public double Pm { get; set; }
        public double Inclinação { get; set; }
        public double Azimute { get; set; }
    }
}
