using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.Corte.EmCriação
{
    public interface IFiltroCorteEmCriaçãoValidator
    {
        ValidationResult Validate(FiltroCorteEmCriação filtro);
    }
}
