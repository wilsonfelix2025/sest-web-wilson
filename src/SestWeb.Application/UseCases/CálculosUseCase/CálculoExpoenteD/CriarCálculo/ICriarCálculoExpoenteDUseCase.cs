using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoExpoenteD.CriarCálculo
{
    public interface ICriarCálculoExpoenteDUseCase
    {
        Task<CriarCálculoExpoenteDOutput> Execute(CriarCálculoExpoenteDInput input);
    }
}
