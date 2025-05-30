using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.PoçoWeb.Well;

namespace SestWeb.Application.UseCases.PoçoWeb.CreateWell
{
    public class CreateWellOutput : UseCaseOutput<CreateWellStatus>
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Oilfield { get; set; }

        private CreateWellOutput(CreateWellStatus status)
        {
            Status = status;
        }

        public static CreateWellOutput WellCreatedSuccesfully(Well well)
        {
            return new CreateWellOutput(CreateWellStatus.WellCreated)
            {
                Name = well.Name,
                Id = well.Id,
                Oilfield = well.OilFieldId,
                Mensagem = "Poço criado com sucesso."
            };
        }

        public static CreateWellOutput WellNotCreated(string message)
        {
            return new CreateWellOutput(CreateWellStatus.WellNotCreated)
            {
                Mensagem = $"Não foi possível criar o poço. {message}"
            };
        }
    }
}
