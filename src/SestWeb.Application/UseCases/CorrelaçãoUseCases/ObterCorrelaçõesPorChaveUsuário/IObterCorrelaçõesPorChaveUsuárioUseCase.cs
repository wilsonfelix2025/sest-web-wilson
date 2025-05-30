using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterCorrelaçõesPorChaveUsuário
{
    public interface IObterCorrelaçõesPorChaveUsuárioUseCase
    {
        Task<ObterCorrelaçõesPorChaveUsuárioOutput> Execute(string idPoço, string userKey);
    }
}
