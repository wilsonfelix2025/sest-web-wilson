using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.RegistrosEventos;

namespace SestWeb.Application.Repositories
{
    public interface IRegistrosEventosReadOnlyRepository
    {
        Task<RegistroEvento> ObterRegistroEvento(string id);

        Task<IReadOnlyCollection<RegistroEvento>> ObterRegistrosEventosDeUmPoço(string idPoço);

        Task<bool> ExisteRegistroEventoComId(string id);
        Task<IReadOnlyCollection<RegistroEvento>> ObterRegistrosEventosPorListaDeIds(List<string> listaIds);

    }
}