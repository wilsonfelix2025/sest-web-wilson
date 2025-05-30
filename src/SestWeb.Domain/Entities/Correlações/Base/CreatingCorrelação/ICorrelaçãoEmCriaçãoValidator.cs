using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Correlações.Base.CreatingCorrelação
{
    public interface ICorrelaçãoEmCriaçãoValidator
    {
        ValidationResult Validate(CorrelaçãoEmCriação correlaçãoEmCriação);
    }
}
