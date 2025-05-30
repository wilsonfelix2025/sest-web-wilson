
using SestWeb.Domain.Entities.Cálculos.Filtros;

namespace SestWeb.Application.UseCases.FiltrosUseCase.EditarFiltro
{
    public class EditarFiltroInput
    {
        public string IdPerfil { get; set; }
        public double? LimiteInferior { get; set; }
        public double? LimiteSuperior { get; set; }
        public TipoCorteEnum? TipoCorte { get; set; }
        public TipoFiltroEnum TipoFiltro { get; set; }
        public string Nome { get; set; }
    }
}
