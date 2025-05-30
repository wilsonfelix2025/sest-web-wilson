using System.ComponentModel;

namespace SestWeb.Domain.Exportadores.Base
{
    public enum TipoExportação
    {
        [Description("Arquivo .las")]
        LAS,
        [Description("Arquivo. csv")]
        CSV
    }
}
