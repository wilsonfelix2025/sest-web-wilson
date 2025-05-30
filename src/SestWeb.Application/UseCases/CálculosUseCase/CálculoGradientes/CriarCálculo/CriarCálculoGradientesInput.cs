using SestWeb.Domain.Enums;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoGradientes.CriarCálculo
{
    public class CriarCálculoGradientesInput
    {
        public string IdPoço { get; set; }
        public string NomeCálculo { get; set; }
        public ModeloAnáliseGradientesEnum TipoModelo { get; set; }
        public bool? FluidoPenetrante { get; set; }
        public double AreaPlastificada { get; set; }
        public CritérioRupturaGradientesEnum TipoCritérioRuptura { get; set; }
        public int? Tempo { get; set; }
        public bool? HabilitarFiltroAutomatico { get; set; }
        public bool? IncluirEfeitosFísicosQuímicos { get; set; }
        public bool? IncluirEfeitosTérmicos { get; set; }
        public bool? CalcularFraturasColapsosSeparadamente { get; set; }
        public int MalhaRextRint { get; set; }
        public int MalhaNunDivAng { get; set; }
        public int MalhaNunDivRad { get; set; }
        public int MalhaRMaxRMin { get; set; }
        public string PerfilANGATId { get; set; }
        public string PerfilAZTHminId { get; set; }
        public string PerfilBIOTId { get; set; }
        public string PerfilCOESAId { get; set; }
        public string PerfilDIAMBROCAId { get; set; }
        public string PerfilGPOROId { get; set; }
        public string PerfilGSOBRId { get; set; }
        public string PerfilKSId { get; set; }
        public string PerfilPERMId { get; set; }
        public string PerfilPOISSId { get; set; }
        public string PerfilPOROId { get; set; }
        public string PerfilRETRId { get; set; }
        public string PerfilTHORmaxId { get; set; }
        public string PerfilTHORminId { get; set; }
        public string PerfilUCSId { get; set; }
        public string PerfilYOUNGId { get; set; }
        public ParâmetrosPoroElásticoInput ParâmetrosPoroElástico { get; set; }

    }
}
