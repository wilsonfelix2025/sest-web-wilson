using SestWeb.Domain.SistemasUnidades.Grupos.Base;
using SestWeb.Domain.SistemasUnidades.Unidades;

namespace SestWeb.Domain.SistemasUnidades.Grupos
{
    public class GrupoUnidadesDeResistividade : GrupoUnidades
    {
        public GrupoUnidadesDeResistividade() : base("Grupo de unidades de resistividade", new OhmPorMetro())
        {
        }
    }
}