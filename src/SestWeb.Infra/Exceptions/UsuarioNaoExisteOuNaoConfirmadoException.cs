using SestWeb.Domain.Exceptions;

namespace SestWeb.Infra.Exceptions
{
    class UsuarioNaoExisteOuNaoConfirmadoException : InfrastructureException
    {
        internal UsuarioNaoExisteOuNaoConfirmadoException() : base("Usuario não existe ou não possui e-mail confirmado!")
        {
        }
    }
}
