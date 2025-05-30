using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Correlações.Base.EditingCorrelação
{
    public interface ICorrelaçãoEmEdiçãoValidator
    {
        ValidationResult Validate(CorrelaçãoEmEdição correlaçãoEmEdição);
    }
}
