using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.Base;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoGradientes.Recalcular
{
    public class RecalcularCálculoGradientesOutput : UseCaseOutput<RecalcularCálculoGradientesStatus>
    {
        public RecalcularCálculoGradientesOutput()
        {

        }

        public ICálculo Cálculo { get; set; }

        public static RecalcularCálculoGradientesOutput CálculoRecalculado(ICálculo cálculo)
        {
            return new RecalcularCálculoGradientesOutput
            {
                Status = RecalcularCálculoGradientesStatus.CálculoRecalculado,
                Mensagem = "Cálculo de gradientes in situ recalculado.",
                Cálculo = cálculo
            };
        }

        public static RecalcularCálculoGradientesOutput CálculoNãoRecalculado(string msg)
        {
            return new RecalcularCálculoGradientesOutput
            {
                Status = RecalcularCálculoGradientesStatus.CálculoNãoRecalculado,
                Mensagem = msg
            };
        }
    }
}
