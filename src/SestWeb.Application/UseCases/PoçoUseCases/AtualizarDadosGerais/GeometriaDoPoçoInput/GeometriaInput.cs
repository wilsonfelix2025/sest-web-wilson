namespace SestWeb.Application.UseCases.PoçoUseCases.AtualizarDadosGerais.GeometriaDoPoçoInput
{
    public class GeometriaInput
    {
        public OnShoreInput OnShore { get; set; } = new OnShoreInput();
        public OffShoreInput OffShore { get; set; } = new OffShoreInput();
        public CoordenadasInput Coordenadas { get; set; } = new CoordenadasInput();
        public double MesaRotativa { get; set; } = 0;
        public CategoriaDoPoçoInput CategoriaPoço { get; set; }
        public double PmFinal { get; set; } = 0;
        public bool ÉVertical { get; set; } = false;
    }
}
