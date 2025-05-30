using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.RemoverCorrelação
{
    public interface IRemoverCorrelaçãoUseCase
    {
        Task<RemoverCorrelaçãoOutput> Execute(string nome);
    }
}
