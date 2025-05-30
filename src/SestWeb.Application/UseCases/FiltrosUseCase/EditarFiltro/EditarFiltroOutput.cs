using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Perfis.Base;
using System.Collections.Generic;

namespace SestWeb.Application.UseCases.FiltrosUseCase.EditarFiltro
{
    public class EditarFiltroOutput : UseCaseOutput<EditarFiltroStatus>
    {
        public EditarFiltroOutput()
        {
            
        }

        public PerfilBase Perfil { get; set; }
        public Cálculo Cálculo { get; set; }
        public List<PerfilBase> PerfisAlterados { get; set; }

        public static EditarFiltroOutput FiltroEditado(PerfilBase perfil, Cálculo cálculo, List<PerfilBase> perfisAlterados)
        {
            return new EditarFiltroOutput
            {
                Status = EditarFiltroStatus.FiltroEditado,
                Mensagem = "Filtro editado com sucesso",
                Perfil = perfil,
                Cálculo = cálculo,
                PerfisAlterados = perfisAlterados
            };
        }

        public static EditarFiltroOutput FiltroNãoEditado(string msg)
        {
            return new EditarFiltroOutput
            {
                Status = EditarFiltroStatus.FiltroNãoEditado,
                Mensagem = $"Não foi possível editar filtro. {msg}"
            };
        }
    }
}
