using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarArquivoUseCase.LerArquivoPoçoWebUseCase
{
    public interface ILerArquivoPoçoWebUseCase
    {
        Task<LerArquivoPoçoWebOutput> Execute(string urlArquivo, string token);
    }
}
