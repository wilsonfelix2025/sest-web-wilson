using System;
using System.Collections.Generic;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída.Factory
{
    public class PerfisSaídaFactory : IPerfisSaídaFactory
    {
        private readonly Func<IList<PerfilBase>, IPerfisSaída> _ctorCaller;

        public PerfisSaídaFactory(Func<IList<PerfilBase>, IPerfisSaída> ctorCaller)
        {
            _ctorCaller = ctorCaller;
        }

        public IPerfisSaída CreatePerfisSaída(IList<PerfilBase> perfis)
        {
            return _ctorCaller(perfis);
        }
    }
}
