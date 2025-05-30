using System;

namespace SestWeb.Domain.Helpers
{
    public static class UnitConverter
    {
        public static double PsiToPascal(double psi)
        {
            return psi * 6894.757;
        }

        public static double PsiToPpg(double psi, double pv)
        {
            return psi / (pv * 0.1704);
        }

        public static double PascalToPsi(double psi)
        {
            return psi / 6894.757;
        }

        public static double CelciusToKelvin(double temp)
        {
            return temp + 273.15;
        }

        public static double PolToMeter(double pol)
        {
            return pol / 39.37;
        }
        public static double MeterToPol(double meter)
        {
            return meter * 39.37;
        }

        public static double mDToM2(double value)
        {
            return value * 9.86923e-16;
        }


        public static double cPToPa_s(double value)
        {
            return value * 0.001;
        }

        public static double GrausParaRadiando(double graus)
        {
            return graus * Math.PI / 180;
        }
        public static double RadiandoParaGraus(double graus)
        {
            return graus / (Math.PI / 180);
        }
    }
}
