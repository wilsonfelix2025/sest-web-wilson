using System;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.Base.Factory;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;

namespace SestWeb.Domain.Entities.Correlações.PoçoCorrelação.Factory
{
    public class CorrelaçãoPoçoFactory : ICorrelaçãoPoçoFactory
    {
        private readonly ICorrelaçãoFactory _correlaçãoFactory;
        private static Func<string, ICorrelação, CorrelaçãoPoço> _ctorCaller;

        public CorrelaçãoPoçoFactory(ICorrelaçãoFactory correlaçãoFactory)
        {
            _correlaçãoFactory = correlaçãoFactory;
        }

        public ValidationResult CreateCorrelação(string idPoço, string nome, string nomeAutor, string chaveAutor,
            string descrição, string expressão, out ICorrelaçãoPoço correlaçãoPoço)
        {
            var result = _correlaçãoFactory
                .CreateCorrelação(nome, nomeAutor, chaveAutor, descrição, Origem.Poço.ToString(), expressão, out ICorrelação correlação);

            if (!result.IsValid)
            {
                correlaçãoPoço = default;
                return result;
            }

            CorrelaçãoPoço.RegisterCorrelaçãoPoçoCtor();
            correlaçãoPoço = _ctorCaller(idPoço, correlação);
            return result;
        }

        public ValidationResult UpdateCorrelação(string idPoço, string nome, string nomeAutor, string chaveAutor,
            string descrição, string origem, string expressão, out ICorrelaçãoPoço correlaçãoPoço)
        {
            var result = _correlaçãoFactory.UpdateCorrelação(nome, nomeAutor, chaveAutor, descrição, origem, expressão,
                out ICorrelação correlação);

            if (!result.IsValid)
            {
                correlaçãoPoço = default;
                return result;
            }

            CorrelaçãoPoço.RegisterCorrelaçãoPoçoCtor();
            correlaçãoPoço = _ctorCaller(idPoço, correlação);
            return result;
        }

        public static void RegisterCorrelaçãoPoçoCtor(Func<string, ICorrelação, CorrelaçãoPoço> ctorCaller)
        {
            _ctorCaller = ctorCaller;
        }
    }
}
