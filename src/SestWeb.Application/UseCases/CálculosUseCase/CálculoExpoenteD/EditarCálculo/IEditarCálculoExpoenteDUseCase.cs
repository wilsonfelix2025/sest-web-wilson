using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoExpoenteD.EditarCálculo
{
    public interface IEditarCálculoExpoenteDUseCase
    {
        Task<EditarCálculoExpoenteDOutput> Execute(EditarCálculoExpoenteDInput input);
    }
}
