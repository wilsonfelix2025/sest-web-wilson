namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos.ModeloPoroElastico
{
    public class TensoesTangenciais
    {
        public TensoesTangenciais(double tensaoTangencial1, double tensaoTangencial2, double tensaoTangencial3, double tensaoTangencialI)
        {
            TensaoTangencial1 = tensaoTangencial1;
            TensaoTangencial2 = tensaoTangencial2;
            TensaoTangencial3 = tensaoTangencial3;
            TensaoTangencialI = tensaoTangencialI;
        }

        public double TensaoTangencial1 { get; }
        public double TensaoTangencial2 { get; }
        public double TensaoTangencial3 { get; }
        public double TensaoTangencialI { get; }
    }
}
