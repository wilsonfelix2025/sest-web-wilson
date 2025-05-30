using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.CálculosUseCase.RemoverCálculo
{
    public class RemoverCálculoOutput : UseCaseOutput<RemoverCálculoStatus>
    {
        public RemoverCálculoOutput()
        {
            
        }

        public static RemoverCálculoOutput CálculoRemovido()
        {
            return new RemoverCálculoOutput
            {
                Status = RemoverCálculoStatus.CálculoRemovido,
                Mensagem = "Cálculo removido com sucesso",
            };
        }

        public static RemoverCálculoOutput CálculoNãoRemovido(string msg)
        {
            return new RemoverCálculoOutput
            {
                Status = RemoverCálculoStatus.CálculoNãoRemovido,
                Mensagem = $"Não foi possível remover o cálculo. {msg}"
            };
        }

        public static RemoverCálculoOutput CálculoNãoEncontrado()
        {
            return new RemoverCálculoOutput
            {
                Status = RemoverCálculoStatus.CálculoNãoEncontrado,
                Mensagem = $"Cálculo não encontrado."
            };
        }
    }
}
