using SestWeb.Domain.SistemasUnidades.Grupos.Base;
using SestWeb.Domain.SistemasUnidades.Unidades;

namespace SestWeb.Domain.SistemasUnidades.Grupos
{
    public class GrupoUnidadesDeRotação : GrupoUnidades
    {
        public GrupoUnidadesDeRotação() : base("Grupo de unidades de rotação", new RotaçõesPorMinuto())
        {
            RegistrarUnidade(new RotaçõesPorSegundo(), 1.0 / 60);
            RegistrarUnidade(new RadianosPorSegundo(), 9.54929659643);
            RegistrarUnidade(new RotaçõesPorHora(), 60);
        }
    }
}