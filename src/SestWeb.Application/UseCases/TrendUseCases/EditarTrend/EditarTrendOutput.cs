using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trend;
using System.Collections.Generic;

namespace SestWeb.Application.UseCases.TrendUseCases.EditarTrend
{
    public class EditarTrendOutput : UseCaseOutput<EditarTrendStatus>
    {
        public EditarTrendOutput()
        {
            
        }

        public Trend Trend { get; set; }
        public List<PerfilBase> PerfisAlterados { get; set; }

        public static EditarTrendOutput TrendEditado(Trend trend, List<PerfilBase> perfisAlterados)
        {
            return new EditarTrendOutput
            {
                Status = EditarTrendStatus.TrendEditado,
                Mensagem = "Trend editado com sucesso",
                Trend = trend,
                PerfisAlterados = perfisAlterados
            };
        }

        public static EditarTrendOutput TrendNãoEditado(string msg)
        {
            return new EditarTrendOutput
            {
                Status = EditarTrendStatus.TrendNãoEditado,
                Mensagem = $"Não foi possível editar trend. {msg}"
            };
        }
    }
}
