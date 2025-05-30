using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.AtualizarCorrelaçãoPoço
{
    public interface IAtualizarCorrelaçãoPoçoUseCase
    {
        Task<AtualizarCorrelaçãoPoçoOutput> Execute(string idPoço, string nome, string descrição, string expressão);
    }
}
