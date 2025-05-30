using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CargaInicial
{
    public interface ICargaInicialUseCase

    {
        Task Execute(string authorization);
    }
}
