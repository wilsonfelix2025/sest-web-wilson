namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos.ModeloPoroElastico
{
    public class TensoesCisalhanteRadialTangencial
    {

        public TensoesCisalhanteRadialTangencial(double tensaoCisalhanteRadialTangencial3, double tensaoCisalhanteRadialTangencialI)
        {
            TensaoCisalhanteRadialTangencial3 = tensaoCisalhanteRadialTangencial3;
            TensaoCisalhanteRadialTangencialI = tensaoCisalhanteRadialTangencialI;
        }

        public double TensaoCisalhanteRadialTangencial3 { get; }
        public double TensaoCisalhanteRadialTangencialI { get; }
    }
}
