using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Correlações.AutorCorrelação;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.PoçoCorrelação;

namespace SestWeb.Application.Repositories
{
    public interface ICorrelaçãoPoçoReadOnlyRepository
    {
        Task<bool> ExisteCorrelaçãoPoço(string idPoço, string nome);
        Task<Correlação> ObterCorrelaçãoPoçoPeloNome(string idPoço, string nome);
        Task<IReadOnlyCollection<Correlação>> ObterCorrelaçõesPoçoPorChaveUsuário(string idPoço, string userKey);
        Task<IReadOnlyCollection<Correlação>> ObterCorrelaçõesPoçoPorMnemônico(string idPoço, string mnemônico);
        Task<IReadOnlyCollection<Autor>> ObterAutoresCorrelaçõesPoço(string idPoço);
        Task<IReadOnlyCollection<Correlação>> ObterTodasCorrelaçõesPoço(string idPoço);
        Task<bool> ExistemCorrelaçõesPoço(string idPoço);
    }
}
