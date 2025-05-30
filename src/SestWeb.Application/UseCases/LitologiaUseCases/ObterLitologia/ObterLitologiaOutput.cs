using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Application.UseCases.LitologiaUseCases.ObterLitologia
{
    public class ObterLitologiaOutput : UseCaseOutput<ObterLitologiaStatus>
    {
        private ObterLitologiaOutput()
        {
        }

        public Litologia Litologia { get; private set; }

        public static ObterLitologiaOutput LitologiaObtida(Litologia litologia)
        {
            return new ObterLitologiaOutput
            {
                Litologia = litologia,
                Status = ObterLitologiaStatus.LitologiaObtida,
                Mensagem = "Litologia obtida com sucesso."
            };
        }

        public static ObterLitologiaOutput PoçoNãoEncontrado(string id)
        {
            return new ObterLitologiaOutput
            {
                Status = ObterLitologiaStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {id}."
            };
        }

        public static ObterLitologiaOutput LitologiaNãoObtida(string mensagem = "")
        {
            return new ObterLitologiaOutput
            {
                Status = ObterLitologiaStatus.LitologiaNãoObtida,
                Mensagem = $"Litologia não obtida. {mensagem}"
            };
        }


    }
}