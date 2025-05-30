using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Correlações.Base.Validator
{
    public interface ICorrelaçãoValidator
    {
        ValidationResult Validate(Correlação correlação);
    }
}
