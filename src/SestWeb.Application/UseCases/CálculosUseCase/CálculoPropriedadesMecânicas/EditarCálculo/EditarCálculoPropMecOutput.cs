using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Perfis.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.EditarCálculo
{
    public class EditarCálculoPropMecOutput : UseCaseOutput<EditarCálculoPropMecStatus>
    {

        public EditarCálculoPropMecOutput()
        {

        }

        public ICálculo Cálculo { get; set; }
        public List<PerfilBase> PerfisAlterados { get; set; }

        public static EditarCálculoPropMecOutput CálculoPropMecEditado(ICálculo cálculo, List<PerfilBase> perfisAlterados)
        {
            return new EditarCálculoPropMecOutput
            {
                Status = EditarCálculoPropMecStatus.CálculoPropMecEditado,
                Mensagem = "Cálculo de propriedades mecânicas editado com sucesso",
                Cálculo = cálculo,
                PerfisAlterados = perfisAlterados
            };
        }

        public static EditarCálculoPropMecOutput CálculoPropMecNãoEditado(string msg)
        {
            return new EditarCálculoPropMecOutput
            {
                Status = EditarCálculoPropMecStatus.CálculoPropMecNãoEditado,
                Mensagem = $"Não foi possível editar cálculo de propriedades mecânicas. {msg}"
            };
        }
    }
}
