using System;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos.ModeloPoroElastico
{
    public class TensaoCisalhanteTangencialAxialModo3
    {
        public double Value { get; }

        public TensaoCisalhanteTangencialAxialModo3(RaioPoco raioPoco, double raio, double teta, MatrizTensorDeTensoes matrizTensorDeTensoes)
        {
            var a = matrizTensorDeTensoes.Value[0, 2];
            var b = matrizTensorDeTensoes.Value[1, 2];

            Value = (a * Math.Sin(teta) - b * Math.Cos(teta)) * (1 + Math.Pow(raioPoco.Value, 2) / Math.Pow(raio, 2));
        }
    }
}
