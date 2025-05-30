using SestWeb.Domain.SistemasUnidades.Grupos.Base;
using SestWeb.Domain.SistemasUnidades.Unidades;

namespace SestWeb.Domain.SistemasUnidades.Grupos
{
    public class GrupoUnidadesDeProporção : GrupoUnidades
    {
        public GrupoUnidadesDeProporção() : base("Grupo de unidades de proporção", new Fator())
        {
            RegistrarUnidade(new Porcentagem(), 0.01);
        }
    }
}