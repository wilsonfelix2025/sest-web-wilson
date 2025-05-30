using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.UsuárioUseCases.EsqueceuSenha
{
    public interface IEsqueceuSenhaUseCase
    {
        Task<EsqueceuSenhaUseCaseOutput> Execute(string email);
    }
}
