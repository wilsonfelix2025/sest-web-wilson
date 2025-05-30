using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Application.Repositories
{
    public interface ICorrelaçãoWriteOnlyRepository
    {
        Task CriarCorrelação(Correlação correlação);

        Task<bool> RemoverCorrelação(string nome);

        Task<bool> UpdateCorrelação(Correlação correlação);

        Task<bool> InsertCorrelaçõesSistema(IList<Correlação> correlações);
    }
}
