using System.Collections.Generic;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Application.UseCases.LitologiaUseCases.ObterLitologias
{
    public class ObterLitologiasOutput : UseCaseOutput<ObterLitologiasStatus>
    {
        private ObterLitologiasOutput()
        {
        }

        public IReadOnlyCollection<Litologia> Litologias { get; private set; }

        public static ObterLitologiasOutput LitologiasObtidas(IReadOnlyCollection<Litologia> litologias)
        {
            return new ObterLitologiasOutput
            {
                Litologias = litologias,
                Status = ObterLitologiasStatus.LitologiasObtidas,
                Mensagem = "Litologias obtidas com sucesso."
            };
        }

        public static ObterLitologiasOutput PoçoNãoEncontrado(string id)
        {
            return new ObterLitologiasOutput
            {
                Status = ObterLitologiasStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {id}."
            };
        }

        public static ObterLitologiasOutput LitologiasNãoObtidas(string mensagem = "")
        {
            return new ObterLitologiasOutput
            {
                Status = ObterLitologiasStatus.LitologiasNãoObtidas,
                Mensagem = $"Litologias não obtidas. {mensagem}"
            };
        }


    }
}
