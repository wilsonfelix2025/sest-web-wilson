using FluentValidation.Results;


namespace SestWeb.Domain.Entities.Cálculos.Filtros.MediaMovel.EmCriação
{
    public interface IFiltroMédiaMóvelEmCriaçãoValidator
    {
        ValidationResult Validate(FiltroMédiaMóvelEmCriação filtro);

    }
}
