using System.Collections.Generic;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Factory
{
    public interface IPerfisEntradaFactory
    {
        PerfisEntrada CreatePerfisEntrada(IList<PerfilBase> perfis);
    }
}
