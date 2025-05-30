using FluentValidation.Results;

namespace SestWeb.Domain.Entities.Correlações.Base.Factory
{
    public interface ICorrelaçãoFactory
    {
        ValidationResult CreateCorrelação(string nome, string nomeAutor, string chaveAutor, string descrição,
            string origem, string expression, out ICorrelação correlação);

        ValidationResult UpdateCorrelação(string nome, string nomeAutor, string chaveAutor,
            string descrição, string origem, string expression, out ICorrelação correlação);
    }
}
