using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.PoçoWeb.OilField;

namespace SestWeb.Application.UseCases.PoçoWeb.GetOilField
{
    public class GetOilFieldOutput : UseCaseOutput<GetOilFieldStatus>
    {
        public OilField Oilfield { get; set; }

        private GetOilFieldOutput(GetOilFieldStatus status)
        {
            Status = status;
        }

        public static GetOilFieldOutput OilFieldFoundSuccessfully(OilField oilField)
        {
            return new GetOilFieldOutput(GetOilFieldStatus.OilFieldFound)
            {
                Oilfield = oilField,
                Mensagem = "Campo encontrado com sucesso."
            };
        }

        public static GetOilFieldOutput OilFieldNotFound(string id)
        {
            return new GetOilFieldOutput(GetOilFieldStatus.OilFieldNotFound)
            {
                Mensagem = $"Não foi possível encontrar o campo com id {id}."
            };
        }

        public static GetOilFieldOutput OilFieldNotObtained(string message)
        {
            return new GetOilFieldOutput(GetOilFieldStatus.OilFieldNotObtained)
            {
                Mensagem = $"Não foi possível obter o campo. {message}"
            };
        }
    }
}
