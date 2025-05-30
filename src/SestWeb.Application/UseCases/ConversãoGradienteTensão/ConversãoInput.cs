using SestWeb.Domain.Enums;

namespace SestWeb.Application.UseCases.ConversãoGradienteTensão
{
    public class ConversãoInput
    {
        public string IdPerfil { get; set; }
        public TipoConversãoEnum TipoConversão { get; set; }
    }
}
