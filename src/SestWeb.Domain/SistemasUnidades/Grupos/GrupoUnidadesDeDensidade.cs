using SestWeb.Domain.SistemasUnidades.Grupos.Base;
using SestWeb.Domain.SistemasUnidades.Unidades;

namespace SestWeb.Domain.SistemasUnidades.Grupos
{
    public class GrupoUnidadesDeDensidade : GrupoUnidades
    {
        public GrupoUnidadesDeDensidade() : base("Grupo de unidades de densidade", new GramaPorCentímetroCúbico())
        {
        }
    }
}