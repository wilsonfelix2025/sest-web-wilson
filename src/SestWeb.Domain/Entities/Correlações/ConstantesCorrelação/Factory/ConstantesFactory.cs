using System;

namespace SestWeb.Domain.Entities.Correlações.ConstantesCorrelação.Factory
{
    public class ConstantesFactory : IConstantesFactory
    {
        private readonly Func<string, IConstantes> _ctorCaller;

        public ConstantesFactory(Func<string, IConstantes> ctorCaller)
        {
            _ctorCaller = ctorCaller;
        }

        public IConstantes CreateConstantes(string expressão)
        {
            return _ctorCaller(expressão);
        }
    }
}
