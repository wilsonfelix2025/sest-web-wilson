using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterTodasCorrelações
{
    public interface IObterTodasCorrelaçõesUseCase
    {
        Task<ObterTodasCorrelaçõesOutput> Execute(string idPoço);
    }
}
