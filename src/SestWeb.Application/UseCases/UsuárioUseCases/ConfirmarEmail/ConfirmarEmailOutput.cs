using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.UsuárioUseCases.ConfirmarEmail
{
    public class ConfirmarEmailOutput : UseCaseOutput<ConfirmarEmailStatus>
    {
        public static ConfirmarEmailOutput EmailConfirmado()
        {
            return new ConfirmarEmailOutput()
            {
                Status = ConfirmarEmailStatus.EmailConfirmado,
                Mensagem = "E-mail confirmado com sucesso!"
            };
        }

        public static ConfirmarEmailOutput EmailNãoConfirmado()
        {
            return new ConfirmarEmailOutput()
            {
                Status = ConfirmarEmailStatus.EmailNaoConfirmado,
                Mensagem = "E-mail não confirmado!"
            };
        }
    }
}
