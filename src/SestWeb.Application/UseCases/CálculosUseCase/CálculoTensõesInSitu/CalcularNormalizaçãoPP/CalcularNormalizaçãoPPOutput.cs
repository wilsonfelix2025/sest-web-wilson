using SestWeb.Application.Helpers;
using SestWeb.Domain.DTOs.Cálculo.TensõesInSitu;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularNormalizaçãoPP
{
    public class CalcularNormalizaçãoPPOutput : UseCaseOutput<CalcularNormalizaçãoPPStatus>
    {
        public CalcularNormalizaçãoPPOutput()
        {

        }

        public RetornoLotDTO Retorno { get; set; }

        public static CalcularNormalizaçãoPPOutput CálculoNormalizaçãoPPEfetuado(RetornoLotDTO retorno)
        {
            return new CalcularNormalizaçãoPPOutput
            {
                Status = CalcularNormalizaçãoPPStatus.CálculoNormalizaçãoPPEfetuado,
                Mensagem = "Cálculo normalização PP efetuado.",
                Retorno = retorno
            };
        }

        public static CalcularNormalizaçãoPPOutput CálculoNormalizaçãoPPNãoEfetuado(string msg)
        {
            return new CalcularNormalizaçãoPPOutput
            {
                Status = CalcularNormalizaçãoPPStatus.CálculoNormalizaçãoPPNãoEfetuado,
                Mensagem = msg
            };
        }
    }
}
