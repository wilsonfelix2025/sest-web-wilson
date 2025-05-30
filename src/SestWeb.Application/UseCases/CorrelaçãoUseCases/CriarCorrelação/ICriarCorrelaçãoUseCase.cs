using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.CriarCorrelação
{
    public interface ICriarCorrelaçãoUseCase
    {
        Task<CriarCorrelaçãoOutput> Execute(string idPoço, string nome, string nomeAutor, string chaveAutor, string descrição, string expressão);
    }
}
