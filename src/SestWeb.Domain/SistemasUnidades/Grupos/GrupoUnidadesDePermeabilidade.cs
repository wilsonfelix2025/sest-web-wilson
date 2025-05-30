using SestWeb.Domain.SistemasUnidades.Grupos.Base;
using SestWeb.Domain.SistemasUnidades.Unidades;

namespace SestWeb.Domain.SistemasUnidades.Grupos
{
    public class GrupoUnidadesDePermeabilidade : GrupoUnidades
    {
        public GrupoUnidadesDePermeabilidade() : base("Grupo de unidades de permeabilidade", new Milidarcy())
        {
            RegistrarUnidade(new Darcy(), 1000.0);
        }
    }
}