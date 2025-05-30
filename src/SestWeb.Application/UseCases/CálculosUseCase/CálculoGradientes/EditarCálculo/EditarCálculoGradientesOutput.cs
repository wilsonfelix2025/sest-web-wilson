using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Perfis.Base;
using System.Collections.Generic;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoGradientes.EditarCálculo
{
    public class EditarCálculoGradientesOutput : UseCaseOutput<EditarCálculoGradientesStatus>
    {
        public EditarCálculoGradientesOutput()
        {

        }

        public ICálculo Cálculo { get; set; }
        public List<PerfilBase> PerfisAlterados { get; set; }

        public static EditarCálculoGradientesOutput CálculoEditado(ICálculo cálculo, List<PerfilBase> perfisAlterados)
        {
            return new EditarCálculoGradientesOutput
            {
                Status = EditarCálculoGradientesStatus.CálculoEditado,
                Mensagem = "Cálculo de gradientes in situ editado.",
                Cálculo = cálculo,
                PerfisAlterados = perfisAlterados
            };
        }

        public static EditarCálculoGradientesOutput CálculoNãoEditado(string msg)
        {
            return new EditarCálculoGradientesOutput
            {
                Status = EditarCálculoGradientesStatus.CálculoNãoEditado,
                Mensagem = msg
            };
        }
    }
}
