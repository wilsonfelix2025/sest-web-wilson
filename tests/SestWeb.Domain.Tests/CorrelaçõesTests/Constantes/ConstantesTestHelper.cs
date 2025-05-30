using System.Collections.Generic;
using SestWeb.Domain.Entities.Correlações.ConstantesCorrelação;

namespace SestWeb.Domain.Tests.CorrelaçõesTests.Constantes
{
    internal class ConstantesTestHelper : IConstantes
    {
        private readonly Entities.Correlações.ConstantesCorrelação.Constantes _constantes;
        private Dictionary<string, double> _variables;

        protected internal ConstantesTestHelper(string expressão)
        {
            _constantes = (Entities.Correlações.ConstantesCorrelação.Constantes)Entities.Correlações.ConstantesCorrelação.Constantes.GetFactory().CreateConstantes(expressão);
            _variables = _constantes._variables;
        }

        public bool AddConst(string name, double value)
        {
            if (_variables.ContainsKey(name))
                return false;

            _variables.Add(name, value);
            return true;
        }

        public void Clear()
        {
            _variables.Clear();
        }

        public List<string> Names => _constantes.Names;

        public bool Exists(string variable)
        {
            return _constantes.Exists(variable);
        }

        public bool Any()
        {
            return _constantes.Any();
        }

        public bool TryGetValue(string name, out double valor)
        {
            return _constantes.TryGetValue(name, out valor);
        }
    }
}
