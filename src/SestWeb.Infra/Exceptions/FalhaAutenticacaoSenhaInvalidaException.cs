using SestWeb.Domain.Exceptions;

namespace SestWeb.Infra.Exceptions
{
    class FalhaAutenticacaoSenhaInvalidaException : InfrastructureException
    {
        internal FalhaAutenticacaoSenhaInvalidaException() : base("Senha incorreta!")
        {
        }
    }
}
