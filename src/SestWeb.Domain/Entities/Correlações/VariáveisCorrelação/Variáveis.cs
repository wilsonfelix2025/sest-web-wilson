using System;
using System.Globalization;
using System.Text.RegularExpressions;
using SestWeb.Domain.Entities.Correlações.VariablesCorrelação;
using SestWeb.Domain.Entities.Correlações.VariáveisCorrelação.Factory;

namespace SestWeb.Domain.Entities.Correlações.VariáveisCorrelação
{
    public class Variáveis : Variables, IVariáveis
    {
        private Variáveis(string expressão) : base(expressão) { }

        #region Methods

        private protected override void Identify(string expressão) 
        {
            string pattern = @"\bvar\b\s*(?<Nome>\b[a-zA-Z]\w*)\s*=\s*(?<Valor>\d+\.?\d*)";

            Regex regex = new Regex(pattern);

            foreach (Match match in regex.Matches(expressão))
            {
                string name = match.Groups["Nome"].Value;
                string value = match.Groups["Valor"].Value;

                if (!double.TryParse(value, NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out double valor))
                {
                    throw new ArgumentException($"Variável: {name} não possui valor: {value} em formato numérico!");
                }

                if (!_variables.ContainsKey(name))
                    _variables.Add(name, valor);
            }
        }

        public static IVariáveisFactory GetFactory()
        {
            return new VariáveisFactory(expressão => new Variáveis(expressão));
        }

        #endregion
    }
}
