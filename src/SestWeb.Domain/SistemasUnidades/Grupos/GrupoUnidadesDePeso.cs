using SestWeb.Domain.SistemasUnidades.Grupos.Base;
using SestWeb.Domain.SistemasUnidades.Unidades;

namespace SestWeb.Domain.SistemasUnidades.Grupos
{
    public class GrupoUnidadesDePeso : GrupoUnidades
    {
        public GrupoUnidadesDePeso() : base("Grupo de unidades de peso", new Tonelada())
        {
            RegistrarUnidade(new Libra(), 0.000453592);
            RegistrarUnidade(new Newton(), 1 / 1000 * 1 / 9.80665);
            RegistrarUnidade(new QuiloNewton(), 1 / 9.80665);
            RegistrarUnidade(new QuiloLibra(), 1000 * 0.000453592);
            RegistrarUnidade(new QuilogramaForça(), 1.0 / 1000);
        }
    }
}