using SestWeb.Domain.SistemasUnidades.Grupos.Base;
using SestWeb.Domain.SistemasUnidades.Unidades;

namespace SestWeb.Domain.SistemasUnidades.Grupos
{
    public class GrupoUnidadesDeDiâmetro : GrupoUnidades
    {
        public GrupoUnidadesDeDiâmetro() : base("Grupo de unidades de diâmetro", new Polegada())
        {
            RegistrarUnidade(new Pé(), 0.3048);
            RegistrarUnidade(new Metro(), 39.37008);
            RegistrarUnidade(new Centímetro(), 0.01);
            RegistrarUnidade(new Milímetro(), 0.001);
        }
    }
}