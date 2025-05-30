using System;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Tensões;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.FatorPlastificacao
{
    public class FatorDePlastificacaoPorTipoCriterio
    {
        public static double ConvertDegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }

        public double CalcularPorDPInterno(double angat, double coesao, TensoesPrincipais tensoesPrincipais)
        {
            var tensaoEfetiva1 = tensoesPrincipais.TensaoPrincipalEfetiva1;
            var tensaoEfetiva2 = tensoesPrincipais.TensaoPrincipalEfetiva2;
            var tensaoEfetiva3 = tensoesPrincipais.TensaoPrincipalEfetiva3;
            var tensaoPrincipal1 = tensoesPrincipais.TensaoPrincipal1;
            var tensaoPrincipal2 = tensoesPrincipais.TensaoPrincipal2;
            var tensaoPrincipal3 = tensoesPrincipais.TensaoPrincipal3;

            var tensaoPrincipalPorCriterioRuptura = (1d / 3d) * Math.Sqrt(Math.Pow(tensaoPrincipal1 - tensaoPrincipal2, 2) + Math.Pow(tensaoPrincipal2 - tensaoPrincipal3, 2) + Math.Pow(tensaoPrincipal3 - tensaoPrincipal1, 2));
            var tensaoEfetivaPorCriterioRuptura = (1d / 3d) * (tensaoEfetiva1 + tensaoEfetiva2 + tensaoEfetiva3);

            var m = (Math.Sqrt(6) * Math.Sin(ConvertDegreesToRadians(angat))) / Math.Sqrt(9 + 3 * Math.Pow(Math.Sin(ConvertDegreesToRadians(angat)), 2));

            var tensao = (Math.Sqrt(6) * coesao * Math.Cos(ConvertDegreesToRadians(angat))) / Math.Sqrt(9 + 3 * Math.Pow(Math.Sin(ConvertDegreesToRadians(angat)), 2));

            var fp = tensaoPrincipalPorCriterioRuptura / (tensao + m * tensaoEfetivaPorCriterioRuptura);

            return fp;
        }

        public double CalcularPorDPCentral(double angat, double coesao, TensoesPrincipais tensoesPrincipais)
        {
            var tensaoEfetiva1 = tensoesPrincipais.TensaoPrincipalEfetiva1;
            var tensaoEfetiva2 = tensoesPrincipais.TensaoPrincipalEfetiva2;
            var tensaoEfetiva3 = tensoesPrincipais.TensaoPrincipalEfetiva3;
            var tensaoPrincipal1 = tensoesPrincipais.TensaoPrincipal1;
            var tensaoPrincipal2 = tensoesPrincipais.TensaoPrincipal2;
            var tensaoPrincipal3 = tensoesPrincipais.TensaoPrincipal3;

            var tensaoPrincipalPorCriterioRuptura = (1d / 3d) * Math.Sqrt(Math.Pow(tensaoPrincipal1 - tensaoPrincipal2, 2) + Math.Pow(tensaoPrincipal2 - tensaoPrincipal3, 2) + Math.Pow(tensaoPrincipal3 - tensaoPrincipal1, 2));
            var tensaoEfetivaPorCriterioRuptura = (1d / 3d) * (tensaoEfetiva1 + tensaoEfetiva2 + tensaoEfetiva3);

            var m = (2 * Math.Sqrt(2) * Math.Sin(ConvertDegreesToRadians(angat))) / (3 + Math.Sin(ConvertDegreesToRadians(angat)));

            var tensao = (2 * Math.Sqrt(2) * coesao * Math.Cos(ConvertDegreesToRadians(angat))) / (3 + Math.Cos(ConvertDegreesToRadians(angat)));

            var fp = tensaoPrincipalPorCriterioRuptura / (tensao + m * tensaoEfetivaPorCriterioRuptura);

            return fp;
        }

        public double CalcularPorDPExterno(double angat, double coesao, TensoesPrincipais tensoesPrincipais)
        {
            var tensaoEfetiva1 = tensoesPrincipais.TensaoPrincipalEfetiva1;
            var tensaoEfetiva2 = tensoesPrincipais.TensaoPrincipalEfetiva2;
            var tensaoEfetiva3 = tensoesPrincipais.TensaoPrincipalEfetiva3;
            var tensaoPrincipal1 = tensoesPrincipais.TensaoPrincipal1;
            var tensaoPrincipal2 = tensoesPrincipais.TensaoPrincipal2;
            var tensaoPrincipal3 = tensoesPrincipais.TensaoPrincipal3;

            var tensaoPrincipalPorCriterioRuptura = (1d / 3d) * Math.Sqrt(Math.Pow(tensaoPrincipal1 - tensaoPrincipal2, 2) + Math.Pow(tensaoPrincipal2 - tensaoPrincipal3, 2) + Math.Pow(tensaoPrincipal3 - tensaoPrincipal1, 2));
            var tensaoEfetivaPorCriterioRuptura = (1d / 3d) * (tensaoEfetiva1 + tensaoEfetiva2 + tensaoEfetiva3);

            var m = (2 * Math.Sqrt(2) * Math.Sin(ConvertDegreesToRadians(angat))) / (3 - Math.Sin(ConvertDegreesToRadians(angat)));

            var tensao = (2 * Math.Sqrt(2) * coesao * Math.Cos(ConvertDegreesToRadians(angat))) / (3 - Math.Cos(ConvertDegreesToRadians(angat)));

            var fp = tensaoPrincipalPorCriterioRuptura / (tensao + m * tensaoEfetivaPorCriterioRuptura);

            return fp;
        }

        public double CalcularPorLade(double angat, double coesao, TensoesPrincipais tensoesPrincipais)
        {
            var tensaoEfetiva1 = tensoesPrincipais.TensaoPrincipalEfetiva1;
            var tensaoEfetiva2 = tensoesPrincipais.TensaoPrincipalEfetiva2;
            var tensaoEfetiva3 = tensoesPrincipais.TensaoPrincipalEfetiva3;

            var S1 = coesao / Math.Tan(ConvertDegreesToRadians(angat));
            var I1 = tensaoEfetiva1 + tensaoEfetiva2 + tensaoEfetiva3 + 3 * S1;
            var I3 = (tensaoEfetiva1 + S1) * (tensaoEfetiva2 + S1) * (tensaoEfetiva3 + S1);
            var nSup = 4 * Math.Pow(Math.Tan(ConvertDegreesToRadians(angat)), 2) * (9 - 7 * Math.Sin(ConvertDegreesToRadians(angat)));
            var nDen = 1 - Math.Sin(ConvertDegreesToRadians(angat));
            var n = nSup / nDen;
            var fp = (Math.Pow(I1, 3) / I3) / (27 + n);

            return fp;
        }

        public double CalcularPorMC(double ucs, double angat, double restr, TensoesPrincipais tensoesPrincipais)
        {
            var tensaoEfetiva1 = tensoesPrincipais.TensaoPrincipalEfetiva1;
            var tensaoEfetiva3 = tensoesPrincipais.TensaoPrincipalEfetiva3;

            var tensaoPorCritarioRuptura = ucs + tensaoEfetiva3 * Math.Pow(Math.Tan(ConvertDegreesToRadians(45 + (angat / 2))), 2);

            if (tensaoEfetiva3 < 0)
            {
                var t2 = Math.Pow(Math.Tan(ConvertDegreesToRadians(45 + angat / 2)), 2);
                var S3min = ucs / t2;

                if (restr > S3min)
                    restr = S3min;

                if (Math.Abs(tensaoEfetiva3) < restr)
                {
                    if (tensaoPorCritarioRuptura > 0)
                    {
                        return tensaoEfetiva1 / tensaoPorCritarioRuptura;
                    }

                    return 1;
                }

                //return 2;
                return tensaoEfetiva1 / tensaoPorCritarioRuptura;
            }

            return tensaoEfetiva1 / tensaoPorCritarioRuptura;
        }
    }
}
