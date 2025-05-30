using System.Collections.Generic;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Domain.Entities.Cálculos.Base.TrechosDeCálculo
{
    public interface ITrechoCálculo
    {
        Correlação Correlação { get; set; }

        double Topo { get; set; }

        double Base { get; set; }

         Dictionary<string, double> Parâmetros { get; set; }
    }
}
