using System;
using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Domain.Entities.Cálculos.Composto.Correlação
{
    public class MontadorDeExpressões : IMontadorDeExpressões
    {
        private readonly IList<ICorrelação> _correlações;
        private readonly string _separador;

        public MontadorDeExpressões(IList<ICorrelação> correlações, string separador)
        {
            _correlações = correlações;
            _separador = separador;
        }

        public string MontarExpressão()
        {
            string expressão = string.Empty;

            foreach (var correlação in _correlações)
            {
                if (correlação.PerfisEntrada.Tipos.Contains("RHOB") && !correlação.PerfisSaída.Tipos.Contains("RHOB"))
                    expressão += Environment.NewLine + correlação.Expressão.Bruta.Replace("RHOB", "RHOBUSER");
                else
                    expressão += Environment.NewLine + correlação.Expressão;

                if (correlação != _correlações.Last())
                    expressão += _separador;
            }

            return expressão;
        }
    }
}
