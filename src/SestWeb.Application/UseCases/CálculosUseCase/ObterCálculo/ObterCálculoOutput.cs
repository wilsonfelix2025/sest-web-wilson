using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.Base;

namespace SestWeb.Application.UseCases.CálculosUseCase.ObterCálculo
{
    public class ObterCálculoOutput : UseCaseOutput<ObterCálculoStatus>
    {
        public ObterCálculoOutput()
        {
            
        }

        public ICálculo Cálculo { get; set; }

        public static ObterCálculoOutput CálculoObtido(ICálculo cálculo)
        {
            return new ObterCálculoOutput
            {
                Status = ObterCálculoStatus.CálculoObtido,
                Mensagem = "Cálculo obtido com sucesso",
                Cálculo = cálculo
            };
        }

        public static ObterCálculoOutput CálculoNãoObtido(string msg)
        {
            return new ObterCálculoOutput
            {
                Status = ObterCálculoStatus.CálculoNãoObtido,
                Mensagem = $"Não foi possível obter cálculo. {msg}"
            };
        }
    }
}
