using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.UsuárioUseCases.ResetarSenha
{
    public interface IResetarSenhaUseCase
    {
        Task<ResetarSenhaUseCaseOutput> Execute(string email, string novaSenha, string codigo);
    }
}
