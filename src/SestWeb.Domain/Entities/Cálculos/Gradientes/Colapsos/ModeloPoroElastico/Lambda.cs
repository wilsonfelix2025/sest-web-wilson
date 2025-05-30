using System;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos.ModeloPoroElastico
{
    public static class Lambda
    {
        public static double Calcular(double tempo, double cf)
        {
            return Math.Sqrt(tempo / cf);
        }
    }
}
