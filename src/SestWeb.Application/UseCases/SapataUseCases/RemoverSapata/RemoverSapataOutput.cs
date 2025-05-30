using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.SapataUseCases.RemoverSapata
{
    public class RemoverSapataOutput : UseCaseOutput<RemoverSapataStatus>
    {
        private RemoverSapataOutput()
        {
        }

        public static RemoverSapataOutput SapataRemovida()
        {
            return new RemoverSapataOutput
            {
                Status = RemoverSapataStatus.SapataRemovida,
                Mensagem = "Sapata removida com sucesso."
            };
        }

        public static RemoverSapataOutput SapataNãoRemovida(string mensagem = "")
        {
            return new RemoverSapataOutput
            {
                Status = RemoverSapataStatus.SapataNãoRemovida,
                Mensagem = $"Não foi possível remover a sapata. {mensagem}"
            };
        }

        public static RemoverSapataOutput PoçoNãoEncontrado(string id)
        {
            return new RemoverSapataOutput
            {
                Status = RemoverSapataStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {id}."
            };
        }
    }
}
