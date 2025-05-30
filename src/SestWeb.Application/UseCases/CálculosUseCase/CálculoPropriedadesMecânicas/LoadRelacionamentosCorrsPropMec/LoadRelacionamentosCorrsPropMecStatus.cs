using System;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.LoadRelacionamentosCorrsPropMec
{
    public enum LoadRelacionamentosCorrsPropMecStatus
    {
        RelacionamentosCarregados,
        RelacionamentosJáCarregados,
        RelacionamentosNãoCarregados,
        PoçoNãoEncontrado,
        CorrelaçõesNãoEncontradas,
        RelacionamentosNãoEncontrados
    }
}
