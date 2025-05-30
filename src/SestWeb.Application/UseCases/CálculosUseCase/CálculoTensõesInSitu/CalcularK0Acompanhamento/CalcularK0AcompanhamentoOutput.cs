using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularK0Acompanhamento
{
    public class CalcularK0AcompanhamentoOutput : UseCaseOutput<CalcularK0AcompanhamentoStatus>
    {
        public CalcularK0AcompanhamentoOutput()
        {

        }

        public PerfilBase Perfil { get; set; }

        public static CalcularK0AcompanhamentoOutput CálculoK0AcompanhamentoEfetuado(PerfilBase perfil)
        {
            return new CalcularK0AcompanhamentoOutput
            {
                Status = CalcularK0AcompanhamentoStatus.CálculoK0AcompanhamentoEfetuado,
                Mensagem = "Cálculo K0 Acompanhamento efetuado.",
                Perfil = perfil
            };
        }

        public static CalcularK0AcompanhamentoOutput CálculoK0AcompanhamentoNãoEfetuado(string msg)
        {
            return new CalcularK0AcompanhamentoOutput
            {
                Status = CalcularK0AcompanhamentoStatus.CálculoK0AcompanhamentoNãoEfetuado,
                Mensagem = msg
            };
        }

    }
}
