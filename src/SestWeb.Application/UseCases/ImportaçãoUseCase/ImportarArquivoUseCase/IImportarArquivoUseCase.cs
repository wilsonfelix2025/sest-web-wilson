using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Enums;

namespace SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarArquivoUseCase
{
    public interface IImportarArquivoUseCase
    {
        Task<ImportarArquivoOutput> Execute(List<DadosSelecionadosEnum> dadosSelecionados, string caminhoDoarquivo, List<PerfilModel> perfisSelecionados,  List<LitologiaModel> litologiasSelecionadas, string idPoço, string correçãoMesaRotativa, Dictionary<string, object> dadosExtras);

    }
}
