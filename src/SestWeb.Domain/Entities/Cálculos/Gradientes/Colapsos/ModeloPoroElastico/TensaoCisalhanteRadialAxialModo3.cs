using System;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos.ModeloPoroElastico
{
    public class TensaoCisalhanteRadialAxialModo3
    {
        public double Value { get; }

        public TensaoCisalhanteRadialAxialModo3(RaioPoco raioPoco, double raio, double teta, MatrizTensorDeTensoes matrizTensorDeTensoes)
        {
            var Tzx = matrizTensorDeTensoes.Value[0, 2];
            var Tzy = matrizTensorDeTensoes.Value[1, 2];

            Value = -(Tzx * Math.Cos(teta) + Tzy * Math.Sin(teta)) * (1 - (Math.Pow(raioPoco.Value, 2) / Math.Pow(raio, 2)));
        }
    }
}
