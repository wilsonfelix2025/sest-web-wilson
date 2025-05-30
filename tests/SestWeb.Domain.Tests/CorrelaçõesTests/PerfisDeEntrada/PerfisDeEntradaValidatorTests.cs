using System;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.Correlações.PerfisEntradaCorrelação;
using SestWeb.Domain.Entities.Correlações.PerfisEntradaCorrelação.Validator;

namespace SestWeb.Domain.Tests.CorrelaçõesTests.PerfisDeEntrada
{
    [TestFixture]
    public class PerfisDeEntradaValidatorTests
    {
        private PerfisEntradaValidator _sut;
        private PerfisEntradaTestsHelper _perfisEntrada;

        [SetUp]
        public void Setup()
        {
            _sut = new PerfisEntradaValidator();
            _perfisEntrada = new PerfisEntradaTestsHelper(string.Empty);
        }

        [TearDown]
        public void TearDown()
        {
            _perfisEntrada?.Clear();
        }

        [Test]
        public void DeveResultarErroQuandoPerfisDeEntradaÉNull()
        {
            _perfisEntrada = null;
            var result = _sut.Validate(_perfisEntrada);
            Check.That(result.IsValid).IsFalse();
        }

        [Test]
        public void ErrorsCountDeveSer1QuandoPerfisDeEntradaÉNull()
        {
            _perfisEntrada = null;
            var result = _sut.Validate(_perfisEntrada);
            Check.That(result.Errors.Count).IsEqualTo(1);
        }

        [Test]
        public void DeveApresentarMensagemDePerfisDeEntradaÉNull()
        {
            _perfisEntrada = null;
            var result = _sut.Validate(_perfisEntrada);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo($"{typeof(PerfisEntrada).Name} não pode ser null.");
        }

        [Test]
        public void DeveInvalidarQuandoÚmPerfilDeEntradaÉNull()
        {
            _perfisEntrada.Add(null);
            var result = _sut.Validate(_perfisEntrada);
            Check.That(result.IsValid).IsFalse();
        }

        [Test]
        public void DeveApresentar2ErrosQuandoPerfilDeEntradaÉNull()
        {
            _perfisEntrada.Add(null);
            var result = _sut.Validate(_perfisEntrada);
            Check.That(result.Errors.Count).IsEqualTo(2);
        }

        [Test]
        public void DeveApresentarMsgPerfilNullQuandoPerfilDeEntradaÉNull()
        {
            _perfisEntrada.Add(null);
            var result = _sut.Validate(_perfisEntrada);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Perfil, index: 0, está null.");
        }

        [Test]
        public void DeveApresentarMsgPerfilInválidoQuandoPerfilDeEntradaÉNull()
        {
            _perfisEntrada.Add(null);
            var result = _sut.Validate(_perfisEntrada);
            Check.That(result.Errors[1].ErrorMessage).IsEqualTo("Perfil, index: 0, não é um perfil válido.");
        }
    }
}
