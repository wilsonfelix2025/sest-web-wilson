using System;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos.ModeloPoroElastico
{
    public class PropriedadesElasticas
    {
        public PropriedadesElasticas(double young, double poisson, double ks, double kf, double porosidade)
        {
            this.Young = young;
            this.Poisson = poisson;
            this.Ks = ks;
            this.Kf = kf;


            this.K = young / (3 * (1 - 2 * poisson));
            this.G = young / (2 * (1 + poisson));
            this.Ku = GetKu(this.K, porosidade);
            this.Vu = (3 * this.Ku - 2 * this.G) / (2 * (3 * this.Ku + this.G));
            this.B = 1 - (porosidade * (this.K / this.Kf - this.K / this.Ks) / ((1 - this.K / this.Ks) + porosidade * (this.K / this.Kf - this.K / this.Ks)));
            this.RelacaoBiot = 1 - (this.K / this.Ks);
            this.Nabla = (this.RelacaoBiot * (1 - 2 * this.Poisson)) / (2 * (1 - this.Poisson));
        }

        private double GetKu(double k, double posodidade)
        {
            var ks = this.Ks;
            var kf = this.Kf;

            return k * (1 + (Math.Pow((1 - (k / ks)), 2) / ((k / ks) * (1 - (k / ks)) + posodidade * ((k / kf) - (k / ks)))));
        }

        public double Young { get; }
        public double Poisson { get; }
        public double Ks { get; }
        public double Kf { get; }

        public double K { get; }
        public double G { get; }
        public double Ku { get; }
        public double Vu { get; }
        public double B { get; }
        public double RelacaoBiot { get; }
        public double Nabla { get; }
    }
}
