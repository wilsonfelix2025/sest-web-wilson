using System;
using System.Globalization;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Correlações.AutorCorrelação;
using SestWeb.Domain.Entities.Correlações.Base.CreatingCorrelação;
using SestWeb.Domain.Entities.Correlações.Base.EditingCorrelação;
using SestWeb.Domain.Entities.Correlações.Base.Validator;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.Factory;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;

namespace SestWeb.Domain.Entities.Correlações.Base.Factory
{
    public class CorrelaçãoFactory : ICorrelaçãoFactory
    {
        private readonly IExpressãoFactory _expressãoFactory;
        private readonly ICorrelaçãoEmCriaçãoValidator _correlaçãoEmCriaçãoValidator;
        private readonly ICorrelaçãoEmEdiçãoValidator _correlaçãoEmEdiçãoValidator;
        private readonly ICorrelaçãoValidator _correlaçãoValidator;
        private static Func<string, IAutor, string, Origem, IExpressão, Correlação> _ctorCaller;

        public CorrelaçãoFactory(IExpressãoFactory expressãoFactory, ICorrelaçãoEmCriaçãoValidator correlaçãoEmCriaçãoValidator, ICorrelaçãoEmEdiçãoValidator correlaçãoEmEdiçãoValidator, ICorrelaçãoValidator correlaçãoValidator)
        {
            _expressãoFactory = expressãoFactory;
            _correlaçãoEmCriaçãoValidator = correlaçãoEmCriaçãoValidator;
            _correlaçãoEmEdiçãoValidator = correlaçãoEmEdiçãoValidator;
            _correlaçãoValidator = correlaçãoValidator;
        }
        
        public static void RegisterCorrelaçãoCtor(Func<string, IAutor, string, Origem, IExpressão, Correlação> ctorCaller)
        {
            _ctorCaller = ctorCaller;
        }

        public ValidationResult CreateCorrelação(string nome, string nomeAutor, string chaveAutor, string descrição, string origem, string expression, out ICorrelação correlação)
        {
            // Verifica se a correlação pode ser montada em segurança, mesmo que com dados inválidos.
            var result =
                EnsureCorrelaçãoCreation(nome, nomeAutor, chaveAutor, DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), descrição, origem, expression);

            if (!result.IsValid)
            {
                correlação = default;
                return result;
            }

            // Monta as dependências
            var expressão = _expressãoFactory.CreateExpressão(expression);
            Origem origin = (Origem)Enum.Parse(typeof(Origem), origem, true);
            var autor = Autor.GetFactory().CreateAutor(nomeAutor,chaveAutor, DateTime.Now);

            // Solicita o registro do construtor
            Correlação.RegisterCorrelaçãoCtor();

            // Chama o construtor
            correlação = _ctorCaller(nome, autor, descrição, origin, expressão);

            // Valida a entidade 
            return _correlaçãoValidator.Validate((Correlação)correlação);
        }

        public ValidationResult UpdateCorrelação(string nome, string nomeAutor, string chaveAutor,
            string descrição, string origem, string expression, out ICorrelação correlação)
        {
            var result = EnsureCorrelaçãoUpdate(origem);
            if (!result.IsValid)
            {
                correlação = default;
                return result;
            }

            return CreateCorrelação(nome, nomeAutor, chaveAutor, descrição, origem, expression, out correlação);
        }

        private ValidationResult EnsureCorrelaçãoCreation(string nome, string nomeAutor, string chaveAutor,
            string dataCriação,
            string descrição, string origem, string expression)
        {
            CorrelaçãoEmCriação correlaçãoEmCriação =
                new CorrelaçãoEmCriação(nome, nomeAutor, chaveAutor, dataCriação, descrição, origem, expression);

            return _correlaçãoEmCriaçãoValidator.Validate(correlaçãoEmCriação);
        }

        private ValidationResult EnsureCorrelaçãoUpdate(string origem)
        {
            CorrelaçãoEmEdição correlaçãoEmEdição = new CorrelaçãoEmEdição(origem);
            return _correlaçãoEmEdiçãoValidator.Validate(correlaçãoEmEdição);
        }
    }
}
