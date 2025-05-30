using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Tensões;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.FatorPlastificacao
{
    public class FpCalculatorPoroelastico
    {
        private readonly Malha.Malha malha;
        private readonly DadosEntradaModeloPoroElastico dadosEntradaPoroelastico;
        private readonly RaioPoco raioPoco;
        private readonly double pv;
        private readonly double ucs;
        private readonly double angat;
        private readonly double restr;
        private readonly ImmutableList<PontoDaMalha> pontos;


        public FpCalculatorPoroelastico(Malha.Malha malha, DadosEntradaModeloPoroElastico dadosEntradaPoroelastico, RaioPoco raioPoco, double pv, double ucs, double angat, double restr, bool areaPlastificada)
        {
            this.restr = restr;
            this.angat = angat;
            this.ucs = ucs;
            this.pv = pv;
            this.raioPoco = raioPoco;
            if (areaPlastificada)
            {
                this.pontos = malha.Pontos;
            }
            else
            {
                this.pontos = malha.GetPontosParedePoco();
            }
            this.dadosEntradaPoroelastico = dadosEntradaPoroelastico;
            this.malha = malha;
        }

        public double CalcularFp(double pw)
        {
            var fps = new List<double>(this.pontos.Count);
            foreach (var ponto in this.pontos)
            {
                var tensoesAoRedorPocoPoroelastico = new TensoesAoRedorPocoModeloPoroelastico(pw, this.dadosEntradaPoroelastico, ponto);
                var tensoesPrincipais = new TensoesPrincipais();
                tensoesPrincipais.CalcularTensoesPrincipais(raioPoco, ponto.Raio, tensoesAoRedorPocoPoroelastico, this.dadosEntradaPoroelastico.PressaoPoros, this.dadosEntradaPoroelastico.Biot);

                var fatorPlastificacao = new FatorDePlastificacaoPorTipoCriterio();

                var fp = fatorPlastificacao.CalcularPorMC(this.ucs, this.angat, this.restr, tensoesPrincipais);
                ponto.FatorPlastificação = fp;
                fps.Add(fp);

                ponto.TensoesAoRedorPoco = tensoesAoRedorPocoPoroelastico;
                ponto.TensoesPrincipais = tensoesPrincipais;
            }

            var max = fps.Max();

            return max;
        }
    }
}
