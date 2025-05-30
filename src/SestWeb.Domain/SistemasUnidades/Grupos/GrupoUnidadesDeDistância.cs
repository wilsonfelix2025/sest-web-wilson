using SestWeb.Domain.SistemasUnidades.Grupos.Base;
using SestWeb.Domain.SistemasUnidades.Unidades;

namespace SestWeb.Domain.SistemasUnidades.Grupos
{
    public class GrupoUnidadesDeDistância : GrupoUnidades
    {
        public GrupoUnidadesDeDistância() : base("Grupo de unidades de distância", new Metro())
        {
            RegistrarUnidade(new Pé(), 0.3048);
            RegistrarUnidade(new Centímetro(), 0.01);
            RegistrarUnidade(new Milímetro(), 0.001);
            RegistrarUnidade(new Polegada(), 0.0254);
        }
    }
}