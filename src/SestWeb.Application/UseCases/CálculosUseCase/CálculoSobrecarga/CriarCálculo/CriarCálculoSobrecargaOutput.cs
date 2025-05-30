using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.Base;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoSobrecarga.CriarCálculo
{
    public class CriarCálculoSobrecargaOutput : UseCaseOutput<CriarCálculoSobrecargaStatus>
    {
        public CriarCálculoSobrecargaOutput() 
        {

        }

        public ICálculo Cálculo { get; set; }

        public static CriarCálculoSobrecargaOutput CálculoSobrecargaCriado(ICálculo cálculo)
        {
            return new CriarCálculoSobrecargaOutput
            {
                Status = CriarCálculoSobrecargaStatus.CálculoSobrecargaCriado,
                Mensagem = "Cálculo de sobrecarga criado com sucesso",
                Cálculo = cálculo
            };
        }

        public static CriarCálculoSobrecargaOutput CálculoSobrecargaNãoCriado(string msg)
        {
            return new CriarCálculoSobrecargaOutput
            {
                Status = CriarCálculoSobrecargaStatus.CálculoSobrecargaNãoCriado,
                Mensagem = $"Não foi possível criar o cálculo de sobrecarga: {msg}"
            };
        }
    }
}
