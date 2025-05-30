using MathNet.Numerics;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos.ModeloPoroElastico
{
    public class ConstantesAritmeticas
    {
        public ConstantesAritmeticas(double lambda, PropriedadesElasticas propElasticas, RaioPoco raioPoco)
        {

            var Vu = propElasticas.Vu;
            var poisson = propElasticas.Poisson;
            var B = propElasticas.B;

            var k1LambdaA = SpecialFunctions.BesselK1(lambda * raioPoco.Value);
            var k2LambdaA = SpecialFunctions.BesselK(2, lambda * raioPoco.Value);

            this.D1 = 2 * (Vu - poisson) * k1LambdaA;
            this.D2 = (lambda * raioPoco.Value) * (1 - poisson) * k2LambdaA;
            this.C1 = -((12 * lambda * raioPoco.Value * (1 - Vu) * (Vu - poisson)) / (B * (1 + Vu) * (this.D2 - this.D1)));
            this.C2 = (4 * (1 - Vu) * this.D2) / (this.D2 - this.D1);
            this.C3 = -((lambda * raioPoco.Value) * (this.D2 + this.D1) + 8 * (Vu - poisson) * k2LambdaA) / (lambda * raioPoco.Value * (this.D2 - this.D1));
        }

        public double D1 { get; }
        public double D2 { get; }
        public double C1 { get; }
        public double C2 { get; }
        public double C3 { get; }
    }
}
