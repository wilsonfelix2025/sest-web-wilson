using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Tensões;
using SestWeb.Domain.Enums;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.FatorPlastificacao
{
    public class FpCalculator
    {
        private double restr;
        private readonly double poisson;
        private readonly double pv;
        private readonly ImmutableList<PontoDaMalha> pontos;
        private readonly RaioPoco raioPoco;
        private readonly double pressaoPoros;
        private readonly double biot;
        private readonly double ucs;
        private readonly double angat;
        private readonly double coesao;
        private bool ehFluidoPenetrante;
        private readonly CritérioRupturaGradientesEnum tipoCritério;

        private FpCalculator(double pv, Malha.Malha malha, RaioPoco raioPoco, double pressaoPoros, double biot, double ucs, double angat, double restr, bool areaPlastificada, CritérioRupturaGradientesEnum tipoCritérioRuptura, double coesao)
        {
            this.coesao = coesao;
            this.tipoCritério = tipoCritérioRuptura;
            this.angat = angat;
            this.ucs = ucs;
            this.biot = biot;
            this.pressaoPoros = pressaoPoros;
            this.raioPoco = raioPoco;
            if (areaPlastificada)
            {
                this.pontos = malha.Pontos;
            }
            else
            {
                this.pontos = malha.GetPontosParedePoco();
            }
            this.pv = pv;
            this.restr = restr;
            ehFluidoPenetrante = false;
        }


        private FpCalculator(double pv, Malha.Malha malha, RaioPoco raioPoco, double pressaoPoros, double biot, double ucs, double angat, double restr, double poisson, bool areaPlastificada, CritérioRupturaGradientesEnum tipoCritérioRuptura, double coesao) : this(pv, malha, raioPoco, pressaoPoros, biot, ucs, angat, restr, areaPlastificada, tipoCritérioRuptura, coesao)
        {
            this.poisson = poisson;
            ehFluidoPenetrante = true;
        }

        public static FpCalculator ElasticoNaoPenetrante(double pv, Malha.Malha malha, RaioPoco raioPoco, double pressaoPoros, double biot, double ucs, double angat, double restr, bool areaPlastificada, CritérioRupturaGradientesEnum tipoCritério, double coesao)
        {
            return new FpCalculator(pv, malha, raioPoco, pressaoPoros, biot, ucs, angat, restr, areaPlastificada, tipoCritério, coesao);
        }

        public static FpCalculator ElasticoPenetrante(double pv, Malha.Malha malha, RaioPoco raioPoco, double pressaoPoros, double biot, double ucs, double angat, double restr, double poisson, bool areaPlastificada, CritérioRupturaGradientesEnum tipoCritério, double coesao)
        {
            return new FpCalculator(pv, malha, raioPoco, pressaoPoros, biot, ucs, angat, restr, poisson, areaPlastificada, tipoCritério, coesao);
        }

        public double CalcularFp(double pw)
        {
            var fps = new List<double>(pontos.Count);
            //for (int i = 0; i < pontos.Count; i++)
            Parallel.For(0, pontos.Count, i =>
            {
                PontoDaMalha ponto = this.pontos[i];
                var tensoesAoRedorPoco = new TensoesAoRedorPocoModeloElastico();
                tensoesAoRedorPoco.CalcularElasticoNaoPenetrante(ponto, pw, this.pressaoPoros);

                if (this.ehFluidoPenetrante)
                {
                    tensoesAoRedorPoco.CalcularElasticoPenetrante(biot, poisson, pw, raioPoco, pressaoPoros, ponto.Raio);
                }

                var tensoesPrincipais = new TensoesPrincipais();
                tensoesPrincipais.CalcularTensoesPrincipais(this.raioPoco, ponto.Raio, tensoesAoRedorPoco, this.pressaoPoros, this.biot);

                var fatorPlastificacao = new FatorDePlastificacaoPorTipoCriterio();
                var fp = 0.0;

                ValidarValorTensãoPrincipalEfetifiva3(tensoesPrincipais);


                switch (tipoCritério)
                {       
                    case CritérioRupturaGradientesEnum.MohrCoulomb:
                        fp = fatorPlastificacao.CalcularPorMC(this.ucs, this.angat, this.restr, tensoesPrincipais);
                        break;
                    case CritérioRupturaGradientesEnum.Lade:
                        fp = fatorPlastificacao.CalcularPorLade(this.angat, this.coesao, tensoesPrincipais);
                        break;
                    case CritérioRupturaGradientesEnum.DruckerPraggerInternal:
                        fp = fatorPlastificacao.CalcularPorDPInterno(this.angat, this.coesao, tensoesPrincipais);                    
                        break;
                    case CritérioRupturaGradientesEnum.DruckerPraggerMiddle:
                        fp = fatorPlastificacao.CalcularPorDPCentral(this.angat, this.coesao, tensoesPrincipais);
                        break;
                    case CritérioRupturaGradientesEnum.DruckerPraggerExternal:
                        fp = fatorPlastificacao.CalcularPorDPExterno(this.angat, this.coesao, tensoesPrincipais);
                        break;
                    default:
                        fp = fatorPlastificacao.CalcularPorMC(this.ucs, this.angat, this.restr, tensoesPrincipais);
                        break;
                }

                ponto.FatorPlastificação = fp;
                fps.Add(fp);

                ponto.TensoesAoRedorPoco = tensoesAoRedorPoco;
                ponto.TensoesPrincipais = tensoesPrincipais;
            //    }
            });
            var max = fps.Max();

            return max;
        }

        private void ValidarValorTensãoPrincipalEfetifiva3(TensoesPrincipais tensoesPrincipais)
        {
            var denominador = Math.Pow(Math.Tan(Math.PI / 4 + angat / 2), 2);
            var razão = -this.coesao / denominador;

            if (!(Math.Abs(tensoesPrincipais.TensaoPrincipalEfetiva3) > Math.Abs(razão)))
            {
                this.restr = this.coesao / denominador;
            }
        }
    }
}
