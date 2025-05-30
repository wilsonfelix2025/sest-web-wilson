using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.EstratigrafiaUseCases.AdicionarItemEstratigrafia
{
    public class AdicionarItemEstratigrafiaOutput : UseCaseOutput<AdicionarItemEstratigrafiaStatus>
    {
        private AdicionarItemEstratigrafiaOutput()
        {
        }

        public static AdicionarItemEstratigrafiaOutput ItemEstratigrafiaAdicionado()
        {
            return new AdicionarItemEstratigrafiaOutput
            {
                Status = AdicionarItemEstratigrafiaStatus.ItemAdicionado,
                Mensagem = "Item de estratigrafia adicionado com sucesso."
            };
        }

        public static AdicionarItemEstratigrafiaOutput ItemEstratigrafiaNãoAdicionado(string mensagem = "")
        {
            return new AdicionarItemEstratigrafiaOutput
            {
                Status = AdicionarItemEstratigrafiaStatus.ItemNãoAdicionado,
                Mensagem = $"[AdicionarItemEstratigrafia] - {mensagem}"
            };
        }
    }
}
