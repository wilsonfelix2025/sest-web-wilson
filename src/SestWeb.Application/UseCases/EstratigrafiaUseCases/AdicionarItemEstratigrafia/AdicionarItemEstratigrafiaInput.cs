using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Application.UseCases.EstratigrafiaUseCases.AdicionarItemEstratigrafia
{
    public class AdicionarItemEstratigrafiaInput
    {
        public AdicionarItemEstratigrafiaInput(string idPoço, string tipo, double pm, string sigla, string descrição, string idade)
        {
            IdPoço = idPoço;
            Tipo = tipo;
            PM = new Profundidade(pm);
            Sigla = sigla;
            Descrição = descrição;
            Idade = idade;
        }

        public string IdPoço { get; }
        public string Tipo { get; }
        public Profundidade PM { get; }
        public string Sigla { get; }
        public string Descrição { get; }
        public string Idade { get; }
    }
}
