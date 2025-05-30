using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.PoçoWeb.Tree;

namespace SestWeb.Application.UseCases.PoçoUseCases.ObterPoço
{
    public class ObterPoçoOutput : UseCaseOutput<ObterPoçoStatus>
    {
        private ObterPoçoOutput()
        {
        }

        public Poço Poço { get; private set; }
        public Node Caminho { get; private set; }

        public static ObterPoçoOutput PoçoObtido(Poço poço, Node caminho)
        {
            return new ObterPoçoOutput
            {
                Status = ObterPoçoStatus.PoçoObtido,
                Mensagem = "Poço obtido com sucesso.",
                Poço = poço,
                Caminho = caminho
            };
        }

        public static ObterPoçoOutput PoçoObtido(Poço poço)
        {
            return new ObterPoçoOutput
            {
                Status = ObterPoçoStatus.PoçoObtido,
                Mensagem = "Poço obtido com sucesso.",
                Poço = poço,
            };
        }

        public static ObterPoçoOutput PoçoNãoEncontrado(string id)
        {
            return new ObterPoçoOutput
            {
                Status = ObterPoçoStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {id}."
            };
        }

        public static ObterPoçoOutput PoçoNãoObtido(string mensagem = "")
        {
            return new ObterPoçoOutput
            {
                Status = ObterPoçoStatus.PoçoNãoObtido,
                Mensagem = $"Não foi possível obter poço. {mensagem}"
            };
        }
    }
}