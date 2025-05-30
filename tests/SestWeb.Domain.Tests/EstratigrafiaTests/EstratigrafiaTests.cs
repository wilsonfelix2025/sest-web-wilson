using System.Collections.Generic;
using System.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.Poço.EstratigrafiaDoPoço;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Tests.EstratigrafiaTests
{
    [TestFixture]
    public class EstratigrafiaTests
    {
        private Estratigrafia _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new Estratigrafia();
        }

        [Test]
        public void DeveAdicionarItemDeEstratigrafia()
        {
            // Arrange
            Check.That(_sut.ObterItensEstratigrafia().Any()).IsFalse();

            var itens = new List<(string tipo, Profundidade pm, string sigla, string descrição, string idade)>
            {
                (tipo: "FM", pm: new Profundidade(1000.0), sigla: "sigla1", descrição: "descrição1", idade: "idade1"),
                (tipo: "MB", pm: new Profundidade(1000.0), sigla: "sigla2", descrição: "descrição2", idade: "idade2"),
                (tipo: "FM", pm: new Profundidade(3000.0), sigla: "sigla3", descrição: "descrição3", idade: "idade3")
            };

            foreach (var (tipo, pm, sigla, descrição, idade) in itens)
            {
                _sut.CriarItemEstratigrafia(null, tipo, pm, sigla, descrição, idade);
            }

            // Act
            var result = _sut.ObterItensEstratigrafia();

            // Assert
            Check.That(result).Not.IsNullOrEmpty();
            Check.That(result.Keys).CountIs(2);
            Check.That(result.Values.ElementAt(0)).CountIs(2);
            Check.That(result.Values.ElementAt(1)).CountIs(1);
        }

        [Test]
        public void NãoDeveAdicionarItemDeEstratigrafiaSeJáExiste()
        {
            // Arrange
            Check.That(_sut.ObterItensEstratigrafia().Any()).IsFalse();

            var itens = new List<(string tipo, Profundidade pm, string sigla, string descrição, string idade)>
            {
                (tipo: "FM", pm: new Profundidade(1000.0), sigla: "sigla1", descrição: "descrição1", idade: "idade1"),
                (tipo: "MB", pm: new Profundidade(1000.0), sigla: "sigla2", descrição: "descrição2", idade: "idade2"),
                (tipo: "FM", pm: new Profundidade(3000.0), sigla: "sigla3", descrição: "descrição3", idade: "idade3")
            };

            foreach (var (tipo, pm, sigla, descrição, idade) in itens)
            {
                _sut.CriarItemEstratigrafia(null, tipo, pm, sigla, descrição, idade);
            }

            var item1 = itens[0];

            // Act
            var result = _sut.CriarItemEstratigrafia(null, item1.tipo, item1.pm, item1.sigla, item1.descrição, item1.idade);

            // Assert
            Check.That(result).IsFalse();
        }

        [Test]
        public void DeveObterItemDeEstratigrafiaPelaProfundidadeMedida()
        {
            // Arrange
            Check.That(_sut.ObterItensEstratigrafia().Any()).IsFalse();

            var itens = new List<(string tipo, Profundidade pm, string sigla, string descrição, string idade)>
            {
                (tipo: "FM", pm: new Profundidade(1000.0), sigla: "sigla1", descrição: "descrição1", idade: "idade1"),
                (tipo: "MB", pm: new Profundidade(1000.0), sigla: "sigla2", descrição: "descrição2", idade: "idade2"),
                (tipo: "FM", pm: new Profundidade(3000.0), sigla: "sigla3", descrição: "descrição3", idade: "idade3")
            };

            foreach (var (tipo, pm, sigla, descrição, idade) in itens)
            {
                _sut.CriarItemEstratigrafia(null, tipo, pm, sigla, descrição, idade);
            }

            // Act
            var result = _sut.TryObterItemEstratigrafia(TipoEstratigrafia.ObterPeloTipo(itens[1].tipo), itens[1].pm, out var item);

            // Assert
            Check.That(result).IsTrue();
            Check.That(item).IsNotNull();
            Check.That(item.Pm == itens[1].pm);
            Check.That(item.Sigla == itens[1].sigla);
            Check.That(item.Descrição == itens[1].descrição);
            Check.That(item.Idade == itens[1].idade);
        }

        [Test]
        public void NãoDeveObterItemDeEstratigrafiaPelaProfundidadeMedidaQueNãoExiste()
        {
            // Arrange
            Check.That(_sut.ObterItensEstratigrafia().Any()).IsFalse();

            var itens = new List<(string tipo, Profundidade pm, string sigla, string descrição, string idade)>
            {
                (tipo: "FM", pm: new Profundidade(1000.0), sigla: "sigla1", descrição: "descrição1", idade: "idade1"),
                (tipo: "MB", pm: new Profundidade(1000.0), sigla: "sigla2", descrição: "descrição2", idade: "idade2"),
                (tipo: "FM", pm: new Profundidade(3000.0), sigla: "sigla3", descrição: "descrição3", idade: "idade3")
            };

            foreach (var (tipo, pm, sigla, descrição, idade) in itens)
            {
                _sut.CriarItemEstratigrafia(null, tipo, pm, sigla, descrição, idade);
            }

            // Act
            var result = _sut.TryObterItemEstratigrafia(TipoEstratigrafia.ObterPeloTipo(itens[1].tipo), new Profundidade(5000), out var item);

            // Assert
            Check.That(result).IsFalse();
            Check.That(item).IsNull();
        }

        [Test]
        public void DeveLimparItensDeEstratigrafia()
        {
            // Arrange
            Check.That(_sut.ObterItensEstratigrafia().Any()).IsFalse();

            var itens = new List<(string tipo, Profundidade pm, string sigla, string descrição, string idade)>
            {
                (tipo: "FM", pm: new Profundidade(1000.0), sigla: "sigla1", descrição: "descrição1", idade: "idade1"),
                (tipo: "MB", pm: new Profundidade(1000.0), sigla: "sigla2", descrição: "descrição2", idade: "idade2"),
                (tipo: "FM", pm: new Profundidade(3000.0), sigla: "sigla3", descrição: "descrição3", idade: "idade3")
            };

            foreach (var (tipo, pm, sigla, descrição, idade) in itens)
            {
                _sut.CriarItemEstratigrafia(null, tipo, pm, sigla, descrição, idade);
            }

            var result = _sut.ObterItensEstratigrafia();

            Check.That(result).Not.IsNullOrEmpty();

            // Act
            _sut.ApagarEstratigrafia();

            // Assert
            var result2 = _sut.ObterItensEstratigrafia();
            Check.That(result2).IsEmpty();
        }

        [Test]
        public void DeveRemoverItemDeEstratigrafiaCasoExista()
        {
            // Arrange
            Check.That(_sut.ObterItensEstratigrafia().Any()).IsFalse();

            var itens = new List<(string tipo, Profundidade pm, string sigla, string descrição, string idade)>
            {
                (tipo: "FM", pm: new Profundidade(1000.0), sigla: "sigla1", descrição: "descrição1", idade: "idade1"),
                (tipo: "MB", pm: new Profundidade(1000.0), sigla: "sigla2", descrição: "descrição2", idade: "idade2"),
                (tipo: "FM", pm: new Profundidade(3000.0), sigla: "sigla3", descrição: "descrição3", idade: "idade3")
            };

            foreach (var (tipo, pm, sigla, descrição, idade) in itens)
            {
                _sut.CriarItemEstratigrafia(null, tipo, pm, sigla, descrição, idade);
            }

            var result = _sut.ObterItensEstratigrafia();

            Check.That(result).Not.IsNullOrEmpty();

            // Act
            var result2 = _sut.RemoverItemEstratigrafia(itens[1].tipo, itens[1].pm);

            // Assert
            Check.That(result2).IsTrue();

            var result3 = _sut.ObterItensEstratigrafia();
            Check.That(result3.ContainsKey(itens[1].tipo)).IsFalse();
        }

        [Test]
        public void NãoDeveRemoverItemDeEstratigrafiaCasoNãoExistaTipo()
        {
            // Arrange
            Check.That(_sut.ObterItensEstratigrafia().Any()).IsFalse();

            var itens = new List<(string tipo, Profundidade pm, string sigla, string descrição, string idade)>
            {
                (tipo: "FM", pm: new Profundidade(1000.0), sigla: "sigla1", descrição: "descrição1", idade: "idade1"),
                (tipo: "MB", pm: new Profundidade(1000.0), sigla: "sigla2", descrição: "descrição2", idade: "idade2"),
                (tipo: "FM", pm: new Profundidade(3000.0), sigla: "sigla3", descrição: "descrição3", idade: "idade3")
            };

            foreach (var (tipo, pm, sigla, descrição, idade) in itens)
            {
                _sut.CriarItemEstratigrafia(null, tipo, pm, sigla, descrição, idade);
            }

            var result = _sut.ObterItensEstratigrafia();

            Check.That(result).Not.IsNullOrEmpty();

            // Act
            var result2 = _sut.RemoverItemEstratigrafia("CR", new Profundidade(1000.0));

            // Assert
            Check.That(result2).IsFalse();
        }

        [Test]
        public void NãoDeveRemoverItemDeEstratigrafiaCasoNãoExistaProfundidade()
        {
            // Arrange
            Check.That(_sut.ObterItensEstratigrafia().Any()).IsFalse();

            var itens = new List<(string tipo, Profundidade pm, string sigla, string descrição, string idade)>
            {
                (tipo: "FM", pm: new Profundidade(1000.0), sigla: "sigla1", descrição: "descrição1", idade: "idade1"),
                (tipo: "MB", pm: new Profundidade(1000.0), sigla: "sigla2", descrição: "descrição2", idade: "idade2"),
                (tipo: "FM", pm: new Profundidade(3000.0), sigla: "sigla3", descrição: "descrição3", idade: "idade3")
            };

            foreach (var (tipo, pm, sigla, descrição, idade) in itens)
            {
                _sut.CriarItemEstratigrafia(null, tipo, pm, sigla, descrição, idade);
            }

            var result = _sut.ObterItensEstratigrafia();

            Check.That(result).Not.IsNullOrEmpty();

            // Act
            var result2 = _sut.RemoverItemEstratigrafia(itens[1].tipo, new Profundidade(5000.0));

            // Assert
            Check.That(result2).IsFalse();
        }
    }
}