using System;
using System.Collections.Generic;

namespace SestWeb.Domain.Helpers
{
    public static class FastMath
    {
        private static readonly double[] RoundLookup = CreateRoundLookup();

        private static double[] CreateRoundLookup()
        {
            double[] result = new double[15];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Math.Pow(10, i);
            }

            return result;
        }

        public static double Round(double value)
        {
            return Math.Floor(value + 0.5);
        }

        public static double Round(double value, int decimalPlaces)
        {
            double adjustment = RoundLookup[decimalPlaces];
            return Math.Floor(value * adjustment + 0.5) / adjustment;
        }

        /// <summary>
        /// Verifica se dois doubles são iguais até uma certa casa decimal.
        /// </summary>
        /// <remarks>
        /// Devido à forma como doubles são armazenados em memória, a melhor forma de comparar os números é
        /// garantir que o valor absoluto da diferença entre eles é menor do que um erro máximo permitido.
        /// </remarks>
        /// <param name="primeiroValor">O primeiro valor a ser comparado.</param>
        /// <param name="segundoValor">O segundo valor a ser comparado.</param>
        /// <param name="erro">O valor máximo aceitável de diferença entre os dois números para que eles sejam considerados "iguais".</param>
        /// <returns></returns>
        public static bool SafeEquals(double primeiroValor, double segundoValor, double erro)
        {
            return Math.Abs(primeiroValor - segundoValor) < erro;
        }

        public static bool SafeLessThan(double primeiroValor, double segundoValor, double erro)
        {
            return primeiroValor - segundoValor < (-erro);
        }

        public static bool SafeGreaterThan(double primeiroValor, double segundoValor, double erro)
        {
            return primeiroValor - segundoValor > erro;
        }


        /// <summary>
        /// Fits a line to a collection of (x,y) points.
        /// Code from: https://gist.github.com/tansey/1375526
        /// </summary>
        /// <param name="xVals">The x-axis values.</param>
        /// <param name="yVals">The y-axis values.</param>
        /// <param name="rSquared">The r^2 value of the line.</param>
        /// <param name="yIntercept">The y-intercept value of the line (i.e. y = ax + b, yIntercept is b).</param>
        /// <param name="slope">The slop of the line (i.e. y = ax + b, slope is a).</param>
        public static void LinearRegression(List<double> xVals, List<double> yVals, out double rSquared, out double yIntercept, out double slope, out List<double> residuals, out List<double> newValues)
        {
            if (xVals.Count != yVals.Count)
            {
                throw new Exception("Quantidade de valores x e y devem ser iguais.");
            }

            double sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double sumOfYSq = 0;
            double sumCodeviates = 0;

            for (var i = 0; i < xVals.Count; i++)
            {
                var x = xVals[i];
                var y = yVals[i];
                sumCodeviates += x * y;
                sumOfX += x;
                sumOfY += y;
                sumOfXSq += x * x;
                sumOfYSq += y * y;
            }

            var count = xVals.Count;
            var ssX = sumOfXSq - ((sumOfX * sumOfX) / count);

            var rNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
            var rDenom = (count * sumOfXSq - (sumOfX * sumOfX)) * (count * sumOfYSq - (sumOfY * sumOfY));
            var sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

            var meanX = sumOfX / count;
            var meanY = sumOfY / count;
            var dblR = rNumerator / Math.Sqrt(rDenom);

            rSquared = dblR * dblR;
            yIntercept = meanY - ((sCo / ssX) * meanX);
            slope = sCo / ssX;

            //var a0 = (sumOfYSq * sumOfX - sumOfY * sumCodeviates) / (count * sumOfYSq - sumOfY * sumOfY);
            //var a1 = (count * sumCodeviates - sumOfY * sumOfX) / (count * sumOfYSq - sumOfY * sumOfY);

            //// cálculo dos pontos
            //for (var i = 0; i < xVals.Count; i++)
            //{
            //    var valorCalculado = a0 + a1 * xVals[i];
            //    newValues.Add(valorCalculado);
            //}


            residuals = new List<double>();
            newValues = new List<double>();
            for (var i = 0; i < xVals.Count; i++)
            {
                var prediction = CalculatePrediction(xVals[i], slope, yIntercept);
                var residual = xVals[i] - prediction;
                residuals.Add(residual);
                newValues.Add(prediction + residual);
            }
        }

        private static double CalculatePrediction(double input, double slope, double yIntercept)
        {
            return (input * slope) + yIntercept;
        }

    }
}