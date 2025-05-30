using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Perfis.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.EditarCálculo
{
    public class EditarCálculoPressãoPorosOutput : UseCaseOutput<EditarCálculoPressãoPorosStatus>
    {
        public EditarCálculoPressãoPorosOutput()
        {

        }

        public ICálculo Cálculo { get; set; }
        public List<PerfilBase> PerfisAlterados { get; set; }

        public static EditarCálculoPressãoPorosOutput CálculoPressãoPorosCriado(ICálculo cálculo, List<PerfilBase> perfisAlterados)
        {
            return new EditarCálculoPressãoPorosOutput
            {
                Status = EditarCálculoPressãoPorosStatus.CálculoPressãoPorosEditado,
                Mensagem = "Cálculo de pressão de poros criado com sucesso",
                Cálculo = cálculo,
                PerfisAlterados = perfisAlterados
            };
        }

        public static EditarCálculoPressãoPorosOutput CálculoPressãoPorosNãoCriado(string msg)
        {
            return new EditarCálculoPressãoPorosOutput
            {
                Status = EditarCálculoPressãoPorosStatus.CálculoPressãoPorosNãoEditado,
                Mensagem = $"Não foi possível criar o cálculo de pressão de poros: {msg}"
            };
        }
    }
}
