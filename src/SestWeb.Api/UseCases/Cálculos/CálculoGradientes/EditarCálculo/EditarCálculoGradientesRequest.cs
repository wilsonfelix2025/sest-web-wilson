namespace SestWeb.Api.UseCases.Cálculos.CálculoGradientes.EditarCálculo
{
    public class EditarCálculoGradientesRequest
    {
        public string IdPoço { get; set; }
        public string IdCálculo { get; set; }
        public string NomeCálculo { get; set; }
        public string TipoModelo { get; set; }
        public bool? FluidoPenetrante { get; set; }
        public double AreaPlastificada { get; set; }
        public string TipoCritérioRuptura { get; set; }
        public int? Tempo { get; set; }
        public bool? HabilitarFiltroAutomatico { get; set; }
        public bool? IncluirEfeitosFísicosQuímicos { get; set; } = false;
        public bool? IncluirEfeitosTérmicos { get; set; } = false;
        public bool? CalcularFraturasColapsosSeparadamente { get; set; }
        public int MalhaRextRint { get; set; }
        public int MalhaNunDivAng { get; set; }
        public int MalhaNunDivRad { get; set; }
        public int MalhaRMaxRMin { get; set; }
        public string PerfilANGATId { get; set; }
        public string PerfilAZTHminId { get; set; }
        public string PerfilBIOTId { get; set; }
        public string PerfilCOESAId { get; set; }
        public string PerfilDIAM_BROCAId { get; set; }
        public string PerfilGPOROId { get; set; }
        public string PerfilGSOBRId { get; set; }
        public string PerfilKSId { get; set; }
        public string PerfilPERMId { get; set; }
        public string PerfilPOISSId { get; set; }
        public string PerfilPOROId { get; set; }
        public string PerfilRESTRId { get; set; }
        public string PerfilTHORmaxId { get; set; }
        public string PerfilTHORminId { get; set; }
        public string PerfilUCSId { get; set; }
        public string PerfilYOUNGId { get; set; }
        public ParâmetrosPoroElásticoRequest ParâmetrosPoroElástico { get; set; } = new ParâmetrosPoroElásticoRequest();
        public double TemperaturaFormaçãoFisicoQuimica { get; internal set; }
    }
}
