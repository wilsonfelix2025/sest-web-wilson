namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos.ModeloPoroElastico
{
    public class TensoesRadiais
    {
        public TensoesRadiais(double tensaoRadial1, double tensaoRadial2, double tensaoRadial3, double tensaoRadialI)
        {
            TensaoRadial1 = tensaoRadial1;
            TensaoRadial2 = tensaoRadial2;
            TensaoRadial3 = tensaoRadial3;
            TensaoRadialI = tensaoRadialI;
        }

        public double TensaoRadial1 { get; }
        public double TensaoRadial2 { get; }
        public double TensaoRadial3 { get; }
        public double TensaoRadialI { get; }
    }
}
