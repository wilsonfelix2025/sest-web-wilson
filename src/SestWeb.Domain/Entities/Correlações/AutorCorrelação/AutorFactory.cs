using System;

namespace SestWeb.Domain.Entities.Correlações.AutorCorrelação
{
    public class AutorFactory : IAutorFactory
    {
        private Func<string, string, DateTime, IAutor> _ctorCaller;

        public AutorFactory(Func<string, string, DateTime, IAutor> ctorCaller)
        {
            _ctorCaller = ctorCaller;
        }

        public IAutor CreateAutor(string nome, string chave, DateTime dataCriação)
        {
            if (IsValid())
                return _ctorCaller(nome, chave, dataCriação);
            else
                return null;
        }

        private bool IsValid()
        {
            // validado na criação da correlação
            return true;
        }
    }
}
