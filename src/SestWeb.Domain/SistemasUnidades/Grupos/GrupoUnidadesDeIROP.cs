using SestWeb.Domain.SistemasUnidades.Base;
using SestWeb.Domain.SistemasUnidades.Grupos.Base;
using SestWeb.Domain.SistemasUnidades.Unidades;

namespace SestWeb.Domain.SistemasUnidades.Grupos
{
    public class GrupoUnidadesDeIROP : GrupoUnidades
    {
        public GrupoUnidadesDeIROP() : base("Grupo de unidades de taxa de penetração para IROP", new MinutoPorMetro())
        {
            RegistrarUnidade(new MetroPorHora(), 60);
        }

        protected override double Convert(double valor, double fator, UnidadeMedida unidadeOrigem)
        {
            return fator / valor;
        }
    }
}