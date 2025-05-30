using SestWeb.Application.Helpers;
using System.Collections.Generic;
using SestWeb.Domain.Entities.PoçoWeb.Tree;

namespace SestWeb.Application.UseCases.PoçoWeb.GetTree
{
    public class GetTreeOutput : UseCaseOutput<GetTreeStatus>
    {
        public List<Node> Tree { get; set; }

        private GetTreeOutput(GetTreeStatus status)
        {
            Status = status;
        }

        public static GetTreeOutput TreeFoundSuccessfully(List<Node> tree)
        {
            return new GetTreeOutput(GetTreeStatus.TreeFound)
            {
                Tree = tree,
                Mensagem = "Campo encontrado com sucesso."
            };
        }

        public static GetTreeOutput TreeNotFound(string id)
        {
            return new GetTreeOutput(GetTreeStatus.TreeNotFound)
            {
                Mensagem = $"Não foi possível encontrar o campo com id {id}."
            };
        }

        public static GetTreeOutput TreeNotObtained(string message)
        {
            return new GetTreeOutput(GetTreeStatus.TreeNotObtained)
            {
                Mensagem = $"Não foi possível obter o campo. {message}"
            };
        }
    }
}
