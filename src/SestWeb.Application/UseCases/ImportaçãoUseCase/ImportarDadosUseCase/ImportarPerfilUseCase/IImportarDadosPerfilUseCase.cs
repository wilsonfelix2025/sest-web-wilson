using System.Collections.Generic;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase.ImportarPerfilUseCase
{
    public interface IImportarDadosPerfilUseCase
    {
        Task<ImportarDadosOutput> Execute(string idPoço, List<PerfilInput> listaPerfil);

    }
}
