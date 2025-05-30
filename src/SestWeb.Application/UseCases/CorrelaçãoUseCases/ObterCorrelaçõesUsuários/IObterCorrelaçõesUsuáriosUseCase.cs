using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterCorrelaçõesUsuários
{
    public interface IObterCorrelaçõesUsuáriosUseCase
    {
        Task<ObterCorrelaçõesUsuáriosOutput> Execute(string idPoço);
    }
}
