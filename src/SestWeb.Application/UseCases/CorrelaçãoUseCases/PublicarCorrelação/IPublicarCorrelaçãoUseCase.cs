using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.PublicarCorrelação
{
    public interface IPublicarCorrelaçãoUseCase
    {
        Task<PublicarCorrelaçãoOutput> Execute(string idPoço, string nome);
    }
}
