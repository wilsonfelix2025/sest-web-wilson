using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterCorrelaçãoPeloNome
{
    public interface IObterCorrelaçãoPeloNomeUseCase
    {
        Task<ObterCorrelaçãoPeloNomeOutput> Execute(string idPoço, string name);
    }
}
