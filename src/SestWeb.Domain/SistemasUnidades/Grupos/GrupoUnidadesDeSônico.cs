using SestWeb.Domain.SistemasUnidades.Grupos.Base;
using SestWeb.Domain.SistemasUnidades.Unidades;

namespace SestWeb.Domain.SistemasUnidades.Grupos
{
    public class GrupoUnidadesDeSônico : GrupoUnidades
    {
        public GrupoUnidadesDeSônico() : base("Grupo de unidades de sônico", new MicrossegundoPorPé())
        {
            RegistrarUnidade(new SegundoPorMetro(), 304799.9995367040007042099189296);
        }
    }
}