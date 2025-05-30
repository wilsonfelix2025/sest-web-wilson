using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.EditarCálculo
{
    public interface IEditarCálculoPressãoPorosUseCase
    {
        Task<EditarCálculoPressãoPorosOutput> Execute(EditarCálculoPressãoPorosInput input);
    }
}
