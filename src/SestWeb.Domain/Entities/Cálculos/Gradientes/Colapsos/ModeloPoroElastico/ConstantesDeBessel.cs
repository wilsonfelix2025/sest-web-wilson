using MathNet.Numerics;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos.ModeloPoroElastico
{
    public class ConstantesDeBessel
    {

        public double K0LambdaRaio { get; }
        public double K0LambdaRaioPoco { get; }

        public double K1LambdaRaio { get; }
        public double K1LambdaRaioPoco { get; }

        public double K2LambdaRaio { get; set; }
        public double K2LambdaRaioPoco { get; set; }

        public ConstantesDeBessel(double lambda, double raio, RaioPoco raioPoco)
        {
            var lambdaRaio = lambda * raio;
            var lambdaRaioPoco = lambda * raioPoco.Value;

            this.K0LambdaRaio = SpecialFunctions.BesselK0(lambdaRaio);
            this.K1LambdaRaio = SpecialFunctions.BesselK1(lambdaRaio);
            this.K2LambdaRaio = SpecialFunctions.BesselK(2, lambdaRaio);

            this.K0LambdaRaioPoco = SpecialFunctions.BesselK0(lambdaRaioPoco);
            this.K1LambdaRaioPoco = SpecialFunctions.BesselK1(lambdaRaioPoco);
            this.K2LambdaRaioPoco = SpecialFunctions.BesselK(2, lambdaRaioPoco);
        }
    }
}
