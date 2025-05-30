using System;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.RemoverRelacionamentoCorrsPropMecPoço
{
    public enum RemoverRelacionamentoCorrsPropMecStatusPoço
    {
        RelacionamentoRemovido,
        RelacionamentoNãoRemovido,
        RelacionamentoNãoEncontrado,
        RelacionamentoSemPermissãoParaRemoção,
        PoçoNãoEncontrado
    }
}
