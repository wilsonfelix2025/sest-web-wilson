using System;
using MathNet.Numerics;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Tensões;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Util;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos.ModeloPoroElastico
{
    public class CalculadorConcentrações
    {
        private readonly double pressaoPoros;
        private readonly double pw;
        private readonly double cf;
        private readonly RaioPoco raioPoco;
        private readonly PropriedadesElasticas propElasticas;
        private readonly MatrizTensorDeTensoes matrizTensorDeTensoes;
        private readonly DadosEntradaModeloPoroElastico dadosEntradaModeloPoroElastico;
        public CalculadorConcentrações(double pressaoPoros, double pw, double cf, RaioPoco raioPoco, PropriedadesElasticas propElasticas, MatrizTensorDeTensoes matrizTensorDeTensoes, DadosEntradaModeloPoroElastico dadosEntradaModeloPoroElastico)
        {
            this.dadosEntradaModeloPoroElastico = dadosEntradaModeloPoroElastico;
            this.matrizTensorDeTensoes = matrizTensorDeTensoes;
            this.propElasticas = propElasticas;
            this.raioPoco = raioPoco;
            this.cf = cf;
            this.pw = pw;
            this.pressaoPoros = pressaoPoros;
        }

        private Laplace.FunctionDelegate GetFuncCalcularConcentração(double raio)
        {
            double func(double x)
            {
                return CalcularConcentrações(x, raio);
            }
            return func;
        }
    
        private double CalcularConcentrações(double tempo, double raio)
        {
            var concentração = 0.0;

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

                var k0QcRaio = SpecialFunctions.BesselK0(qc * raio);
                var k0QcRaioPoco = SpecialFunctions.BesselK0(qc * raioPoco.Value);


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


                    concentração = deltaC * k0QcRaio / (tempo * k0QcRaioPoco);

                }
            }

            return concentração;
        }

    
        //TODO mudar o retorno para concentrações
        public Pressoes CalcularPressoes(double tempo, PontoDaMalha pontoDaMalha)
        {
            var raio = pontoDaMalha.Raio;
            var angulo = pontoDaMalha.Angulo;

            var concentração = Laplace.InverseTransform(GetFuncCalcularConcentração(raio), tempo);

            return null;
        }
    }
}
