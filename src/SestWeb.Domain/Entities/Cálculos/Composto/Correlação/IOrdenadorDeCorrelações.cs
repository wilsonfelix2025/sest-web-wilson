using SestWeb.Domain.Entities.Correlações.Base;
using System.Collections.Generic;

namespace SestWeb.Domain.Entities.Cálculos.Composto.Correlação
{
    public interface IOrdenadorDeCorrelações
    {
        IList<ICorrelação> OrdenarCorrelações();
    }
}