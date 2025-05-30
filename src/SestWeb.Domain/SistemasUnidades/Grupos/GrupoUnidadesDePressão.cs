using SestWeb.Domain.SistemasUnidades.Grupos.Base;
using SestWeb.Domain.SistemasUnidades.Unidades;

namespace SestWeb.Domain.SistemasUnidades.Grupos
{
    public class GrupoUnidadesDePressão : GrupoUnidades
    {
        public GrupoUnidadesDePressão() : base("Grupo de unidades de pressão", new LibraPorPolegadaQuadrada())
        {
            RegistrarUnidade(new Atmosfera(), 14.6959488);
            RegistrarUnidade(new MegaPascal(), 145.0377377);
            RegistrarUnidade(new Bar(), 14.5037738);
        }
    }
}