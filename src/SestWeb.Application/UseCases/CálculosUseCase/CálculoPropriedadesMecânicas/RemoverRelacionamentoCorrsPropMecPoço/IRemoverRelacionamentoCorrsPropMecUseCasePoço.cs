using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.RemoverRelacionamentoCorrsPropMecPoço
{
    public interface IRemoverRelacionamentoCorrsPropMecUseCasePoço
    {
        Task<RemoverRelacionamentoCorrsPropMecOutputPoço> Execute(string idPoço, string nome);
    }
}
