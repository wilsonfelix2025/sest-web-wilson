using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Poço;

namespace SestWeb.Application.UseCases.PoçoUseCases.AtualizarState
{
    public class AtualizarStateOutput : UseCaseOutput<AtualizarStateStatus>
    {
        private AtualizarStateOutput()
        {
        }

        public State State { get; set; }

        public static AtualizarStateOutput StateAtualizado(State state)
        {
            return new AtualizarStateOutput
            {
                Status = AtualizarStateStatus.StateAtualizado,
                State = state,
                Mensagem = "State atualizado com sucesso."
            };
        }

        public static AtualizarStateOutput PoçoNãoEncontrado(string id)
        {
            return new AtualizarStateOutput
            {
                Status = AtualizarStateStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {id}."
            };
        }

        public static AtualizarStateOutput StateNãoAtualizado(string mensagem = "")
        {
            return new AtualizarStateOutput
            {
                Status = AtualizarStateStatus.StateNãoAtualizado,
                Mensagem = $"State não foi atualizado - {mensagem}."
            };
        }
    }
}
