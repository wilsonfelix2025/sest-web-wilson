using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.Simples.EmCriação
{
    public interface IFiltroSimplesEmCriaçãoValidator
    {
        ValidationResult Validate(FiltroSimplesEmCriação filtro);

    }
}
