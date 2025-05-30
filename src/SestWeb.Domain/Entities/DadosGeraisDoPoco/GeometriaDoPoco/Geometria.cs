using System;

namespace SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco
{
    public class Geometria
    {
        public OnShore OnShore { get; set; } = new OnShore();
        public OffShore OffShore { get; set; } = new OffShore();
        public Coordenadas Coordenadas { get; set; } = new Coordenadas();

        public CategoriaPoço CategoriaPoço
        {
            get
            {
                if (OffShore.LaminaDagua == 0.0 && OnShore.Elevação > 0.0) return CategoriaPoço.OnShore;

                return CategoriaPoço.OffShore;
            }
        }

        public double MesaRotativa { get; set; } = 0;

        public void AjusteDeMesaRotativaComElevação(bool ajustaMesaRotativa)
        {
            if (CategoriaPoço == CategoriaPoço.OnShore && ajustaMesaRotativa)
            {
                MesaRotativa = Math.Round(MesaRotativa - OnShore.Elevação,2);
            }
        }
    }
}