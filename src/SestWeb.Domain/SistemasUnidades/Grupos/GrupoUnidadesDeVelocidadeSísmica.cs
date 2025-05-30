using SestWeb.Domain.SistemasUnidades.Grupos.Base;
using SestWeb.Domain.SistemasUnidades.Unidades;

namespace SestWeb.Domain.SistemasUnidades.Grupos
{
    public class GrupoUnidadesDeVelocidadeSísmica : GrupoUnidades
    {
        public GrupoUnidadesDeVelocidadeSísmica() : base("Grupo de unidades de velocidade sísmica", new MetroPorSegundo())
        {
            RegistrarUnidade(new QuilometroPorSegundo(), 1000);
            RegistrarUnidade(new QuilopéPorSegundo(), 304.8);
            RegistrarUnidade(new PéPorSegundo(), 0.3048);
        }
    }
}
