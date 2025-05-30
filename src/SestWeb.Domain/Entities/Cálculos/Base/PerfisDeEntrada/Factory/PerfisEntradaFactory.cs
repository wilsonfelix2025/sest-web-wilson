using System;
using System.Collections.Generic;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada.Factory
{
    public class PerfisEntradaFactory : IPerfisEntradaFactory
    {
        private readonly Func<IList<PerfilBase>, PerfisEntrada> _ctorCaller;

        public PerfisEntradaFactory(Func<IList<PerfilBase>, PerfisEntrada> ctorCaller)
        {
            _ctorCaller = ctorCaller;
        }

        public PerfisEntrada CreatePerfisEntrada(IList<PerfilBase> perfis)
        {
            return _ctorCaller(perfis);
        }
    }
}
