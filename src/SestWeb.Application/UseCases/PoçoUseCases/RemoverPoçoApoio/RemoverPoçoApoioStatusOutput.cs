using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.PoçoUseCases.RemoverPoçoApoio
{
    public class RemoverPoçoApoioOutput : UseCaseOutput<RemoverPoçoApoioStatus>
    {
        private RemoverPoçoApoioOutput()
        {
        }

        public static RemoverPoçoApoioOutput ComSucesso()
        {
            return new RemoverPoçoApoioOutput
            {
                Status = RemoverPoçoApoioStatus.ComSucesso,
                Mensagem = "Poço apoio removido com sucesso.",
            };
        }

        public static RemoverPoçoApoioOutput SemSucesso(string mensagem = "")
        {
            return new RemoverPoçoApoioOutput
            {
                Status = RemoverPoçoApoioStatus.SemSucesso,
                Mensagem = $"Não foi possível remover poço de apoio. {mensagem}"
            };
        }
    }
}