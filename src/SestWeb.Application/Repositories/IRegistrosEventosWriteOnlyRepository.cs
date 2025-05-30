using System.Threading.Tasks;
using System.Collections.Generic;
using SestWeb.Domain.Entities.RegistrosEventos;
using SestWeb.Domain.Entities.Poço;

namespace SestWeb.Application.Repositories
{
    public interface IRegistrosEventosWriteOnlyRepository
    {
        Task AtualizarRegistrosEventos(Poço poçoAtualizado, IReadOnlyCollection<RegistroEvento> registrosEventos);
        Task<bool> AtualizarRegistroEvento(Poço poço, RegistroEvento registroEventoAtualizado);

    }
}