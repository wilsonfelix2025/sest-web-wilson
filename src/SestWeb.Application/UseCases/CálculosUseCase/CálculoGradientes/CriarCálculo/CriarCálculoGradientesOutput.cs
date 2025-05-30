using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.Base;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoGradientes.CriarCálculo
{
    public class CriarCálculoGradientesOutput : UseCaseOutput<CriarCálculoGradientesStatus>
    {
        public CriarCálculoGradientesOutput()
        {

        }

        public ICálculo Cálculo { get; set; }

        public static CriarCálculoGradientesOutput CálculoCriado(ICálculo cálculo)
        {
            return new CriarCálculoGradientesOutput
            {
                Status = CriarCálculoGradientesStatus.CálculoCriado,
                Mensagem = "Cálculo de gradientes in situ criado.",
                Cálculo = cálculo
            };
        }

        public static CriarCálculoGradientesOutput CálculoNãoCriado(string msg)
        {
            return new CriarCálculoGradientesOutput
            {
                Status = CriarCálculoGradientesStatus.CálculoNãoCriado,
                Mensagem = msg
            };
        }
    }
}
