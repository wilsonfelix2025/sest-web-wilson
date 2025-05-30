using System;
using MathNet.Numerics;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Tensões;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Util;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos.ModeloPoroElastico
{
    public class CalculadorTemperatura
    {
        private readonly double pressaoPoros;
        private readonly double pw;
        private readonly double cf;
        private readonly RaioPoco raioPoco;
        private readonly PropriedadesElasticas propElasticas;
        private readonly MatrizTensorDeTensoes matrizTensorDeTensoes;
        private readonly DadosEntradaModeloPoroElastico dadosEntradaModeloPoroElastico;
        public CalculadorTemperatura(double pressaoPoros, double pw, double cf, RaioPoco raioPoco, PropriedadesElasticas propElasticas, MatrizTensorDeTensoes matrizTensorDeTensoes, DadosEntradaModeloPoroElastico dadosEntradaModeloPoroElastico)
        {
            this.dadosEntradaModeloPoroElastico = dadosEntradaModeloPoroElastico;
            this.matrizTensorDeTensoes = matrizTensorDeTensoes;
            this.propElasticas = propElasticas;
            this.raioPoco = raioPoco;
            this.cf = cf;
            this.pw = pw;
            this.pressaoPoros = pressaoPoros;
        }

        private Laplace.FunctionDelegate GetFuncCalcularTemperatura(double raio)
        {
            double func(double x)
            {
                return CalcularTemperatura(x, raio);
            }
            return func;
        }
        
        private double CalcularTemperatura(double tempo, double raio)
        {
            var temperatura = 0.0;

            if (this.dadosEntradaModeloPoroElastico.EfeitoTermico)
            {
                var difusidade = this.dadosEntradaModeloPoroElastico.DifusividadeTermica;
                var expensaoTermicaRocha = this.dadosEntradaModeloPoroElastico.ExpansaoTermicaRocha;
                var expansaoTermicaFluidoPoros = this.dadosEntradaModeloPoroElastico.ExpansaoTermicaFluidoPoros;
                var temperaturaPoco = this.dadosEntradaModeloPoroElastico.TemperaturaPoco;
                var temperaturaFormacao = this.dadosEntradaModeloPoroElastico.TemperaturaFormacao;

                var qt = Math.Sqrt(tempo / difusidade);

                var k0QtRaio = SpecialFunctions.BesselK0(qt * raio);
                var k0QtRaioPoco = SpecialFunctions.BesselK0(qt * raioPoco.Value);

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

                    temperatura = deltaT * k0QtRaio / (tempo * k0QtRaioPoco);
                }
            }
       
            return temperatura;
        }

        //TODO mudar o retorno para temperatura
        public Pressoes CalcularPressoes(double tempo, PontoDaMalha pontoDaMalha)
        {
            var raio = pontoDaMalha.Raio;
            var angulo = pontoDaMalha.Angulo;

            var p2 = Laplace.InverseTransform(GetFuncCalcularTemperatura(raio), tempo);

            return null;
        }
    }
}
