using NFluent;
using NUnit.Framework;
using FakeItEasy;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Tests.LitologiaTests
{
    [TestFixture]
    public class LitologiaTests
    {
        private Litologia _sut;

        [SetUp]
        public void Setup()
        {
            var traj = A.Fake<IConversorProfundidade>();
            _sut = new Litologia(TipoLitologia.Adaptada, traj);
        }

        [Test]
        public void DeveTerId()
        {
            Check.That(_sut.Id).Not.IsDefaultValue();
        }

        [Test]
        public void DeveObterClassificação()
        {
            Check.That(_sut.Classificação).IsEqualTo(TipoLitologia.Adaptada);
        }

        [Test]
        public void DeveObterTodosOsPontosDeLitologia()
        {

            var traj = A.Fake<IConversorProfundidade>();
            _sut.AddPontoEmPm(traj, 10, "FLH", TipoProfundidade.PM, OrigemPonto.Importado);
            _sut.AddPontoEmPm(traj, 100, "ARG", TipoProfundidade.PM, OrigemPonto.Importado);
            _sut.AddPontoEmPm(traj, 200, "FLT", TipoProfundidade.PM, OrigemPonto.Importado);
            _sut.AddPontoEmPm(traj, 300, "GRS", TipoProfundidade.PM, OrigemPonto.Importado);
            _sut.AddPontoEmPm(traj, 400, "HAL", TipoProfundidade.PM, OrigemPonto.Importado);

            var result = _sut.GetPontos();

            Check.That(result).Not.IsNullOrEmpty();
            Check.That(result).CountIs(5);
        }

        [TestCase(10, 57)]
        [TestCase(100, 55)]
        [TestCase(200, 72)]
        [TestCase(300, 15)]
        [TestCase(400, 85)]
        public void DeveObterPontoLitologiaExato(double pm, int tipoRocha)
        {
            var traj = A.Fake<IConversorProfundidade>();
            _sut.AddPontoEmPm(traj, 10, "FLH", TipoProfundidade.PM, OrigemPonto.Importado);
            _sut.AddPontoEmPm(traj, 100, "ARG", TipoProfundidade.PM, OrigemPonto.Importado);
            _sut.AddPontoEmPm(traj, 200, "FLT", TipoProfundidade.PM, OrigemPonto.Importado);
            _sut.AddPontoEmPm(traj, 300, "GRS", TipoProfundidade.PM, OrigemPonto.Importado);
            _sut.AddPontoEmPm(traj, 400, "HAL", TipoProfundidade.PM, OrigemPonto.Importado);

            var result = _sut.TryGetPontoEmPm(traj,new Profundidade(pm), out var pontoLito);

            Check.That(result).IsTrue();
            Check.That(pontoLito).IsNotNull();
            Check.That(pontoLito.Pm).IsEqualTo(pm);
            Check.That(pontoLito.TipoRocha.Numero).IsEqualTo(tipoRocha);
        }

        [TestCase(15, 57)]
        [TestCase(150, 55)]
        [TestCase(250, 72)]
        [TestCase(350, 15)]
        [TestCase(400, 85)]
        public void DeveObterValorIntermediário(double pm, int tipoRocha)
        {
            var traj = A.Fake<IConversorProfundidade>();
            _sut.AddPontoEmPm(traj, 10, "FLH", TipoProfundidade.PM, OrigemPonto.Importado);
            _sut.AddPontoEmPm(traj, 100, "ARG", TipoProfundidade.PM, OrigemPonto.Importado);
            _sut.AddPontoEmPm(traj, 200, "FLT", TipoProfundidade.PM, OrigemPonto.Importado);
            _sut.AddPontoEmPm(traj, 300, "GRS", TipoProfundidade.PM, OrigemPonto.Importado);
            _sut.AddPontoEmPm(traj, 400, "HAL", TipoProfundidade.PM, OrigemPonto.Importado);

            var result = _sut.TryGetPontoEmPm(traj, new Profundidade(pm), out var pontoLito);
            Check.That(result).IsTrue();
            Check.That(pontoLito).IsNotNull();
            Check.That(pontoLito.Pm).IsEqualTo(pm);
            Check.That(pontoLito.TipoRocha.Numero).IsEqualTo(tipoRocha);
        }
    }
}