using System;

namespace SestWeb.Domain.Entities.Cálculos.Tensões
{
    public static class CoeficienteAngular
    {
        public static double CalcularCoeficienteAngularPassandoPelaOrigem(double[] x, double[] y)
        {
            if (x.Length != y.Length)
                throw new ArgumentException("Listas devem ter tamanhos iguais.");

            var xy = 0.0;
            var x2 = 0.0;

            for (var i = 0; i < x.Length; i++)
            {
                xy += x[i] * y[i];
                x2 += x[i] * x[i];
            }

            return xy / x2;
        }
    }
}
