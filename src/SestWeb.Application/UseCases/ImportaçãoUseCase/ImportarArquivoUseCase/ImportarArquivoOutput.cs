using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarArquivoUseCase
{
    public class ImportarArquivoOutput : UseCaseOutput<ImportarArquivoStatus>
    {
        public ImportarArquivoOutput()
        {

        }


        public static ImportarArquivoOutput ImportadoComSucesso()
        {
            return new ImportarArquivoOutput
            {
                Status = ImportarArquivoStatus.ImportadoComSucesso,
                Mensagem = "Importação do arquivo feita com sucesso."
            };
        }

        public static ImportarArquivoOutput ImportaçãoComFalhasDeValidação(string msg = "")
        {
            return new ImportarArquivoOutput
            {
                Status = ImportarArquivoStatus.ImportaçãoComFalhasDeValidação,
                Mensagem = $"Importação não realizada. Foram encontrados erros de validação. {msg}"
            };
        }

        public static ImportarArquivoOutput ImportaçãoComFalha(string msg = "")
        {
            return new ImportarArquivoOutput
            {
                Status = ImportarArquivoStatus.ImportaçãoComFalha,
                Mensagem = $"Não foi possível importar o arquivo. {msg}"
            };
        }

        public static ImportarArquivoOutput ImportaçãoCanceladaPerfisIncompletos(string nomePerfil = "")
        {
            return new ImportarArquivoOutput
            {
                Status = ImportarArquivoStatus.ImportaçãoComFalhasDeValidação,
                Mensagem = $"O nome, tipo e unidade de todos os perfis devem ser preenchidos. Perfil incompleto: {nomePerfil}"
            };
        }
    }
}
