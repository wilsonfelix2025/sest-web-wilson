using System.Collections.Generic;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Factory
{
    public interface IPerfisSaídaFactory
    {
        IPerfisSaída CreatePerfisSaída(IList<PerfilBase> perfis);
    }
}
