using SestWeb.Domain.Exceptions;

namespace SestWeb.Infra.Exceptions
{
    class FalhaAutenticacaoUsuarioInvalido : InfrastructureException
    {
        internal FalhaAutenticacaoUsuarioInvalido() : base("Usuário inválido!")
        {
        }
    }
}
