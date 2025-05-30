using SestWeb.Domain.SistemasUnidades.Grupos.Base;
using SestWeb.Domain.SistemasUnidades.Unidades;

namespace SestWeb.Domain.SistemasUnidades.Grupos
{
    public class GrupoUnidadesDeTaxaPenetração : GrupoUnidades
    {
        public GrupoUnidadesDeTaxaPenetração() : base("Grupo de unidades de taxa de penetração", new MetroPorHora())
        {
            RegistrarUnidade(new MetroPorMinuto(), 60);
            RegistrarUnidade(new MetroPorSegundo(), 3600);
            RegistrarUnidade(new PéPorHora(), 0.3048);
            RegistrarUnidade(new PéPorMinuto(), 18.288);
            RegistrarUnidade(new PéPorSegundo(), 1097.28);
        }
    }
}