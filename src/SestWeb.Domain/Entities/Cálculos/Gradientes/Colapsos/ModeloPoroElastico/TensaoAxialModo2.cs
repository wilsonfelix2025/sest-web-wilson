using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos.ModeloPoroElastico
{
    public class TensaoAxialModo2
    {
        public double Value { get; }

        public TensaoAxialModo2(double pressaoPoros, MatrizTensorDeTensoes matrizTensorDeTensoes, PropriedadesElasticas propElasticas)
        {
            var poisson = propElasticas.Poisson;
            var relacaoBiot = propElasticas.RelacaoBiot;

            var a = matrizTensorDeTensoes.Value[0, 0];
            var b = matrizTensorDeTensoes.Value[1, 1];
            var c = matrizTensorDeTensoes.Value[2, 2];

            Value = -c + (poisson * (a + b) + relacaoBiot * (1 - 2 * poisson) * pressaoPoros);
        }
    }
}
