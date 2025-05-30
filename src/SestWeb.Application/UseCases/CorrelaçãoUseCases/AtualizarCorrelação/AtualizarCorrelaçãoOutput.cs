using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.AtualizarCorrelação
{
    public class AtualizarCorrelaçãoOutput : UseCaseOutput<AtualizarCorrelaçãoStatus>
    {
        private AtualizarCorrelaçãoOutput() { }

        private Correlação Correlação { get; set; }

        public static AtualizarCorrelaçãoOutput CorrelaçãoAtualizada(Correlação correlação)
        {
            return new AtualizarCorrelaçãoOutput
            {
                Status = AtualizarCorrelaçãoStatus.CorrelaçãoAtualizada,
                Mensagem = "Correlação atualizada com sucesso.",
                Correlação = correlação
            };
        }

        public static AtualizarCorrelaçãoOutput CorrelaçãoNãoEncontrada(string nome)
        {
            return new AtualizarCorrelaçãoOutput
            {
                Status = AtualizarCorrelaçãoStatus.CorrelaçãoNãoEncontrada,
                Mensagem = $"Não foi possível encontrar uma correlação com esse nome: {nome}"
            };
        }

        public static AtualizarCorrelaçãoOutput CorrelaçãoNãoAtualizada(string nome)
        {
            return new AtualizarCorrelaçãoOutput
            {
                Status = AtualizarCorrelaçãoStatus.CorrelaçãoNãoAtualizada,
                Mensagem = $"Não foi possível atualizar correlação: {nome}"
            };
        }
    }
}
