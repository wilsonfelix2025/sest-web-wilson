using SestWeb.Domain.Entities.Correlações.AutorCorrelação;

namespace SestWeb.Api.UseCases.Correlação.ObterAutores
{
    public class ObterAutoresViewModel
    {
        public ObterAutoresViewModel(Autor autor)
        {
            NomeAutor = autor.Nome;
            ChaveAutor = autor.Chave;
        }

        public string NomeAutor { get; }

        public string ChaveAutor { get; }
    }
}
