using System;
using System.Globalization;

namespace SestWeb.Domain.Importadores.Shallow.Utils
{
    public static class StringUtils
    {
        /// <summary>
        /// Alias para checar se a linha é nula, vazia ou composta apenas de espaços.
        /// </summary>
        /// <param name="linha">A linha a ser lida.</param>
        /// <returns>True se a linha é nula, vazia ou composta apenas de espaços; false nos outros casos.</returns>
        public static bool HasContent(string linha)
        {
            return !string.IsNullOrEmpty(linha) && !string.IsNullOrWhiteSpace(linha);
        }

        /// <summary>
        /// Converte <paramref name="stringNumérica"/> para double com cultura invariante.
        /// </summary>
        /// <exception cref="ArgumentNullException">Quando <paramref name="stringNumérica"/> é nulo.</exception>
        /// <exception cref="FormatException">Quando <paramref name="stringNumérica"> é impossível de converter.</exception>
        /// <param name="stringNumérica">A string numérica a ser convertida para double.</param>
        /// <param name="casasDecimais">Quantidade de casas decimais a ser retornada.</param>
        /// <returns>Valor da string como um double.</returns>
        public static double ToDoubleInvariantCulture(string stringNumérica, int casasDecimais)
        {
            /*
             * Optou-se por não fazer uso de regex neste método para validar o formato das strings
             * porque consultas de regex são caras e podemos partir das premissas que:
             *
             * 1. Os dados do arquivo .las vão, na grande maioria dos casos, estar corretamente formatados;
             * 2. As informações que não estiverem apropriadamente formatadas serão pegas pelo catch do try abaixo.
             *
             * Logo, não há a necessidade de encarecer o processo de conversão de string para double com
             * a inserção de um regex que serã válido 99.9999% das vezes.
             */

            try
            {
                // Variável que receberá o valor a ser retornado
                double númeroDouble;

                if (stringNumérica == null)
                {
                    throw new ArgumentNullException("Impossível converter null para double.");
                }

                // Se a string numérica usa vírgula como separador decimal
                if (stringNumérica.Contains(","))
                {
                    // Substitui pelo padrão internacional, que é o ponto, e converte pra double
                    stringNumérica = stringNumérica.Replace(",", ".");
                }

                // Do contrário, tenta fazer o parse 
                númeroDouble = double.Parse(stringNumérica, CultureInfo.InvariantCulture);

                // Arredonda para o número de casas decimais fornecidas
                return Round(númeroDouble, casasDecimais);
            }
            catch(FormatException)
            {
                throw new FormatException($"Impossível converter para double. {stringNumérica} está em formato inconsistente.");
            }
        }

        /// <summary>
        /// Padronizador de precisão para doubles
        /// </summary>
        /// <param name="number">Número no formato double.</param>
        /// <param name="digits">Quantidade de dígitos.</param>
        /// <returns></returns>
        private static double Round(double number, int digits)
        {
            var ajust = 1;
            for (int i = 0; i < digits; i++)
            {
                ajust *= 10;
            }
            return (double)Math.Truncate((decimal)number * ajust) / ajust;
        }

        public static string FixarCasasDecimais(double número, int casasDecimais)
        {
            return string.Format($"{{0:N{casasDecimais}}}", número).Replace(".", "").Replace(',', '.');
        }
    }
}