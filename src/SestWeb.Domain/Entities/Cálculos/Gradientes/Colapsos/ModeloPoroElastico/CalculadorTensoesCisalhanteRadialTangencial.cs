using System;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Util;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos.ModeloPoroElastico
{
    public class CalculadorTensoesCisalhanteRadialTangencial
    {
        private readonly RaioPoco raioPoco;
        private readonly PropriedadesElasticas propElasticas;
        private readonly MatrizTensorDeTensoes matrizTensorDeTensoes;

        private readonly double cf;
        public CalculadorTensoesCisalhanteRadialTangencial(RaioPoco raioPoco, double cf, PropriedadesElasticas propElasticas, MatrizTensorDeTensoes matrizTensorDeTensoes)
        {
            this.cf = cf;
            this.matrizTensorDeTensoes = matrizTensorDeTensoes;
            this.propElasticas = propElasticas;
            this.raioPoco = raioPoco;
        }

        public TensoesCisalhanteRadialTangencial CalcularTensoesCisalhanteRadialTangencial(double tempo, double raio, double teta)
        {
            var cisalhanteRadialTangencial3 = Laplace.InverseTransform(this.GetFuncCalcularCisalhanteRadialTangencial3(raio, teta), tempo);
            var cisalhanteRadialTangencialI = CalcularCisalhanteRadialTangencialI(cisalhanteRadialTangencial3, teta);

            return new TensoesCisalhanteRadialTangencial(cisalhanteRadialTangencial3, cisalhanteRadialTangencialI);
        }

        private Laplace.FunctionDelegate GetFuncCalcularCisalhanteRadialTangencial3(double raio, double teta)
        {
            double func(double tempo)
            {
                return CalcularCisalhanteRadialTangencial3(tempo, raio, teta);
            }
            return func;
        }

        private double CalcularCisalhanteRadialTangencial3(double tempo, double raio, double teta)
        {
            var lambda = Lambda.Calcular(tempo, cf);

            var constantesAritmeticas = new ConstantesAritmeticas(lambda, this.propElasticas, this.raioPoco);
            var constantesDeBessel = new ConstantesDeBessel(lambda, raio, this.raioPoco);

            var B = propElasticas.B;
            var Vu = propElasticas.Vu;

            var Ssh = matrizTensorDeTensoes.Ssh;

            var C1 = constantesAritmeticas.C1;
            var C2 = constantesAritmeticas.C2;
            var C3 = constantesAritmeticas.C3;

            var k1LambdaRaio = constantesDeBessel.K1LambdaRaio;
            var k2LambdaRaio = constantesDeBessel.K2LambdaRaio;


            var parcela1 = Ssh * Math.Sin(2 * teta);
            var parcela2 = ((((2 * B * (1 + Vu) * C1) / (3 * (1 - Vu)) * (k1LambdaRaio / (lambda * raio) + (3 * k2LambdaRaio) / (Math.Pow(lambda * raio, 2)))) - ((C2 * Math.Pow(raioPoco.Value, 2)) / (Math.Pow(raio, 2) * (2 * (1 - Vu)))) - ((3 * C3 * Math.Pow(raioPoco.Value, 4)) / (Math.Pow(raio, 4)))));
            return parcela1 * parcela2 / tempo;
        }

        private double CalcularCisalhanteRadialTangencialI(double cisalhanteRadialTangencial3, double teta)
        {
            var Ssh = matrizTensorDeTensoes.Ssh;
            return -Ssh * Math.Sin(2 * teta) + cisalhanteRadialTangencial3;
        }
    }
}
