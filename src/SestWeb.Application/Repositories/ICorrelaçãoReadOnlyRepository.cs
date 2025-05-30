using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Correlações.AutorCorrelação;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Application.Repositories
{
    public interface ICorrelaçãoReadOnlyRepository
    {
        Task<bool> ExisteCorrelação(string nome);

        Task<Correlação> ObterCorrelaçãoPeloNome(string nome);
        Task<IReadOnlyCollection<Correlação>> ObterCorrelaçõesPorNomes(List<string> listaNomes);
        Task<IReadOnlyCollection<Correlação>> ObterCorrelaçõesPorChaveUsuário(string userKey);

        Task<IReadOnlyCollection<Correlação>> ObterCorrelaçõesPorMnemônico(string mnemônico);

        Task<IReadOnlyCollection<Correlação>> ObterCorrelaçõesFixas();

        Task<IReadOnlyCollection<Correlação>> ObterCorrelaçõesDeUsuários();

        Task<IReadOnlyCollection<Correlação>> ObterTodasCorrelações();

        Task<IReadOnlyCollection<Autor>> ObterAutoresCorrelações();

        Task<bool> ExistemCorrelações();

        Task<bool> CorrelaçõesSistemaCarregadas();
    }
}
