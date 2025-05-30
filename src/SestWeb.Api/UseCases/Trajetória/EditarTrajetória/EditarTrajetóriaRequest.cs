using SestWeb.Api.UseCases.Trajetória.EditarTrajetória;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Api.UseCases.Perfil.EditarTrajetória
{
    public class EditarTrajetóriaRequest
    {
        public PontoTrajetóriaRequest[] Pontos { get; set; }
    }
}