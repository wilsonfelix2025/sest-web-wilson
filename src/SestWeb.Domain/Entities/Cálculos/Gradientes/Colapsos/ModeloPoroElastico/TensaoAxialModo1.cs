namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos.ModeloPoroElastico
{
    public class TensaoAxialModo1
    {
        public double Value { get; }

        public TensaoAxialModo1(double pressaoI, double tensaoRadialI, double tensaoTangencialI, PropriedadesElasticas propElasticas)
        {
            var poisson = propElasticas.Poisson;
            var relacaoBiot = propElasticas.RelacaoBiot;

            Value = poisson * (tensaoRadialI + tensaoTangencialI) - relacaoBiot * (1 - 2 * poisson) * pressaoI;
        }
    }
}
