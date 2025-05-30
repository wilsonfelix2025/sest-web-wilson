using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.Corte.Validator
{
    public interface IFiltroCorteValidator
    {
        ValidationResult Validate(FiltroCorte filtro);
    }
}
