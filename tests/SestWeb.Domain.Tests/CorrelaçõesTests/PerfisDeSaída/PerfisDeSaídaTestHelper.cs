using System;
using System.Collections.Generic;
using System.Reflection;
using SestWeb.Domain.Entities.Correlações.PerfisSaídaCorrelação;

namespace SestWeb.Domain.Tests.CorrelaçõesTests.PerfisDeSaída
{
    public class PerfisSaídaTestsHelper : IPerfisSaída
    {
        private readonly PerfisSaída _perfisSaída;

        protected internal PerfisSaídaTestsHelper(string expressão)
        {
            _perfisSaída = (PerfisSaída)PerfisSaída.GetFactory().CreatePerfisSaída(expressão);
        }

        public bool Add(string tipo)
        {
            if (_perfisSaída.Tipos.Contains(tipo))
                return false;

            _perfisSaída.Tipos.Add(tipo);
            return true;
        }

        public void Clear()
        {
            _perfisSaída.Tipos.Clear();
        }

        public List<string> Tipos => _perfisSaída.Tipos;
    }
}
