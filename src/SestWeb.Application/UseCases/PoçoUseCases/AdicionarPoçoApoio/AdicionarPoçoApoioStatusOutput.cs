using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.PoçoUseCases.AdicionarPoçoApoio
{
    public class AdicionarPoçoApoioOutput : UseCaseOutput<AdicionarPoçoApoioStatus>
    {
        private AdicionarPoçoApoioOutput()
        {
        }

        public static AdicionarPoçoApoioOutput ComSucesso()
        {
            return new AdicionarPoçoApoioOutput
            {
                Status = AdicionarPoçoApoioStatus.ComSucesso,
                Mensagem = "Poço apoio adicionado com sucesso.",
            };
        }

        public static AdicionarPoçoApoioOutput SemSucesso(string mensagem = "")
        {
            return new AdicionarPoçoApoioOutput
            {
                Status = AdicionarPoçoApoioStatus.SemSucesso,
                Mensagem = $"Não foi possível adicionar poço de apoio. {mensagem}"
            };
        }
    }
}