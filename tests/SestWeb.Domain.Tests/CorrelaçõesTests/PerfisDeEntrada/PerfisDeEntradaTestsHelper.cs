using System.Collections.Generic;
using SestWeb.Domain.Entities.Correlações.PerfisEntradaCorrelação;

namespace SestWeb.Domain.Tests.CorrelaçõesTests.PerfisDeEntrada
{
    public class PerfisEntradaTestsHelper : IPerfisEntrada
    {
        private readonly PerfisEntrada _perfisEntrada;
        public List<string> Tipos => _perfisEntrada.Tipos;

        public PerfisEntradaTestsHelper(string expressão)
        {
            _perfisEntrada = (PerfisEntrada)PerfisEntrada.GetFactory().CreatePerfisEntrada(expressão);
        }

        public bool Add(string tipo)
        {
            if (Tipos.Contains(tipo))
                return false;

            Tipos.Add(tipo);
            return true;
        }

        public void Clear()
        {
            Tipos.Clear();
        }
    }
}
