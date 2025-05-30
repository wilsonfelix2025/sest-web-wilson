using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.Simples.Validator
{
    public interface IFiltroSimplesValidator
    {
        ValidationResult Validate(FiltroSimples filtro);
    }
}
