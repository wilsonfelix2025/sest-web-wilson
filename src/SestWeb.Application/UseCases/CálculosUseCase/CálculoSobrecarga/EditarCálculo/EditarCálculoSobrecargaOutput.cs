using SestWeb.Application.Helpers;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoSobrecarga.CriarCálculo;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Perfis.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoSobrecarga.EditarCálculo
{
    public class EditarCálculoSobrecargaOutput : UseCaseOutput<EditarCálculoSobrecargaStatus>
    {
        public EditarCálculoSobrecargaOutput()
        {

        }

        public ICálculo Cálculo { get; set; }
        public List<PerfilBase> PerfisAlterados { get; set; }

        public static EditarCálculoSobrecargaOutput CálculoSobrecargaEditado(ICálculo cálculo, List<PerfilBase> perfisAlterados)
        {
            return new EditarCálculoSobrecargaOutput
            {
                Status = EditarCálculoSobrecargaStatus.CálculoSobrecargaEditado,
                Mensagem = "Cálculo de sobrecarga editado com sucesso",
                Cálculo = cálculo,
                PerfisAlterados = perfisAlterados
            };
        }

        public static EditarCálculoSobrecargaOutput CálculoSobrecargaNãoEditado(string msg)
        {
            return new EditarCálculoSobrecargaOutput
            {
                Status = EditarCálculoSobrecargaStatus.CálculoSobrecargaNãoEditado,
                Mensagem = msg
            };
        }
    }
}
