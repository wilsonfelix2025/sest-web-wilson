
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.CriarCálculo
{
    public class RegiõesInput
    {
        public string GrupoLitológico { get; set; }
        public string UCSCorrelação { get; set; }
        public string CoesãoCorrelação { get; set; }
        public string ANGATCorrelação { get; set; }
        public string RESTRCorrelação { get; set; }
        public string BIOTCorrelação { get; set; }
    }
}
