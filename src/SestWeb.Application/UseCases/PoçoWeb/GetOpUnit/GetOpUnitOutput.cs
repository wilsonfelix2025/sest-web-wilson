using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.PoçoWeb.OpUnit;

namespace SestWeb.Application.UseCases.PoçoWeb.GetOpUnit
{
    public class GetOpUnitOutput : UseCaseOutput<GetOpUnitStatus>
    {
        public OpUnit Opunit { get; set; }

        private GetOpUnitOutput(GetOpUnitStatus status)
        {
            Status = status;
        }

        public static GetOpUnitOutput OpUnitFoundSuccessfully(OpUnit opUnit)
        {
            return new GetOpUnitOutput(GetOpUnitStatus.OpUnitFound)
            {
                Opunit = opUnit,
                Mensagem = "Unidade operacional encontrada com sucesso."
            };
        }

        public static GetOpUnitOutput OpUnitNotFound(string id)
        {
            return new GetOpUnitOutput(GetOpUnitStatus.OpUnitNotFound)
            {
                Mensagem = $"Não foi possível encontrar a unidade operacional com id {id}."
            };
        }

        public static GetOpUnitOutput OpUnitNotObtained(string message)
        {
            return new GetOpUnitOutput(GetOpUnitStatus.OpUnitNotObtained)
            {
                Mensagem = $"Não foi possível obter a unidade operacional. {message}"
            };
        }
    }
}
