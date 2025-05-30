using System.ComponentModel;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.Base
{
    public enum CorrelaçãoPressãoPoros
    {
        [Description("Eaton DTC Filtrado")]
        EatonDTC,
        [Description("Eaton Expoente D Filtrado")]
        EatonExpoenteD,
        [Description("Eaton Resistividade Filtrado")]
        EatonResistividade,
        [Description("Hidrostática")]
        Hidrostática,
        [Description("Gradiente")]
        Gradiente
    }
}
