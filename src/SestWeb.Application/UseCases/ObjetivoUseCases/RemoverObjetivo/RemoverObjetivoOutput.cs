using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.ObjetivoUseCases.RemoverObjetivo
{
    public class RemoverObjetivoOutput : UseCaseOutput<RemoverObjetivoStatus>
    {
        private RemoverObjetivoOutput()
        {
        }

        public static RemoverObjetivoOutput ObjetivoRemovido()
        {
            return new RemoverObjetivoOutput
            {
                Status = RemoverObjetivoStatus.ObjetivoRemovido,
                Mensagem = "Objetivo removido com sucesso."
            };
        }

        public static RemoverObjetivoOutput ObjetivoNãoRemovido(string mensagem = "")
        {
            return new RemoverObjetivoOutput
            {
                Status = RemoverObjetivoStatus.ObjetivoNãoRemovido,
                Mensagem = $"Não foi possível remover o objetivo. {mensagem}"
            };
        }

        public static RemoverObjetivoOutput PoçoNãoEncontrado(string id)
        {
            return new RemoverObjetivoOutput
            {
                Status = RemoverObjetivoStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {id}."
            };
        }
    }
}
