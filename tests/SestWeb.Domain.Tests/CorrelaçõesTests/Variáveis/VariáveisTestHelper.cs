using System;
using System.Collections.Generic;
using System.Reflection;
using SestWeb.Domain.Entities.Correlações.VariablesCorrelação;
using SestWeb.Domain.Entities.Correlações.VariáveisCorrelação;
using SestWeb.Domain.Entities.Correlações.VariáveisCorrelação.Factory;
using SestWeb.Domain.Tests.Helpers;

namespace SestWeb.Domain.Tests.CorrelaçõesTests.Variáveis
{
    internal class VariáveisTestHelper : IVariáveis
    {
        private readonly Entities.Correlações.VariáveisCorrelação.Variáveis _variáveis;
        private Dictionary<string, double> _variables;

        protected internal VariáveisTestHelper(string expressão)
        {
            var variáveisFactory = Entities.Correlações.VariáveisCorrelação.Variáveis.GetFactory();
            _variáveis = (Entities.Correlações.VariáveisCorrelação.Variáveis)variáveisFactory.CreateVariáveis(expressão);
            _variables = _variáveis._variables;
        }

        public List<string> Names => _variáveis.Names;

        public bool AddVar(string name, double value)
        {
            if (_variables.ContainsKey(name))
                return false;

            _variables.Add(name, value);
            return true;
        }

        public bool Any()
        {
            return _variáveis.Any();
        }

        public void Clear()
        {
            _variables.Clear();
        }

        public bool Exists(string variable)
        {
            return _variáveis.Exists(variable);
        }

        public bool TryGetValue(string name, out double valor)
        {
            return _variáveis.TryGetValue(name, out valor);
        }
    }
}
