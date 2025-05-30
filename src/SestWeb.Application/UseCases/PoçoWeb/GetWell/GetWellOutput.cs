using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.PoçoWeb.Well;

namespace SestWeb.Application.UseCases.PoçoWeb.GetWell
{
    public class GetWellOutput : UseCaseOutput<GetWellStatus>
    {
        public Well Well { get; set; }

        private GetWellOutput(GetWellStatus status)
        {
            Status = status;
        }

        public static GetWellOutput WellFoundSuccessfully(Well well)
        {
            return new GetWellOutput(GetWellStatus.WellFound)
            {
                Well = well,
                Mensagem = "Poço encontrado com sucesso."
            };
        }

        public static GetWellOutput WellNotFound(string id)
        {
            return new GetWellOutput(GetWellStatus.WellNotFound)
            {
                Mensagem = $"Não foi possível encontrar o poço com id {id}."
            };
        }

        public static GetWellOutput WellNotObtained(string message)
        {
            return new GetWellOutput(GetWellStatus.WellNotObtained)
            {
                Mensagem = $"Não foi possível obter o poço. {message}"
            };
        }
    }
}
