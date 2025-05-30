using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Perfis.Base;
using System.Collections.Generic;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.EditarCálculo
{
    public class EditarCálculoTensõesInSituOutput : UseCaseOutput<EditarCálculoTensõesInSituStatus>
    {
        public EditarCálculoTensõesInSituOutput()
        {

        }

        public ICálculo Cálculo { get; set; }
        public List<PerfilBase> PerfisAlterados { get; set; }

        public static EditarCálculoTensõesInSituOutput CálculoEditado(ICálculo cálculo, List<PerfilBase> perfisAlterados)
        {
            return new EditarCálculoTensõesInSituOutput
            {
                Status = EditarCálculoTensõesInSituStatus.CálculoEditado,
                Mensagem = "Cálculo tensões in situ editado.",
                Cálculo = cálculo,
                PerfisAlterados = perfisAlterados
            };
        }

        public static EditarCálculoTensõesInSituOutput CálculoNãoEditado(string msg)
        {
            return new EditarCálculoTensõesInSituOutput
            {
                Status = EditarCálculoTensõesInSituStatus.CálculoNãoEditado,
                Mensagem = msg
            };
        }
    }
}
