namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos.ModeloPoroElastico
{
    public class Pressoes
    {
        public Pressoes(double pressaoI, double pressao2, double pressao3)
        {
            this.PressaoI = pressaoI;
            this.Pressao2 = pressao2;
            this.Pressao3 = pressao3;

        }
        public double PressaoI { get; }
        public double Pressao2 { get; }
        public double Pressao3 { get; }
    }
}
