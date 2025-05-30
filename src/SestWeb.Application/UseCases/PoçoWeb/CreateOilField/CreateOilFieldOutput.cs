using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.PoçoWeb.OilField;

namespace SestWeb.Application.UseCases.PoçoWeb.CreateOilField
{
    public class CreateOilFieldOutput : UseCaseOutput<CreateOilFieldStatus>
    {
        public string Name { get; set; }
        public string OpUnit { get; set; }

        private CreateOilFieldOutput(CreateOilFieldStatus status)
        {
            Status = status;
        }

        public static CreateOilFieldOutput OilFieldCreatedSuccesfully(OilField oilField)
        {
            return new CreateOilFieldOutput(CreateOilFieldStatus.OilFieldCreated)
            {
                Name = oilField.Name,
                OpUnit = oilField.OpUnitId,
                Mensagem = "Campo criado com sucesso."
            };
        }

        public static CreateOilFieldOutput OilFieldNotCreated(string message)
        {
            return new CreateOilFieldOutput(CreateOilFieldStatus.OilFieldNotCreated)
            {
                Mensagem = $"Não foi possível criar o campo. {message}"
            };
        }
    }
}
