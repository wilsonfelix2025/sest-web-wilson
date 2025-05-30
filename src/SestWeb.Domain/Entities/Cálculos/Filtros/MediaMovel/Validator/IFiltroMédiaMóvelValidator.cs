using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.MediaMovel.Validator
{
    public interface IFiltroMédiaMóvelValidator
    {
        ValidationResult Validate(FiltroMédiaMóvel filtro);
    }
}
