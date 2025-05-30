using System.Collections.Generic;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase.ImportarTrajetóriaUseCase
{
    public interface IImportarDadosTrajetóriaUseCase
    {
        Task<ImportarDadosOutput> Execute(string idPoço, TrajetóriaInput trajetória);

    }
}
