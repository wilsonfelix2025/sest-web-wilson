using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Correlações.PoçoCorrelação;

namespace SestWeb.Application.Repositories
{
    public interface ICorrelaçãoPoçoWriteOnlyRepository
    {
        Task CriarCorrelaçãoPoço(CorrelaçãoPoço correlaçãoPoço);

        Task<bool> RemoverCorrelaçãoPoço(string nome);

        Task<bool> UpdateCorrelaçãoPoço(CorrelaçãoPoço correlação);
    }
}
