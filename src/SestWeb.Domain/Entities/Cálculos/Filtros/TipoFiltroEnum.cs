using System.ComponentModel;

namespace SestWeb.Domain.Entities.Cálculos.Filtros
{
    public enum TipoFiltroEnum
    {
        [Description("Filtro simples")]
        FiltroSimples,
        [Description("Filtro por litologia")]
        FiltroLitologia,
        [Description("Filtro por média móvel")]
        FiltroMédiaMóvel,
        [Description("Filtro do tipo linha base folhelho")]
        FiltroLinhaBaseFolhelho,
        [Description("Filtro de corte")]
        FiltroCorte,

    }
}