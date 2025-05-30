using SestWeb.Domain.SistemasUnidades.Grupos.Base;
using SestWeb.Domain.SistemasUnidades.Unidades;

namespace SestWeb.Domain.SistemasUnidades.Grupos
{
    public class GrupoUnidadesDeGradientes : GrupoUnidades
    {
        public GrupoUnidadesDeGradientes() : base("Grupo de unidades de gradientes", new LibraPorGalão())
        {
            RegistrarUnidade(new GramaPorCentímetroCúbico(), 8.3454064);
            RegistrarUnidade(new LibraPorPolegadaQuadradaPorPé(), 19.229744);
            RegistrarUnidade(new QuiloPascalPorMetro(), 0.8505805);
            RegistrarUnidade(new GravidadeEspecífica(), 8.3454064);

            RegistrarUnidade(new QuilogramaPorMetroCúbico(), 0.0083457);
            RegistrarUnidade(new GramasPorLitro(), 0.0083454);
            RegistrarUnidade(new QuilogramaPorLitro(), 8.3454064);
            RegistrarUnidade(new LibraPorPéCúbico(), 0.1336684);
            RegistrarUnidade(new LibraPorBarril(), 0.0238095);
            RegistrarUnidade(new LibraPorPolegadaQuadradaPorCemPés(), 0.1924117);
        }
    }
}