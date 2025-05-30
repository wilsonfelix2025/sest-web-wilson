using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.Factory;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.Validator;
using SestWeb.Domain.Tests.Helpers;

namespace SestWeb.Domain.Tests.CorrelaçõesTests.Expression
{
    public class ExpressionValidatorTests : IoCSupportedTest<DomainModule>
    {
        private IExpressãoFactory _expressãoFactory;
        private ExpressionValidator _sut;

        [OneTimeSetUp]
        public void Setup()
        {
            _expressãoFactory = Resolve<IExpressãoFactory>();
            _sut = (ExpressionValidator)Resolve<IExpressionValidator>();
        }

        [Test]
        public void DeveInvalidarCasoExistaUmaConstanteEUmaVariávelComOMesmoNome()
        {
            var expressão = _expressãoFactory.CreateExpressão("var sameNameTest = 1, const sameNameTest = 2");
            var result =_sut.Validate(expressão);
            Check.That(result.IsValid).IsFalse();
            Check.That(result.Errors[1].ErrorMessage).IsEqualTo($"A expressão \"{expressão.Bruta}\" contém constantes e variáveis com o mesmo nome: sameNameTest.");
        }

        [Test]
        public void DeveInvalidarCasoExistaNConstantesEVariáveisComOMesmoNome()
        {
            var expressão = _expressãoFactory.CreateExpressão("var sameNameTest1 = 1, var sameNameTest2 = 2, var sameNameTest3 = 3,const sameNameTest1 = 1,const sameNameTest2 = 2,const sameNameTest3 = 3");
            var result = _sut.Validate(expressão);
            Check.That(result.IsValid).IsFalse();
            Check.That(result.Errors[1].ErrorMessage).IsEqualTo($"A expressão \"{expressão.Bruta}\" contém constantes e variáveis com o mesmo nome: sameNameTest1, sameNameTest2, sameNameTest3.");
        }
    }
}
