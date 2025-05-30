using System;
using System.Collections.Generic;

namespace SestWeb.Domain.Importadores.Utils
{
    public class ReferênciaParaImportação
    {
        // Lista de números que representam que não foi computado ou inserido um valor numa curva.
        private static readonly List<double> AusênciaDeValor = new List<double> {-999.25, -9999, -99999, -9999.99, -999};

        // Constante de precisão para comparações envolvendo doubles.
        private const double TolerânciaDeComparação = 0.000000001;

        public static bool ÉSemValor(double valor)
        {
            // TODO usar o método FastMath para comparar double
            return double.IsNaN(valor) || AusênciaDeValor.Exists(v => Math.Abs(valor - v) < TolerânciaDeComparação);
        }
    }
}
