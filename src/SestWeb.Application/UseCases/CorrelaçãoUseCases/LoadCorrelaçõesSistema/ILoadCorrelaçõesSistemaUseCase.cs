using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.LoadCorrelaçõesSistema
{
    public interface ILoadCorrelaçõesSistemaUseCase
    {
        Task<LoadCorrelaçõesSistemaOutput> Execute();
    }
}
