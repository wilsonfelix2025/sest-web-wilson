
using SestWeb.Domain.Entities.Cálculos.Filtros;

namespace SestWeb.Api.UseCases.Filtros.EditarFiltro
{
    public class EditarFiltroRequest
    {
        public string IdPerfil { get; set; }
        public double? LimiteInferior { get; set; }
        public double? LimiteSuperior { get; set; }
        public TipoCorteEnum? TipoCorte { get; set; }
        public string Nome { get; set; }
    }
}
