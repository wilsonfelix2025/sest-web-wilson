using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.EstratigrafiaUseCases.RemoverItemEstratigrafia
{
    public class RemoverItemEstratigrafiaOutput : UseCaseOutput<RemoverItemEstratigrafiaStatus>
    {
        private RemoverItemEstratigrafiaOutput()
        {
        }

        public static RemoverItemEstratigrafiaOutput ItemEstratigrafiaRemovido()
        {
            return new RemoverItemEstratigrafiaOutput
            {
                Status = RemoverItemEstratigrafiaStatus.ItemEstratigrafiaRemovido,
                Mensagem = "Item de estratigrafia removido com sucesso."
            };
        }

        public static RemoverItemEstratigrafiaOutput ItemEstratigrafiaNãoRemovido(string mensagem = "")
        {
            return new RemoverItemEstratigrafiaOutput
            {
                Status = RemoverItemEstratigrafiaStatus.ItemEstratigrafiaNãoRemovido,
                Mensagem = $"[RemoverItemEstratigrafia] - {mensagem}"
            };
        }
    }
}
