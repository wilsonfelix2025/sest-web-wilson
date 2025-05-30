using SestWeb.Application.Helpers;
using SestWeb.Domain.DTOs.Cálculo.TensõesInSitu;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularNormalizaçãoLDA
{
    public class CalcularNormalizaçãoLDAOutput : UseCaseOutput<CalcularNormalizaçãoLDAStatus>
    {
        public CalcularNormalizaçãoLDAOutput()
        {

        }

        public RetornoLotDTO Retorno { get; set; }

        public static CalcularNormalizaçãoLDAOutput CálculoNormalizaçãoLDAEfetuado(RetornoLotDTO retorno)
        {
            return new CalcularNormalizaçãoLDAOutput
            {
                Status = CalcularNormalizaçãoLDAStatus.CálculoNormalizaçãoLDAEfetuado,
                Mensagem = "Cálculo normalização LDA efetuado.",
                Retorno = retorno
            };
        }

        public static CalcularNormalizaçãoLDAOutput CálculoNormalizaçãoLDANãoEfetuado(string msg)
        {
            return new CalcularNormalizaçãoLDAOutput
            {
                Status = CalcularNormalizaçãoLDAStatus.CálculoNormalizaçãoLDANãoEfetuado,
                Mensagem = msg
            };
        }
    }
}
