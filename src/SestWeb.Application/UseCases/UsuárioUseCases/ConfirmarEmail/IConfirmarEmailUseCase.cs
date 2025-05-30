using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.UsuárioUseCases.ConfirmarEmail
{
    public interface IConfirmarEmailUseCase
    {
        Task<ConfirmarEmailOutput> Execute(string idUsuario, string codigo);
    }
}
