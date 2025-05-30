using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.Litologia.EmCriação
{
    public interface IFiltroLitologiaEmCriaçãoValidator
    {
        ValidationResult Validate(FiltroLitologiaEmCriação filtro);
    }
}
