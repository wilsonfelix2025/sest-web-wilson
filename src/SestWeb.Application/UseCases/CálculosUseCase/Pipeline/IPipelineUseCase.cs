using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.Trend;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.Pipeline
{
    public interface IPipelineUseCase
    {
        Task<List<PerfilBase>> Execute(Poço poço, ICálculo cálculo, string idCálculo);
        Task<List<PerfilBase>> Execute(Poço poço, PerfilBase perfil);
        Task<List<PerfilBase>> Execute(Poço poço, ILitologia litologia);
        Task<List<PerfilBase>> Execute(Poço poço, Trend trend);
        Task<List<PerfilBase>> Execute(List<Cálculo> cálculos, Poço poço);



    }
}
