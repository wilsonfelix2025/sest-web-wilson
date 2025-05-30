using System;
using MathNet.Numerics;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Tensões;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Util;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos.ModeloPoroElastico
{
    public class CalculadorTensoesRadiais
    {
        private readonly RaioPoco raioPoco;
        private readonly PropriedadesElasticas propElasticas;
        private readonly MatrizTensorDeTensoes matrizTensorDeTensoes;
        private readonly double pw;
        private readonly double pressaoPoros;
        private readonly double cf;
        private readonly DadosEntradaModeloPoroElastico dadosEntradaModeloPoroElastico;
        public CalculadorTensoesRadiais(RaioPoco raioPoco, PropriedadesElasticas propElasticas, MatrizTensorDeTensoes matrizTensorDeTensoes, double pw, double pressaoPoros, double cf, DadosEntradaModeloPoroElastico dadosEntradaModeloPoroElastico)
        {
            this.dadosEntradaModeloPoroElastico = dadosEntradaModeloPoroElastico;
            this.cf = cf;
            this.pressaoPoros = pressaoPoros;
            this.pw = pw;
            this.matrizTensorDeTensoes = matrizTensorDeTensoes;
            this.propElasticas = propElasticas;
            this.raioPoco = raioPoco;
        }

        public TensoesRadiais CalcularTensoesRadiais(double tempo, PontoDaMalha pontoDaMalha)
        {
            var raio = pontoDaMalha.Raio;
            var angulo = pontoDaMalha.Angulo;

            var radial1 = this.CalcularRadial1(raio);
            var radial2 = Laplace.InverseTransform(GetFuncCalcularRadial2(raio), tempo);
            var radial3 = Laplace.InverseTransform(GetFuncCalcularRadial3(raio, angulo), tempo);

            var radialI = this.CalcularRadialI(angulo, radial1, radial2, radial3);

            return new TensoesRadiais(radial1, radial2, radial3, radialI);
        }

        private double CalcularRadial1(double raio)
        {
            var pSh = matrizTensorDeTensoes.Psh;

            return (pSh - this.pw) * Math.Pow((raioPoco.Value / raio), 2);
        }

        private double CalcularRadial2(double tempo, double raio)
        {
            var nabla = propElasticas.Nabla;
            var lambda = Lambda.Calcular(tempo, this.cf);

            var constBessel = new ConstantesDeBessel(lambda, raio, this.raioPoco);

            var k1LambdaRaio = constBessel.K1LambdaRaio;
            var k1LambdaRaioPoco = constBessel.K1LambdaRaioPoco;
            var k0LambdaRaioPoco = constBessel.K0LambdaRaioPoco;

            var radial = 2 * nabla * (pw - pressaoPoros) * ((k1LambdaRaio / (tempo * raio * lambda * k0LambdaRaioPoco)) - (raioPoco.Value * k1LambdaRaioPoco) / (tempo * Math.Pow(raio, 2) * lambda * k0LambdaRaioPoco));

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

                    radial += (-2 * nabla) * (cft * deltaT) / dx * 1 / tempo * ((k1LambdaRaio / (raio * lambda * k0LambdaRaioPoco)) - raioPoco.Value / (raio * raio) * k1(raioPoco.Value * lambda) / (lambda * k0(lambda * raioPoco.Value)));

                    radial += (2 * nabla) * (cft * deltaT) / dx * 1 / tempo * (k1(qt * raio) / (raio * qt * k0(qt * raioPoco.Value)) - raioPoco.Value / (raio * raio) * k1(qt * raioPoco.Value) / (qt * k0(qt * raioPoco.Value)));

                    radial += (2 * G * expensaoTermicaRocha * (1 + poisson)) / (3 * (1 - poisson)) * deltaT * 1 / tempo * (k1(qt * raio) / (raio * qt * k0(qt * raioPoco.Value)) - raioPoco.Value / (raio * raio) * k1(qt * raioPoco.Value) / (qt * k0(qt * raioPoco.Value)));
                }
            }

            if (dadosEntradaModeloPoroElastico.EfeitoFisicoQuimico)
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

                    radial += (2 * nabla) * (-cfC * deltaC / dx) * (k1(lambda * raio) / (tempo * raio * lambda * k0(lambda * raioPoco.Value)) - raioPoco.Value * k1(lambda * raioPoco.Value) / (tempo * raio * raio * lambda * k0(lambda * raioPoco.Value)));
                    radial += (2 * nabla) * (cfC * deltaC / dx) * (k1(qc * raio) / (tempo * raio * qc * k0(qc * raioPoco.Value)) - raioPoco.Value * k1(qc * raioPoco.Value) / (tempo * raio * raio * qc * k0(qc * raioPoco.Value)));
                    radial += (exis * (1 - 2 * poisson) / (1 - poisson)) * deltaC * (k1(qc * raio) / (tempo * qc * raio * k0(qc * raioPoco.Value)) - raioPoco.Value * k1(qc * raioPoco.Value) / (tempo * raio * raio * qc * k0(qc * raioPoco.Value)));
                }
            }

            return radial;
        }

        private double CalcularRadial3(double tempo, double raio, double teta)
        {
            var Ssh = matrizTensorDeTensoes.Ssh;
            var lambda = Lambda.Calcular(tempo, this.cf);

            var B = propElasticas.B;
            var Vu = propElasticas.Vu;

            var constantesAritmeticas = new ConstantesAritmeticas(lambda, this.propElasticas, this.raioPoco);
            var constBessel = new ConstantesDeBessel(lambda, raio, this.raioPoco);

            var C1 = constantesAritmeticas.C1;
            var C2 = constantesAritmeticas.C2;
            var C3 = constantesAritmeticas.C3;

            var k1LambdaRaio = constBessel.K1LambdaRaio;
            var k2LambdaRaio = constBessel.K2LambdaRaio;

            return Ssh * Math.Cos(2 * teta) / tempo * ((((B * (1 + Vu) * C1) / (3 * (1 - Vu))) * (k1LambdaRaio / (lambda * raio) + (6 * k2LambdaRaio) / Math.Pow(lambda * raio, 2))) - ((C2 * Math.Pow(raioPoco.Value, 2)) / ((Math.Pow(raio, 2)) * (1 - Vu))) - ((3 * C3 * Math.Pow(raioPoco.Value, 4)) / (Math.Pow(raio, 4))));
        }

        private double CalcularRadialI(double teta, double radial1, double radial2, double radial3)
        {
            var Ssh = matrizTensorDeTensoes.Ssh;
            var Psh = matrizTensorDeTensoes.Psh;

            return -Psh + Ssh * Math.Cos(2 * teta) + radial1 + radial2 + radial3;
        }

        private Laplace.FunctionDelegate GetFuncCalcularRadial2(double raio)
        {
            double func(double x)
            {
                return CalcularRadial2(x, raio);
            }
            return func;
        }

        private Laplace.FunctionDelegate GetFuncCalcularRadial3(double raio, double teta)
        {
            double func(double x)
            {
                return CalcularRadial3(x, raio, teta);
            }
            return func;
        }
    }
}
