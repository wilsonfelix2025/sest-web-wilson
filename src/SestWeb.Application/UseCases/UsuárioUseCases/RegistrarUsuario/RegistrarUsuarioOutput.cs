using SestWeb.Application.Helpers;
using SestWeb.Domain.Usuario;

namespace SestWeb.Application.UseCases.UsuárioUseCases.RegistrarUsuario
{
    public class RegistrarUsuarioOutput : UseCaseOutput<RegistrarUsuarioStatus>
    {
        public string CodigoConfirmaçãoEmail { get; set; } = null;
        public string IdUsuario { get; set; } = null;
        public string Email { get; private set; }

        private RegistrarUsuarioOutput(RegistrarUsuarioStatus status)
        {
            Status = status;
        }

        public static RegistrarUsuarioOutput UsuarioRegistradoComSucesso(Usuario usuario)
        {
            return new RegistrarUsuarioOutput(RegistrarUsuarioStatus.UsuarioCriado)
            {
                Email = usuario.Email,
                IdUsuario = usuario.Id,
                CodigoConfirmaçãoEmail = usuario.CódigoConfirmaçãoEmail,
                Mensagem = "Usuário criado com sucesso."
            };
        }

        public static RegistrarUsuarioOutput UsuarioNãoRegistrado(string mensagem)
        {
            return new RegistrarUsuarioOutput(RegistrarUsuarioStatus.UsuarioNaoCriado)
            {
                Mensagem = mensagem,
                Email = null
            };
        }
    }
}
