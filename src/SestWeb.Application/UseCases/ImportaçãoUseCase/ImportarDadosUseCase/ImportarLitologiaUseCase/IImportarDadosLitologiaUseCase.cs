using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase.ImportarLitologiaUseCase
{
    public interface IImportarDadosLitologiaUseCase
    {
        Task<ImportarDadosOutput> Execute(string idPoço, LitologiaInput litologia);

    }
}
