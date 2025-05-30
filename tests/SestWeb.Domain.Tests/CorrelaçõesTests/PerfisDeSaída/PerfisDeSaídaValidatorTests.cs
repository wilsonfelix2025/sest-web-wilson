using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.Correlações.PerfisSaídaCorrelação.Validator;

namespace SestWeb.Domain.Tests.CorrelaçõesTests.PerfisDeSaída
{
    [TestFixture]
    public class PerfisDeSaídaValidatorTests
    {
        private PerfisSaídaValidator _sut;
        private PerfisSaídaTestsHelper _perfisSaída;

        [SetUp]
        public void Setup()
        {
            _sut = new PerfisSaídaValidator();
            _perfisSaída = new PerfisSaídaTestsHelper(string.Empty);
        }

        [TearDown]
        public void TearDown()
        {
            _perfisSaída?.Clear();
        }

        [Test]
        public void DeveResultarErroQuandoPerfisDeSaídaÉNull()
        {
            _perfisSaída = null;
            var result = _sut.Validate(_perfisSaída);
            Check.That(result.IsValid).IsFalse();
        }

        [Test]
        public void ErrorsCountDeveSer1QuandoPerfisDeSaídaÉNull()
        {
            _perfisSaída = null;
            var result = _sut.Validate(_perfisSaída);
            Check.That(result.Errors.Count).IsEqualTo(1);
        }

        [Test]
        public void DeveApresentarMensagemDePerfisDeSaídaÉNull()
        {
            _perfisSaída = null;
            var result = _sut.Validate(_perfisSaída);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo($"PerfisSaída não pode ser null.");
        }

        [Test]
        public void DeveInvalidarQuandoÚmPerfilDeSaídaÉNull()
        {
            _perfisSaída.Add(null);
            var result = _sut.Validate(_perfisSaída);
            Check.That(result.IsValid).IsFalse();
        }

        [Test]
        public void DeveApresentar2ErrosQuandoPerfilDeSaídaÉNull()
        {
            _perfisSaída.Add(null);
            var result = _sut.Validate(_perfisSaída);
            Check.That(result.Errors.Count).IsEqualTo(2);
        }

        [Test]
        public void DeveApresentarMsgPerfilNullQuandoPerfilDeSaídaÉNull()
        {
            _perfisSaída.Add(null);
            var result = _sut.Validate(_perfisSaída);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Perfil, index: 0, está null.");
        }

        [Test]
        public void DeveApresentarMsgPerfilInválidoQuandoPerfilDeSaídaÉNull()
        {
            _perfisSaída.Add(null);
            var result = _sut.Validate(_perfisSaída);
            Check.That(result.Errors[1].ErrorMessage).IsEqualTo("Perfil, index: 0, não é um perfil válido.");
        }

        [Test]
        public void DeveApresentarMsgPerfilDeSaídaNãoIdentificadoQuandoNãoHáMnemônicosPerfisNaExpressão()
        {
            var result = _sut.Validate(_perfisSaída);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Perfil de Saída não identificado na expressão.");
        }
    }
}
