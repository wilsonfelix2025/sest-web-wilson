using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trend;

namespace SestWeb.Application.Repositories
{
    public interface IPerfilReadOnlyRepository
    {
        Task<PerfilBase> ObterPerfil(string id);

        Task<IReadOnlyCollection<PerfilBase>> ObterPerfisDeUmPoço(string idPoço);
        Task<IReadOnlyCollection<PerfilBase>> ObterPerfisTrechoDeUmPoço(string idPoço);
        Task<IReadOnlyCollection<PerfilBase>> ObterPerfisPorTipo(string idPoço, string mnemônico);

        Task<bool> ExistePerfilComMesmoNome(string nome, string idPoço);

        Task<bool> ExistePerfilComId(string idPerfil);
        Task<IReadOnlyCollection<PerfilBase>> ObterPerfisPorListaDeIds(List<string> listaIds);
        Task<Trend> ObterTrendDoPerfil(string id);

    }
}