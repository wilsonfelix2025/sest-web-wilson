using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.Litologia.Validator
{
    public interface IFiltroLitologiaValidator
    {
        ValidationResult Validate(FiltroLitologia filtro);
    }
}
