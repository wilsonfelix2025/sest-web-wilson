using System;
using MathNet.Numerics;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Tensões;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Util;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos.ModeloPoroElastico
{
    public class CalculadorPressoes
    {
        private readonly double pressaoPoros;
        private readonly double pw;
        private readonly double cf;
        private readonly RaioPoco raioPoco;
        private readonly PropriedadesElasticas propElasticas;
        private readonly MatrizTensorDeTensoes matrizTensorDeTensoes;
        private readonly DadosEntradaModeloPoroElastico dadosEntradaModeloPoroElastico;
        public CalculadorPressoes(double pressaoPoros, double pw, double cf, RaioPoco raioPoco, PropriedadesElasticas propElasticas, MatrizTensorDeTensoes matrizTensorDeTensoes, DadosEntradaModeloPoroElastico dadosEntradaModeloPoroElastico)
        {
            this.dadosEntradaModeloPoroElastico = dadosEntradaModeloPoroElastico;
            this.matrizTensorDeTensoes = matrizTensorDeTensoes;
            this.propElasticas = propElasticas;
            this.raioPoco = raioPoco;
            this.cf = cf;
            this.pw = pw;
            this.pressaoPoros = pressaoPoros;
        }

        private Laplace.FunctionDelegate GetFuncCalcularPressao2(double raio)
        {
            double func(double x)
            {
                return CalcularPressao2(x, raio);
            }
            return func;
        }

        private Laplace.FunctionDelegate GetFuncCalcularPressao3(double raio, double teta)
        {
            double func(double x)
            {
                return CalcularPressao3(x, raio, teta);
            }
            return func;
        }

        private double CalcularPressao2(double tempo, double raio)
        {
            var lambda = Lambda.Calcular(tempo, cf);

            var k0LambdaRaio = SpecialFunctions.BesselK0(lambda * raio);
            var k0LambdaRaioPoco = SpecialFunctions.BesselK0(lambda * raioPoco.Value);

            var pressao = (pw - pressaoPoros) * k0LambdaRaio / (tempo * k0LambdaRaioPoco);

            if (this.dadosEntradaModeloPoroElastico.EfeitoTermico)
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

                    pressao -= (cft * deltaT) / dx * 1 / tempo * (k0LambdaRaio / k0LambdaRaioPoco);
                    pressao += (cft * deltaT) / dx * 1 / tempo * (k0(qt * raio) / k0(qt * raioPoco.Value));
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
                //var cfC = -651347069310.96423;

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
                    var nabla = this.propElasticas.Nabla;

                    Func<double, double> k0 = SpecialFunctions.BesselK0;
                    Func<double, double> k1 = SpecialFunctions.BesselK1;

                    pressao += (cfC * deltaC) / dx * 1 / tempo * (k0(lambda * raio) / k0(lambda * raioPoco.Value));
                    pressao -= (cfC * deltaC) / dx * 1 / tempo * (k0(qc * raio) / k0(qc * raioPoco.Value));
                }
            }

            return pressao;
        }

        private double CalcularPressao3(double tempo, double raio, double teta)
        {
            var lambda = Lambda.Calcular(tempo, cf);

            var constantesAritmeticas = new ConstantesAritmeticas(lambda, this.propElasticas, this.raioPoco);

            var B = propElasticas.B;
            var Vu = propElasticas.Vu;
            var poisson = propElasticas.Poisson;
            var C1 = constantesAritmeticas.C1;
            var C2 = constantesAritmeticas.C2;

            var a = matrizTensorDeTensoes.Value[0, 0];
            var b = matrizTensorDeTensoes.Value[0, 1];
            var c = matrizTensorDeTensoes.Value[1, 1];

            var Ssh = Math.Sqrt((Math.Pow(((a - c) / 2), 2) + Math.Pow(b, 2)));

            var k0LambdaRaio = SpecialFunctions.BesselK0(lambda * raio);
            var k0LambdaRaioPoco = SpecialFunctions.BesselK0(lambda * raioPoco.Value);
            var k2LambdaRaio = SpecialFunctions.BesselK(2, lambda * raio);

            return Ssh * Math.Cos(2 * teta) * ((Math.Pow(B, 2) * (1 - poisson) * (Math.Pow(1 + Vu, 2) * C1 * k2LambdaRaio) / (9 * (1 - Vu) * (Vu - poisson)) + (B * (1 + Vu) * C2 * Math.Pow(raioPoco.Value, 2)) / (3 * (1 - Vu) * (Math.Pow(raio, 2))))) / tempo;
        }

        public Pressoes CalcularPressoes(double tempo, PontoDaMalha pontoDaMalha)
        {
            var raio = pontoDaMalha.Raio;
            var angulo = pontoDaMalha.Angulo;

            var p2 = Laplace.InverseTransform(GetFuncCalcularPressao2(raio), tempo);
            var p3 = Laplace.InverseTransform(GetFuncCalcularPressao3(raio, angulo), tempo);
            var pI = this.pressaoPoros + p2 + p3;

            return new Pressoes(pI, p2, p3);
        }
    }
}
