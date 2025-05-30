using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.Base;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.CriarCálculo
{
    public class CriarCálculoPressãoPorosOutput : UseCaseOutput<CriarCálculoPressãoPorosStatus>
    {
        public CriarCálculoPressãoPorosOutput() 
        {

        }

        public ICálculo Cálculo { get; set; }

        public static CriarCálculoPressãoPorosOutput CálculoPressãoPorosCriado(ICálculo cálculo)
        {
            return new CriarCálculoPressãoPorosOutput
            {
                Status = CriarCálculoPressãoPorosStatus.CálculoPressãoPorosCriado,
                Mensagem = "Cálculo de pressão de poros criado com sucesso",
                Cálculo = cálculo
            };
        }

        public static CriarCálculoPressãoPorosOutput CálculoPressãoPorosNãoCriado(string msg)
        {
            return new CriarCálculoPressãoPorosOutput
            {
                Status = CriarCálculoPressãoPorosStatus.CálculoPressãoPorosNãoCriado,
                Mensagem = $"Não foi possível criar o cálculo de pressão de poros: {msg}"
            };
        }
    }
}
