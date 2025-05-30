using SestWeb.Application.Helpers;
using SestWeb.Domain.Importadores.Shallow;

namespace SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarArquivoUseCase.LerArquivoPoçoWebUseCase
{
    public class LerArquivoPoçoWebOutput : UseCaseOutput<LerArquivoPoçoWebStatus>
    {
        public LerArquivoPoçoWebOutput()
        {
            
        }

        public RetornoDTO Retorno { get; set; }
        private string ObjJson { get; set; }

        public static LerArquivoPoçoWebOutput LeituraComSucesso(RetornoDTO retorno, string objJson)
        {
            return new LerArquivoPoçoWebOutput
            {
                Status = LerArquivoPoçoWebStatus.LeituraComSucesso,
                Mensagem = "Leitura realizada com sucesso.",
                Retorno = retorno,
                ObjJson = objJson
            };
        }

        public static LerArquivoPoçoWebOutput LeituraSemSucesso(string mensagem)
        {
            return new LerArquivoPoçoWebOutput
            {
                Status = LerArquivoPoçoWebStatus.LeituraSemSucesso,
                Mensagem = $"Não foi possível ler o arquivo. {mensagem}"
            };
        }
    }
}
