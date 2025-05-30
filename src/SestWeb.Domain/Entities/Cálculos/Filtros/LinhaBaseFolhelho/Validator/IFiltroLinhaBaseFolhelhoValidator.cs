using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.LinhaBaseFolhelho.Validator
{
    public interface IFiltroLinhaBaseFolhelhoValidator
    {
        ValidationResult Validate(FiltroLinhaBaseFolhelho filtro);
    }
}
