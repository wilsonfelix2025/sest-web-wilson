using System;
using MathNet.Numerics;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Tensões;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Util;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos.ModeloPoroElastico
{
    public class CalculadorTensoesTangenciais
    {
        private readonly RaioPoco raioPoco;
        private readonly double pw;
        private readonly double cf;
        private readonly PropriedadesElasticas propElasticas;
        private readonly MatrizTensorDeTensoes matrizTensorDeTensoes;
        private readonly double pressaoPoros;
        private readonly DadosEntradaModeloPoroElastico dadosEntradaModeloPoroElastico;
        public CalculadorTensoesTangenciais(RaioPoco raioPoco, double pw, double cf, PropriedadesElasticas propElasticas, MatrizTensorDeTensoes matrizTensorDeTensoes, double pressaoPoros, DadosEntradaModeloPoroElastico dadosEntradaModeloPoroElastico)
        {
            this.dadosEntradaModeloPoroElastico = dadosEntradaModeloPoroElastico;
            this.pressaoPoros = pressaoPoros;
            this.matrizTensorDeTensoes = matrizTensorDeTensoes;
            this.propElasticas = propElasticas;
            this.cf = cf;
            this.pw = pw;
            this.raioPoco = raioPoco;
        }

        public TensoesTangenciais CalcularTensoesTangenciais(double tempo, PontoDaMalha pontoDaMalha)
        {
            var raio = pontoDaMalha.Raio;
            var angulo = pontoDaMalha.Angulo;

            var tangencial1 = this.CalcularTensaoTangencial1(raio);
            var tangencial2 = Laplace.InverseTransform(this.GetFuncCalcularTangencial2(raio), tempo);
            var tangencial3 = Laplace.InverseTransform(this.GetFuncCalcularTangencial3(raio, angulo), tempo);

            var tangencialI = this.CalcularTensaoTangencialI(tangencial1, tangencial2, tangencial3, angulo);

            return new TensoesTangenciais(tangencial1, tangencial2, tangencial3, tangencialI);
        }

        private double CalcularTensaoTangencial1(double raio)
        {
            var Psh = matrizTensorDeTensoes.Psh;

            return -(Psh - pw) * Math.Pow(raioPoco.Value / raio, 2);
        }

        private double CalcularTensaoTangencial2(double tempo, double raio)
        {
            var nabla = propElasticas.Nabla;
            var lambda = Lambda.Calcular(tempo, cf);

            var constBessel = new ConstantesDeBessel(lambda, raio, this.raioPoco);

            var k0LambdaRaio = constBessel.K0LambdaRaio;
            var k1LambdaRaio = constBessel.K1LambdaRaio;

            var k0LambdaRaioPoco = constBessel.K0LambdaRaioPoco;
            var k1LambdaRaioPoco = constBessel.K1LambdaRaioPoco;

            var parcela1 = 2 * nabla * (pw - pressaoPoros);
            var parcela2 = (k1LambdaRaio / (tempo * raio * lambda * k0LambdaRaioPoco)) - (raioPoco.Value * k1LambdaRaioPoco) / (tempo * raio * raio * lambda * k0LambdaRaioPoco) + k0LambdaRaio / (tempo * k0LambdaRaioPoco);

            var tangencial = -parcela1 * parcela2;

            if (dadosEntradaModeloPoroElastico.EfeitoTermico)
            {
                var difusidade = this.dadosEntradaModeloPoroElastico.DifusividadeTermica;
                var expensaoTermicaRocha = this.dadosEntradaModeloPoroElastico.ExpansaoTermicaRocha;
                var expansaoTermicaFluidoPoros = this.dadosEntradaModeloPoroElastico.ExpansaoTermicaFluidoPoros;
                var temperaturaPoco = this.dadosEntradaModeloPoroElastico.TemperaturaPoco;
                var temperaturaFormacao = this.dadosEntradaModeloPoroElastico.TemperaturaFormacao;

                var qt = Math.Sqrt(tempo / difusidade);
                var deltaT = temperaturaPoco - temperaturaFormacao;

                if (Math.Abs(deltaT) > 1e-5)
                {
                    var dx = (1 - cf / difusidade);
                    if (Math.Abs(dx) < 1e-7)
                    {
                        if (dx >= 0)
                        {
                            dx = 1e-7;
                        }
                        else
                        {
                            dx = -1e-7;
                        }
                    }


                    var viscosidade = this.dadosEntradaModeloPoroElastico.Viscosidade;
                    var permeabilidade = this.dadosEntradaModeloPoroElastico.Permeabilidade;
                    var porosidade = this.dadosEntradaModeloPoroElastico.Porosidade;
                    var biot = this.dadosEntradaModeloPoroElastico.Biot;
                    var poisson = this.dadosEntradaModeloPoroElastico.Poisson;

                    var cft = (cf * viscosidade / permeabilidade) * ((2 * biot * (1 - 2 * poisson) * expensaoTermicaRocha) / (3 * (1 - poisson)) - (expensaoTermicaRocha - expansaoTermicaFluidoPoros) * porosidade);

                    Func<double, double> k0 = SpecialFunctions.BesselK0;
                    Func<double, double> k1 = SpecialFunctions.BesselK1;

                    var G = this.propElasticas.G;

                    tangencial += (2 * nabla) * (cft * deltaT) / dx * 1 / tempo * ((k1LambdaRaio / (raio * lambda * k0LambdaRaioPoco)) - raioPoco.Value / (raio * raio) * k1(raioPoco.Value * lambda) / (lambda * k0(lambda * raioPoco.Value)) + k0(qt * raio) / k0(qt * raioPoco.Value));

                    tangencial += (-2 * nabla) * (cft * deltaT) / dx * 1 / tempo * (k1(qt * raio) / (raio * qt * k0(qt * raioPoco.Value)) - raioPoco.Value / (raio * raio) * k1(qt * raioPoco.Value) / (qt * k0(qt * raioPoco.Value)) + k0(qt * raio) / k0(qt * raioPoco.Value));

                    tangencial += (-2 * G * expensaoTermicaRocha * (1 + poisson)) / (3 * (1 - poisson)) * deltaT * 1 / tempo * (k1(qt * raio) / (raio * qt * k0(qt * raioPoco.Value)) - raioPoco.Value / (raio * raio) * k1(qt * raioPoco.Value) / (qt * k0(qt * raioPoco.Value)) + k0(qt * raio) / k0(qt * raioPoco.Value));
                }
            }

            if (this.dadosEntradaModeloPoroElastico.EfeitoFisicoQuimico)
            {
                var coefReflexao = this.dadosEntradaModeloPoroElastico.CoefReflexao;
                var coefDifusaoSoluto = this.dadosEntradaModeloPoroElastico.CoefDifusaoSoluto;
                var porosidade = this.dadosEntradaModeloPoroElastico.Porosidade;
                var massaMolarSoluto = this.dadosEntradaModeloPoroElastico.MassaMolarSoluto;
                var concentracaoSolutoFluidoPerfuracao = this.dadosEntradaModeloPoroElastico.ConcentracaoSolutoFluidoPerfuracao;
                var concentracaoSolutoFluidoRocha = this.dadosEntradaModeloPoroElastico.ConcentracaoSolutoFluidoRocha;
                var temperaturaFormacao = this.dadosEntradaModeloPoroElastico.TemperaturaFormacao_FisicoQuimico;
                var coefDissociacaoSoluto = this.dadosEntradaModeloPoroElastico.CoefDissociacaoSoluto;
                var coefInchamento = this.dadosEntradaModeloPoroElastico.CoefInchamento;
                var densidadeFluidoFormacao = this.dadosEntradaModeloPoroElastico.DensidadeFluidoFormacao;


                var difusidadeQuimicaEfetiva = (1 - coefReflexao) * coefDifusaoSoluto / porosidade;

                const double R = 8.314;
                var cfC = -R * temperaturaFormacao * coefReflexao / (1 - coefReflexao) * coefDissociacaoSoluto * cf * porosidade / (massaMolarSoluto * coefDifusaoSoluto);

                var qc = Math.Sqrt(tempo / difusidadeQuimicaEfetiva);
                var deltaC = concentracaoSolutoFluidoPerfuracao - concentracaoSolutoFluidoRocha;
                var exis = coefInchamento / densidadeFluidoFormacao;

                if (Math.Abs(deltaC) > 1e-5)
                {
                    var dx = (1 - cf / difusidadeQuimicaEfetiva);
                    if (Math.Abs(dx) < 1e-7)
                    {
                        if (dx >= 0)
                        {
                            dx = 1e-7;
                        }
                        else
                        {
                            dx = -1e-7;
                        }
                    }


                    var viscosidade = this.dadosEntradaModeloPoroElastico.Viscosidade;
                    var permeabilidade = this.dadosEntradaModeloPoroElastico.Permeabilidade;
                    var biot = this.dadosEntradaModeloPoroElastico.Biot;
                    var poisson = this.dadosEntradaModeloPoroElastico.Poisson;

                    Func<double, double> k0 = SpecialFunctions.BesselK0;
                    Func<double, double> k1 = SpecialFunctions.BesselK1;

                    var a = (raioPoco.Value * k1(qc * raioPoco.Value)) / (tempo * raio * raio * qc * k0(qc * raioPoco.Value));

                    var p1 = (2 * nabla) * (cfC * deltaC / dx) * (k1(lambda * raio) / (tempo * raio * lambda * k0(lambda * raioPoco.Value)) - raioPoco.Value * k1(lambda * raioPoco.Value) / (tempo * raio * raio * lambda * k0(lambda * raioPoco.Value)) + k0(lambda * raio) / (tempo * k0(lambda * raioPoco.Value)));
                    var p2 = (2 * nabla) * (-cfC * deltaC / dx) * (k1(qc * raio) / (tempo * raio * qc * k0(qc * raioPoco.Value)) - raioPoco.Value * k1(qc * raioPoco.Value) / (tempo * raio * raio * qc * k0(qc * raioPoco.Value)) + k0(qc * raio) / (tempo * k0(qc * raioPoco.Value)));
                    var p3 = (exis * (1 - 2 * poisson) / (1 - poisson)) * deltaC * (k1(qc * raio) / (tempo * raio * qc * k0(qc * raioPoco.Value)) - raioPoco.Value * k1(qc * raioPoco.Value) / (tempo * raio * raio * qc * k0(qc * raioPoco.Value)) + k0(qc * raio) / (tempo * k0(qc * raioPoco.Value)));

                    //tangencial += (2 * nabla) * (-cfC * deltaC / dx) * (k1(lambda * raio) / (tempo * raio * lambda * k0(lambda * raioPoco.Value)) - raioPoco.Value * k1(lambda * raioPoco.Value) / (tempo * raio * raio * lambda * k0(lambda * raioPoco.Value)) + k0(lambda * raio) / (tempo * k0(lambda * raioPoco.Value)));
                    tangencial += p1;
                    tangencial += p2;
                    tangencial -= p3;
                }
            }

            return tangencial;
        }

        private double CalcularTensaoTangencial3(double tempo, double raio, double teta)
        {
            var lambda = Lambda.Calcular(tempo, cf);

            var B = propElasticas.B;
            var Vu = propElasticas.Vu;

            var constBessel = new ConstantesDeBessel(lambda, raio, this.raioPoco);
            var constantesAritmeticas = new ConstantesAritmeticas(lambda, this.propElasticas, this.raioPoco);

            var C1 = constantesAritmeticas.C1;
            var C3 = constantesAritmeticas.C3;

            var Ssh = matrizTensorDeTensoes.Ssh;

            var k1LambdaRaio = constBessel.K1LambdaRaio;
            var k2LambdaRaio = constBessel.K2LambdaRaio;


            var parcela1 = Ssh * Math.Cos(2 * teta);
            var parcela2 = ((-B * (1 + Vu) * C1) / (3 * (1 - Vu)) * (k1LambdaRaio / (lambda * raio) + (1 + (6 / (Math.Pow(lambda * raio, 2)))) * k2LambdaRaio) + ((3 * C3 * (Math.Pow(raioPoco.Value, 4))) / (Math.Pow(raio, 4))));
            return parcela1 * parcela2 / tempo;
        }

        private double CalcularTensaoTangencialI(double tangencial1, double tangencial2, double tangencial3, double teta)
        {
            var Ssh = matrizTensorDeTensoes.Ssh;
            var Psh = matrizTensorDeTensoes.Psh;

            return -Psh - Ssh * Math.Cos(2 * teta) + tangencial1 + tangencial2 + tangencial3;
        }

        private Laplace.FunctionDelegate GetFuncCalcularTangencial2(double raio)
        {
            double func(double x)
            {
                return this.CalcularTensaoTangencial2(x, raio);
            }
            return func;
        }

        private Laplace.FunctionDelegate GetFuncCalcularTangencial3(double raio, double teta)
        {
            double func(double x)
            {
                return this.CalcularTensaoTangencial3(x, raio, teta);
            }
            return func;
        }
    }
}
