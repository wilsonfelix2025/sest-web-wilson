using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterAutoresCorrelações
{
    public interface IObterAutoresCorrelaçõesUseCase
    {
        Task<ObterAutoresCorrelaçõesOutput> Execute(string idPoço);
    }
}
