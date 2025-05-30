using System;
using System.Collections.Generic;
using System.Linq;

namespace SestWeb.Domain.Entities.Correlações.PerfisEntradaCorrelação.Factory
{
    public class PerfisEntradaFactory : IPerfisEntradaFactory
    {
        private readonly Func<List<string>, IPerfisEntrada> _ctorCaller;

        public PerfisEntradaFactory(Func<List<string>, IPerfisEntrada> ctorCaller)
        {
            _ctorCaller = ctorCaller;
        }

        public IPerfisEntrada CreatePerfisEntrada(string expressão)
        {
            return _ctorCaller(PerfisEntrada.Identify(expressão).ToList());
        }
    }
}
