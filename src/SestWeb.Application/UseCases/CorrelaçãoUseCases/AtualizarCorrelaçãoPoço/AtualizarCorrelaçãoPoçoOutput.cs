using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Correlações.PoçoCorrelação;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.AtualizarCorrelaçãoPoço
{
    public class AtualizarCorrelaçãoPoçoOutput : UseCaseOutput<AtualizarCorrelaçãoPoçoStatus>
    {
        private AtualizarCorrelaçãoPoçoOutput() { }

        private CorrelaçãoPoço CorrelaçãoPoço { get; set; }

        public static AtualizarCorrelaçãoPoçoOutput CorrelaçãoAtualizada(CorrelaçãoPoço correlaçãoPoço)
        {
            return new AtualizarCorrelaçãoPoçoOutput
            {
                Status = AtualizarCorrelaçãoPoçoStatus.CorrelaçãoAtualizada,
                Mensagem = "Correlação atualizada com sucesso.",
                CorrelaçãoPoço = correlaçãoPoço
            };
        }

        public static AtualizarCorrelaçãoPoçoOutput CorrelaçãoNãoEncontrada(string nome)
        {
            return new AtualizarCorrelaçãoPoçoOutput
            {
                Status = AtualizarCorrelaçãoPoçoStatus.CorrelaçãoNãoEncontrada,
                Mensagem = $"Não foi possível encontrar uma correlação com esse nome: {nome}"
            };
        }

        public static AtualizarCorrelaçãoPoçoOutput CorrelaçãoNãoAtualizada(string nome)
        {
            return new AtualizarCorrelaçãoPoçoOutput
            {
                Status = AtualizarCorrelaçãoPoçoStatus.CorrelaçãoNãoAtualizada,
                Mensagem = $"Não foi possível atualizar correlação: {nome}"
            };
        }

        public static AtualizarCorrelaçãoPoçoOutput PoçoNãoEncontrado(string idPoço)
        {
            return new AtualizarCorrelaçãoPoçoOutput
            {
                Status = AtualizarCorrelaçãoPoçoStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {idPoço}."
            };
        }
    }
}
