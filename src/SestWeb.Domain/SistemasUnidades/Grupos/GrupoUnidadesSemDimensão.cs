using SestWeb.Domain.SistemasUnidades.Grupos.Base;
using SestWeb.Domain.SistemasUnidades.Unidades;

namespace SestWeb.Domain.SistemasUnidades.Grupos
{
    public class GrupoUnidadesSemDimensão : GrupoUnidades
    {
        public GrupoUnidadesSemDimensão() : base("Grupo de unidades sem dimensão", new SemDimensão())
        {
        }
    }
}