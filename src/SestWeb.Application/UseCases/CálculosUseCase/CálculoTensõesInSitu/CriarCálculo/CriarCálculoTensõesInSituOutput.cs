using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.Base;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CriarCálculo
{
    public class CriarCálculoTensõesInSituOutput : UseCaseOutput<CriarCálculoTensõesInSituStatus>
    {
        public CriarCálculoTensõesInSituOutput()
        {

        }

        public ICálculo Cálculo { get; set; }

        public static CriarCálculoTensõesInSituOutput CálculoCriado(ICálculo cálculo)
        {
            return new CriarCálculoTensõesInSituOutput
            {
                Status = CriarCálculoTensõesInSituStatus.CálculoCriado,
                Mensagem = "Cálculo tensões in situ criado.",
                Cálculo = cálculo
            };
        }

        public static CriarCálculoTensõesInSituOutput CálculoNãoCriado(string msg)
        {
            return new CriarCálculoTensõesInSituOutput
            {
                Status = CriarCálculoTensõesInSituStatus.CálculoNãoCriado,
                Mensagem = msg
            };
        }
    }
}
