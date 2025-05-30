using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.CriarCorrelaçãoPoço
{
    public interface ICriarCorrelaçãoPoçoUseCase
    {
        Task<CriarCorrelaçãoPoçoOutput> Execute(string idPoço, string nome, string nomeAutor, string chaveAutor,
            string descrição, string expressão);
    }
}
