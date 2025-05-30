using System;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.CriarRelacionamentoCorrsPropMecPoço
{
    public enum CriarRelacionamentoCorrsPropMecPoçoStatus
    {
        RelacionamentoCriado,
        RelacionamentoExistente,
        RelacionamentoNãoCriado,
        PoçoNãoEncontrado,
        CorrelaçãoInexistente,
        GrupoLitológicoNãoEncontrado
    }
}
