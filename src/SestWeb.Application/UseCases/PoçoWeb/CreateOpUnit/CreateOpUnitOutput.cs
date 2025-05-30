using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.PoçoWeb.OpUnit;

namespace SestWeb.Application.UseCases.PoçoWeb.CreateOpUnit
{
    public class CreateOpUnitOutput : UseCaseOutput<CreateOpUnitStatus>
    {
        public string Name { get; set; }

        private CreateOpUnitOutput(CreateOpUnitStatus status)
        {
            Status = status;
        }

        public static CreateOpUnitOutput OpUnitCreatedSuccesfully(OpUnit opUnit)
        {
            return new CreateOpUnitOutput(CreateOpUnitStatus.OpUnitCreated)
            {
                Name = opUnit.Name,
                Mensagem = "Unidade operacional criada com sucesso.",
            };
        }

        public static CreateOpUnitOutput OpUnitNotCreated(string message)
        {
            return new CreateOpUnitOutput(CreateOpUnitStatus.OpUnitNotCreated)
            {
                Mensagem = $"Não foi possível criar a unidade operacional. {message}",
            };
        }
    }
}
