using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.LitologiaUseCases.RemoverLitologia
{
    public class RemoverLitologiaOutput : UseCaseOutput<RemoverLitologiaStatus>
    {
        private RemoverLitologiaOutput()
        {
        }

        public static RemoverLitologiaOutput LitologiaRemovida()
        {
            return new RemoverLitologiaOutput
            {
                Status = RemoverLitologiaStatus.LitologiaRemovida,
                Mensagem = "Litologia removida com sucesso."
            };
        }

        public static RemoverLitologiaOutput LitologiaNãoRemovida(string mensagem = "")
        {
            return new RemoverLitologiaOutput
            {
                Status = RemoverLitologiaStatus.LitologiaNãoRemovida,
                Mensagem = $"Não foi possível remover a litologia. {mensagem}"
            };
        }

        public static RemoverLitologiaOutput PoçoNãoEncontrado(string id)
        {
            return new RemoverLitologiaOutput
            {
                Status = RemoverLitologiaStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {id}."
            };
        }
    }
}
