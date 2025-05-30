using SestWeb.Application.Repositories;

namespace SestWeb.Api.UseCases.Correlação.ObterTodasCorrsLeve
{
    public class ObterTodasCorrsLeveViewModel
    {
        public ObterTodasCorrsLeveViewModel(Domain.Entities.Correlações.Base.Correlação corr)
        {
            Nome = corr.Nome;
            PerfilSaída = corr.PerfisSaída.Tipos[0];
            ChaveAutor = corr.Autor.Chave;
            Origem = corr.Origem.ToString();
        }

        public string Nome { get; }

        public string PerfilSaída { get; }

        public string ChaveAutor { get; }

        public string Origem { get; }
    }
}
