
using System.Collections.Generic;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Domain.Entities.Cálculos.Base.TrechosDeCálculo
{
    public class TrechoCálculo : ITrechoCálculo
    {
        public TrechoCálculo(Correlação correlação, double topo, double baSe, Dictionary<string, double> parâmetros = null)
        {
            Correlação = correlação;
            Topo = topo;
            Base = baSe;
            Parâmetros = parâmetros;
        }

        public Correlação Correlação { get; set; }

        public double Topo { get; set; }

        public double Base { get; set; }

        public Dictionary<string, double> Parâmetros { get; set; }

        
    }
}
