namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.ObterCorrelaçõesPossíveisConformeAsSelecionadas
{
    public class ObterCorrsPossíveisRequest
    {
        public string IdPoço { get; set; }
        public string GrupoLitológicoSelecionado { get; set; }
        public string CorrUcsSelecionada { get; set; }
        public string CorrCoesaSelecionada { get; set; }
        public string CorrAngatSelecionada { get; set; }
        public string CorrRestrSelecionada { get; set; }
    }
}
