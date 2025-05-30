using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Perfis.Base;
using System.Collections.Generic;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPerfis.EditarCálculo
{
    public class EditarCálculoPerfisOutput : UseCaseOutput<EditarCálculoPerfisStatus>
    {
        public EditarCálculoPerfisOutput()
        {
            
        }

        public ICálculo Cálculo { get; set; }
        public List<PerfilBase> PerfisAlterados { get; set; }

        public static EditarCálculoPerfisOutput CálculoPerfisEditado(ICálculo cálculo, List<PerfilBase> perfisAlterados)
        {
            return new EditarCálculoPerfisOutput
            {
                Status = EditarCálculoPerfisStatus.CálculoPerfisEditado,
                Mensagem = "Cálculo de perfis editado com sucesso",
                Cálculo = cálculo,
                PerfisAlterados = perfisAlterados
            };
        }

        public static EditarCálculoPerfisOutput CálculoPerfisNãoEditado(string msg)
        {
            return new EditarCálculoPerfisOutput
            {
                Status = EditarCálculoPerfisStatus.CálculoPerfisNãoEditado,
                Mensagem = $"Não foi possível editar cálculo de perfis. {msg}"
            };
        }

        public static EditarCálculoPerfisOutput CálculoPerfisNãoEncontrado(string msg)
        {
            return new EditarCálculoPerfisOutput
            {
                Status = EditarCálculoPerfisStatus.CálculoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar cálculo de perfis."
            };
        }
    }
}
