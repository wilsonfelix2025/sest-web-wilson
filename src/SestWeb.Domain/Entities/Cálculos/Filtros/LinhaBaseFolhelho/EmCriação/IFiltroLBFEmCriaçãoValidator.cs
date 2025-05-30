using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.LinhaBaseFolhelho.EmCriação
{
    public interface IFiltroLBFEmCriaçãoValidator
    {
        ValidationResult Validate(FiltroLBFEmCriação filtro);
    }
}
