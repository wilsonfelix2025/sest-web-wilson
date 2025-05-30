using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.AtualizarCorrelação
{
    public interface IAtualizarCorrelaçãoUseCase
    {
        Task<AtualizarCorrelaçãoOutput> Execute(string nome, string descrição, string expressão);
    }
}
