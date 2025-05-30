using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Correlações.PoçoCorrelação.Factory
{
    public interface ICorrelaçãoPoçoFactory
    {
        ValidationResult CreateCorrelação(string idPoço, string nome, string nomeAutor, string chaveAutor,
            string descrição, string expressão, out ICorrelaçãoPoço correlaçãoPoço);
        ValidationResult UpdateCorrelação(string idPoço, string nome, string nomeAutor, string chaveAutor,
            string descrição, string origem, string expressão, out ICorrelaçãoPoço correlaçãoPoço);
    }
}
