using SestWeb.Domain.SistemasUnidades.Grupos.Base;
using SestWeb.Domain.SistemasUnidades.Unidades;

namespace SestWeb.Domain.SistemasUnidades.Grupos
{
    public class GrupoUnidadesDeÂngulo : GrupoUnidades
    {
        public GrupoUnidadesDeÂngulo() : base("Grupo de Unidades de ângulo", new Grau())
        {
            RegistrarUnidade(new Radiano(), 57.2957795);
            RegistrarUnidade(new SegundoDeArco(), 0.0002778);
        }
    }
}