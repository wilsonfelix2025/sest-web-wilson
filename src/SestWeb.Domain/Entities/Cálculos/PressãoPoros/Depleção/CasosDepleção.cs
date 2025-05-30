using System.ComponentModel;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.Depleção
{
    public enum CasosDepleção
    {
        [Description("Se PVc < PVi e PVc < PVgás/óleo e PVóleo/água < PVi")]
        Caso1,
        [Description("Se PVc < PVi e PVgás/óleo < PVc < PVóleo/água < PVi")]
        Caso2,
        [Description("Se PVc < PVi e PVóleo/água < PVc")]
        Caso3,
        [Description("Se PVc > PVi e PVc > PVóleo/água e PVi < PVgás/óleo")]
        Caso4,
        [Description("Se PVc > PVi e PVóleo/água > PVc > PVgás/óleo e PVi < PVgás/óleo")]
        Caso5,
        [Description("Se PVc > PVi e PVc < PVgás/óleo")]
        Caso6,
        [Description("Se PVc < PVi e PVc < PVgás/óleo e PVgás/óleo < PVi < PVóleo/água")]
        Caso7,
        [Description("Se PVc < PVi e PVc < PVgás/óleo e PVi < PVgás/óleo")]
        Caso8,
        [Description("Se PVc < PVi e PVgás/óleo < PVc < PVóleo/água e PVgás/óleo < PVi < PVóleo/água")]
        Caso9,
        [Description("Se PVc > PVi e PVc > PVóleo/água e Pvgás/óleo < PVi < PVóleo/água")]
        Caso10,
        [Description("Se PVc > PVi e PVc > PVóleo/água e PVi > PVóleo/água")]
        Caso11,
        [Description("Se PVc > PVi e PVóleo/água > PVc > PVgás/óleo e PVgás/óleo < PVi < PVóleo/água")]
        Caso12
    }
}
