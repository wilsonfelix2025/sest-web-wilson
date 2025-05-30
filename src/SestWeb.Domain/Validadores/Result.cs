using FluentValidation.Results;

namespace SestWeb.Domain.Validadores
{
    public class Result
    {
        public Result()
        {
            
        }

        public ValidationResult result { get; set; }

        public object Entity { get; set; }
    }
}
