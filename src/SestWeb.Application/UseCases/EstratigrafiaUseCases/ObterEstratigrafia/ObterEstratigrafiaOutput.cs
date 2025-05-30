using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Poço.EstratigrafiaDoPoço;

namespace SestWeb.Application.UseCases.EstratigrafiaUseCases.ObterEstratigrafia
{
    public class ObterEstratigrafiaOutput : UseCaseOutput<ObterEstratigrafiaStatus>
    {
        private ObterEstratigrafiaOutput()
        {
        }

        public Estratigrafia Estratigrafia { get; private set; }

        public static ObterEstratigrafiaOutput EstratigrafiaObtida(Estratigrafia estratigrafia)
        {
            return new ObterEstratigrafiaOutput
            {
                Estratigrafia = estratigrafia,
                Status = ObterEstratigrafiaStatus.EstratigrafiaObtida,
                Mensagem = "Estratigrafia obtida com sucesso."
            };
        }

        public static ObterEstratigrafiaOutput PoçoNãoEncontrado(string id)
        {
            return new ObterEstratigrafiaOutput
            {
                Status = ObterEstratigrafiaStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {id}."
            };
        }

        public static ObterEstratigrafiaOutput EstratigrafiaNãoObtida(string mensagem)
        {
            return new ObterEstratigrafiaOutput
            {
                Status = ObterEstratigrafiaStatus.EstratigrafiaNãoObtida,
                Mensagem = $"[ObterEstratigrafia] - {mensagem}"
            };
        }
    }
}
