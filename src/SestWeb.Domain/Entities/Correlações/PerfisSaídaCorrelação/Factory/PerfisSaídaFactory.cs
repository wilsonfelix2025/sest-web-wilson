using System;
using System.Collections.Generic;
using System.Linq;

namespace SestWeb.Domain.Entities.Correlações.PerfisSaídaCorrelação.Factory
{
    public class PerfisSaídaFactory : IPerfisSaídaFactory
    {
        private readonly Func<List<string>, IPerfisSaída> _ctorCaller;

        public PerfisSaídaFactory(Func<List<string>, IPerfisSaída> ctorCaller)
        {
            _ctorCaller = ctorCaller;
        }

        public IPerfisSaída CreatePerfisSaída(string expressão)
        {
            return _ctorCaller(PerfisSaída.Identify(expressão).ToList());
        }
    }
}
