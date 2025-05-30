
namespace SestWeb.Application.Helpers
{
    internal class ValidationResult
    {
        public ValidationResult(ValidationResultStatus status, string mensagem = "")
        {
            Status = status;
            Mensagem = mensagem;
        }

        public ValidationResultStatus Status { get; }
        public string Mensagem { get; }
    }
}
