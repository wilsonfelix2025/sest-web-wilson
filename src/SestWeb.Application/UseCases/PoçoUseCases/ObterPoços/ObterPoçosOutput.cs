using System.Collections.Generic;
using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.PoçoUseCases.ObterPoços
{
    public class ObterPoçosOutput : UseCaseOutput<ObterPoçosStatus>
    {
        private ObterPoçosOutput()
        {
        }

        public List<PoçoOutput> Poços { get; private set; }

        public static ObterPoçosOutput PoçosObtidos(List<PoçoOutput> poços)
        {
            return new ObterPoçosOutput
            {
                Status = ObterPoçosStatus.PoçosObtidos,
                Mensagem = "Poços obtidos com sucesso.",
                Poços = poços
            };
        }
    }
}