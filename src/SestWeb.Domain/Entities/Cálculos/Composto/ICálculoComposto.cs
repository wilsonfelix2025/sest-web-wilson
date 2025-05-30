using System.Collections.Generic;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Domain.Entities.Cálculos.Composto
{
    public interface ICálculoComposto : ICálculo
    {
        IList<ICorrelação> ListaCorrelação { get; }
    }
}
