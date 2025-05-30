using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoSobrecarga.EditarCálculo
{
    public interface IEditarCálculoSobrecargaUseCase
    {
        Task<EditarCálculoSobrecargaOutput> Execute(EditarCálculoSobrecargaInput input);
    }
}
