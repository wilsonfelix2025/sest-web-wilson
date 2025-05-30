namespace SestWeb.Domain.EstilosVisuais
{
    public class EstiloVisual
    {
        public EstiloVisual()
        {
            CorDaLinha = "black";
            Espessura = 1.0;
            Marcador = TipoMarcador.Nenhum;
            CorDoMarcador = "Transparent";
            EstiloLinha = EstiloLinha.Solid;
        }

        public EstiloVisual(string corDaLinha, double espessura, EstiloLinha estiloLinha, TipoMarcador marcador, string corDoMarcador)
        {
            CorDaLinha = corDaLinha;
            Espessura = espessura;
            EstiloLinha = estiloLinha;
            Marcador = marcador;
            CorDoMarcador = corDoMarcador;
        }

        public string CorDaLinha { get; private set; }
        public double Espessura { get; private set; }
        public EstiloLinha EstiloLinha { get; private set; }
        public TipoMarcador Marcador { get; private set; }
        public string CorDoMarcador { get; private set; }
    }

    public enum EstiloLinha
    {
        Normal,
        Pontilhada,
        Tracejada,
        Solid,
        ShortDash,
        ShortDot,
        ShortDashDot,
        ShortDashDotDot,
        Dot,
        Dash,
        LongDash,
        DashDot,
        LongDashDot,
        LongDashDotDot
    }

    public enum TipoMarcador
    {
        Circulo,
        Diamante,
        Quadrado,
        Triangulo,
        TrianguloInvertido,
        Nenhum
    }
}