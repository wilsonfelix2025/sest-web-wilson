using SestWeb.Application.Helpers;
using SestWeb.Domain.Usuario;

namespace SestWeb.Application.UseCases.UsuárioUseCases.AutenticarUsuario
{
    public class AutenticarUsuarioOutput : UseCaseOutput<AutenticarUsuarioStatus>
    {
        public Usuario Usuario { get; private set; }

        public static AutenticarUsuarioOutput UsuarioAutenticado(Usuario usuario)
        {
            return new AutenticarUsuarioOutput()
            {
                Usuario = usuario,
                Status = AutenticarUsuarioStatus.UsuarioAutenticado
            };
        }

        public static AutenticarUsuarioOutput EmailNãoConfirmado()
        {
            return new AutenticarUsuarioOutput()
            {
                Mensagem = "Email não confirmado! Favor conferir sua caixa de e-mail para acessar o link de confirmação.",
                Status = AutenticarUsuarioStatus.UsuarioNaoAutenticado
            };
        }

        public static AutenticarUsuarioOutput UsuarioNaoAutenticado(string mensagem)
        {
            return new AutenticarUsuarioOutput()
            {
                Mensagem = mensagem,
                Status = AutenticarUsuarioStatus.UsuarioNaoAutenticado
            };
        }

    }
}
