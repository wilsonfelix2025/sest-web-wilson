using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.PoçoUseCases.RemoverPoço
{
    public class RemoverPoçoOutput : UseCaseOutput<RemoverPoçoStatus>
    {
        private RemoverPoçoOutput()
        {
        }

        public static RemoverPoçoOutput PoçoRemovido()
        {
            return new RemoverPoçoOutput
            {
                Status = RemoverPoçoStatus.PoçoRemovido,
                Mensagem = "Poço removido com sucesso."
            };
        }

        public static RemoverPoçoOutput PoçoNãoEncontrado(string id)
        {
            return new RemoverPoçoOutput
            {
                Status = RemoverPoçoStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {id}."
            };
        }

        public static RemoverPoçoOutput PoçoNãoRemovido(string mensagem = "")
        {
            return new RemoverPoçoOutput
            {
                Status = RemoverPoçoStatus.PoçoNãoRemovido,
                Mensagem = $"Não foi possível remover poço. {mensagem}"
            };
        }
    }
}