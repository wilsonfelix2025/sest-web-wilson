using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterTodasCorrsLeve
{
    public interface IObterTodasCorrsLeveUseCase
    {
        Task<ObterTodasCorrsLeveOutput> Execute(string idPoço);
    }
}
