using System;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.RemoverRelacionamentoCorrsPropMec
{
    public enum RemoverRelacionamentoCorrsPropMecStatus
    {
        RelacionamentoRemovido,
        RelacionamentoNãoRemovido,
        RelacionamentoNãoEncontrado,
        RelacionamentoSemPermissãoParaRemoção
    }
}
