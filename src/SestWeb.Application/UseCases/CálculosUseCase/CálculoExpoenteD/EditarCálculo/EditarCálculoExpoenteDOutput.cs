using System;
using System.Collections.Generic;
using System.Text;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoExpoenteD.EditarCálculo
{
    public class EditarCálculoExpoenteDOutput : UseCaseOutput<EditarCálculoExpoenteDStatus>
    {
        public EditarCálculoExpoenteDOutput() { }

        public ICálculo Cálculo { get; set; }
        public List<PerfilBase> PerfisAlterados { get; set; }

        public static EditarCálculoExpoenteDOutput CálculoEditado(ICálculo cálculo, List<PerfilBase> perfisAlterados)
        {
            return new EditarCálculoExpoenteDOutput
            {
                Status = EditarCálculoExpoenteDStatus.CálculoEditado,
                Mensagem = "Cálculo de ExpoenteD editado.",
                Cálculo = cálculo,
                PerfisAlterados = perfisAlterados
            };
        }

        public static EditarCálculoExpoenteDOutput CálculoNãoEditado(string msg)
        {
            return new EditarCálculoExpoenteDOutput
            {
                Status = EditarCálculoExpoenteDStatus.CálculoNãoEditado,
                Mensagem = msg
            };
        }
    }
}
