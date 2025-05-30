using System;
using MongoDB.Bson.Serialization.Attributes;

namespace SestWeb.Domain.Entities.Trend
{
    public class TrechoTrend
    {

        
        public TrechoTrend(double pvTopo, double valorTopo, double pvBase, double valorBase)
        {
            PvTopo = pvTopo;
            ValorTopo = valorTopo;
            PvBase = pvBase;
            ValorBase = valorBase;
            Inclinação = CalcularInclinação(this);
        }

        [BsonConstructor]
        public TrechoTrend(double pvTopo, double valorTopo, double pvBase, double valorBase, double inclinação)
        {
            PvTopo = pvTopo;
            ValorTopo = valorTopo;
            PvBase = pvBase;
            ValorBase = valorBase;
            Inclinação = inclinação;
        }

        [BsonElement]
        public double ValorTopo { get; }
        [BsonElement]
        public double PvTopo { get; }
        [BsonElement]
        public double ValorBase { get; }
        [BsonElement]
        public double PvBase { get; }
        [BsonElement]
        public double Inclinação { get; }

        //calcula a inclinação da reta do trend, a partir do PV e do Valor
        public double CalcularInclinação(TrechoTrend trecho)
        {
            if (trecho.ValorBase == trecho.ValorTopo)
                return 0;

            return (trecho.PvBase - trecho.PvTopo) / (trecho.ValorBase - trecho.ValorTopo);
        }

        public double CalcularValor(double pvBase, double pvTopo, double inclinação, double valorBase, double valorTopoTrecho)
        {
            var valorTopo = 0.0;

            if (Math.Abs(valorBase - valorTopoTrecho) < 0.01)
                valorTopo = valorTopoTrecho;
            else
            {
                if (Math.Abs(inclinação) < 0.01)
                    valorTopo = valorBase;
                else
                    valorTopo = ((pvBase - pvTopo) / inclinação - valorBase) * -1;
            }

            return valorTopo;
        }

        public TrechoTrend EditarInclinação(TrechoTrend trecho, double inclinação)
        {
            return trecho;
        }

        public TrechoTrend EditarProfundidade(TrechoTrend trecho, double profundidade)
        {
            return trecho;
        }

    }
}
