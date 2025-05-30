namespace SestWeb.Domain.Entities.Trajetoria
{
    public class PontoProjeção
    {
        public PontoProjeção(double profundidade, double valor)
        {
            Profundidade = profundidade;
            Valor = valor;
        }

        public double Profundidade { get; private set; }
        public double Valor { get; private set; }
    }
}