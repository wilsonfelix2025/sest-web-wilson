using System;

namespace SestWeb.Domain.Entities.Correlações.VariáveisCorrelação.Factory
{
    public class VariáveisFactory : IVariáveisFactory
    {
        private readonly Func<string, IVariáveis> _ctorCaller;

        public VariáveisFactory(Func<string, IVariáveis> ctorCaller)
        {
            _ctorCaller = ctorCaller;
        }

        public IVariáveis CreateVariáveis(string expressão)
        {
            return _ctorCaller(expressão);
        }
    }
}
