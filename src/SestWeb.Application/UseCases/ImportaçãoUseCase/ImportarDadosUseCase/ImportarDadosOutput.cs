using System.Collections.Generic;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase
{
    public class ImportarDadosOutput : UseCaseOutput<ImportarDadosStatus>
    {
        public List<PerfilBase> PerfisImportados { get; set; }

        public ImportarDadosOutput()
        {
            PerfisImportados = new List<PerfilBase>();
        }

        public static ImportarDadosOutput ImportadoComSucesso(List<PerfilBase> perfisImportados= null )
        {
            return new ImportarDadosOutput
            {
                PerfisImportados = perfisImportados,
                Status = ImportarDadosStatus.ImportadoComSucesso,
                Mensagem = "Importação dos dados feita com sucesso."
            };
        }

        public static ImportarDadosOutput ImportaçãoComFalhasDeValidação(string msg = "")
        {
            return new ImportarDadosOutput
            {
                Status = ImportarDadosStatus.ImportaçãoComFalhasDeValidação,
                Mensagem = $"Importação não realizada dos dados. Foram encontrados erros de validação. {msg}"
            };
        }

        public static ImportarDadosOutput ImportaçãoComFalha(string msg = "")
        {
            return new ImportarDadosOutput
            {
                Status = ImportarDadosStatus.ImportaçãoComFalha,
                Mensagem = $"Não foi possível importar os dados. {msg}"
            };
        }

       
    }
}
