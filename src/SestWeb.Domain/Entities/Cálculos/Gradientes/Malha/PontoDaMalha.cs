using SestWeb.Domain.Entities.Cálculos.Gradientes.Tensões;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Malha
{
    public class PontoDaMalha
    {

        public PontoDaMalha(double x, double y, double angulo, double raio, ConstantesDeRaio constantesDeRaio, ConstantesDeAngulo constantesDeAngulo)
        {
            this.X = x;
            this.Y = y;
            this.Raio = UnitConverter.PolToMeter(raio);
            this.Angulo = angulo;
            this.ConstantesDeAngulos = constantesDeAngulo;
            this.ConstantesDeRaio = constantesDeRaio;
        }

        public double X { get; }
        public double Y { get; }
        public double FatorPlastificação { get; set; }
        public double Raio { get; }
        /// <summary>
        /// Ângulo em radianos
        /// </summary>
        public double Angulo { get; }
        public ConstantesDeAngulo ConstantesDeAngulos { get; }
        public ConstantesDeRaio ConstantesDeRaio { get; }

        public ITensoesAoRedorPoco TensoesAoRedorPoco { get; set; }
        public TensoesPrincipais TensoesPrincipais { get; set; }
    }
}
