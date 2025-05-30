using System;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.RemoverCorrelaçãoPoço
{
    public enum RemoverCorrelaçãoPoçoStatus
    {
        CorrelaçãoRemovida,
        CorrelaçãoNãoRemovida,
        CorrelaçãoNãoEncontrada,
        CorrelaçãoSemPermissãoParaRemoção,
        PoçoNãoEncontrado
    }
}
