namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.CriarRelacionamentoCorrsPropMecPoço
{
    public class CreateWellRelationshipRequest
    {
        public string IdPoço { get; set; }
        public string GrupoLitológico { get; set; }
        public string NomeAutor { get; set; }
        public string ChaveAutor { get; set; }
        public string CorrUcs { get; set; }
        public string CorrCoesa { get; set; }
        public string CorrAngat { get; set; }
        public string CorrRestr { get; set; }
    }
}