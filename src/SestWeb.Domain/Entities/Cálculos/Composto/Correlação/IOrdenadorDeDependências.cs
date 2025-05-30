using System.Collections.Generic;

namespace SestWeb.Domain.Entities.Cálculos.Composto.Correlação
{
    internal interface IOrdenadorDeDependências
    {
        List<string> OrdenarDependências();
    }
}