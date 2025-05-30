using SestWeb.Application.Helpers;
using SestWeb.Domain.DTOs.Cálculo.TensõesInSitu;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularK0
{
    public class CalcularK0Output : UseCaseOutput<CalcularK0Status>
    {
        public CalcularK0Output()
        {

        }

        public RetornoLotDTO Retorno { get; set; }

        public static CalcularK0Output CálculoK0Efetuado(RetornoLotDTO retorno)
        {
            return new CalcularK0Output
            {
                Status = CalcularK0Status.CálculoK0Efetuado,
                Mensagem = "Cálculo K0 efetuado.",
                Retorno = retorno
            };
        }

        public static CalcularK0Output CálculoK0NãoEfetuado(string msg)
        {
            return new CalcularK0Output
            {
                Status = CalcularK0Status.CálculoK0NãoEfetuado,
                Mensagem = msg
            };
        }

    }
}
