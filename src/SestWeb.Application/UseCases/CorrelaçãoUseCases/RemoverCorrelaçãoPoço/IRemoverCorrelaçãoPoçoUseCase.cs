using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.RemoverCorrelaçãoPoço
{
    public interface IRemoverCorrelaçãoPoçoUseCase
    {
        Task<RemoverCorrelaçãoPoçoOutput> Execute(string idPoço, string nome);
    }
}
