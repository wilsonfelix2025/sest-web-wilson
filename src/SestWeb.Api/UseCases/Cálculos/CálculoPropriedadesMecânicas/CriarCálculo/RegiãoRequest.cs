using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.CriarCálculo
{
    public class RegiãoRequest
    {
        public string GrupoLitológico { get; set; }
        public string UCS { get; set; }
        public string COESA { get; set; }
        public string ANGAT { get; set; }
        public string RESTR { get; set; }
        public string BIOT { get; set; }

    }
}