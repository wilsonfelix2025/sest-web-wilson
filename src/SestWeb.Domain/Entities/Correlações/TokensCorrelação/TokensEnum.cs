using System.ComponentModel;

namespace SestWeb.Domain.Entities.Correlações.TokensCorrelação
{
    public enum TokensEnum
    {
        [Description("Grupo Litológico")]
        GRUPO_LITOLOGICO,
        [Description("Profundidade")]
        PROFUNDIDADE,
        [Description("Profundidade inicial")]
        PROFUNDIDADE_INICIAL,
        [Description("RHOB inicial")]
        RHOB_INICIAL,
        [Description("Densidade da água do mar")]
        DENSIDADE_AGUA_MAR,
        [Description("Lâmina d'água")]
        LAMINA_DAGUA,
        [Description("Mesa Rotativa")]
        MESA_ROTATIVA,
        [Description("Altura do Antepoço")]
        ALTURA_ANTEPOCO,
        [Description("Categoria do Poço")]
        CATEGORIA_POCO,
        [Description("Poço Offshore")]
        OFFSHORE,
        [Description("Poço Onshore")]
        ONSHORE,
        [Description("Diferença entre profundidades subsequentes")]
        STEP
    }
}
