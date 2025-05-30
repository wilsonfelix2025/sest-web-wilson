using System;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.EstratigrafiaUseCases.RemoverItemEstratigrafia;
using SestWeb.Domain.Entities.Poço.EstratigrafiaDoPoço;
using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Application.Tests.UseCases.RemoverItemEstratigrafia
{
    [TestFixture]
    public class RemoverItemEstratigrafiaTests
    {
        [Test]
        public async Task DeveRemoverItemEstratigrafiaComSucesso()
        {
            // Arrange
            const string idPoço = "id";
            const string tipo = "FM";
            Profundidade pm = new Profundidade(1000.0);
            const string sigla = "sigla";
            const string descrição = "descrição";
            const string idade = "idade";
            var estratigrafia = new Estratigrafia();
            estratigrafia.CriarItemEstratigrafia(null, tipo, pm, sigla, descrição, idade);

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterEstratigrafia(idPoço)).Returns(estratigrafia);
            A.CallTo(() => poçoWriteOnlyRepository.AtualizarEstratigrafia(idPoço, estratigrafia)).Returns(true);

            var removerItemEstratigrafiaUseCase =
                new RemoverItemEstratigrafiaUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await removerItemEstratigrafiaUseCase.Execute(idPoço, tipo, pm.Valor);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(RemoverItemEstratigrafiaStatus.ItemEstratigrafiaRemovido);
            Check.That(result.Mensagem).IsEqualTo("Item de estratigrafia removido com sucesso.");
        }

        [Test]
        public async Task NãoConseguiuAtualizarEstratigrafiaNoDomínio()
        {
            // Arrange
            const string idPoço = "id";
            const string tipo = "FM";
            Profundidade pm = new Profundidade(1000.0);
            const string sigla = "sigla";
            const string descrição = "descrição";
            const string idade = "idade";
            var estratigrafia = new Estratigrafia();
            estratigrafia.CriarItemEstratigrafia(null, tipo, pm, sigla, descrição, idade);

            const double outroPm = 2000.0;

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterEstratigrafia(idPoço)).Returns(estratigrafia);

            var removerItemEstratigrafiaUseCase =
                new RemoverItemEstratigrafiaUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await removerItemEstratigrafiaUseCase.Execute(idPoço, tipo, outroPm);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(RemoverItemEstratigrafiaStatus.ItemEstratigrafiaNãoRemovido);
            Check.That(result.Mensagem)
                .IsEqualTo(
                    $"[RemoverItemEstratigrafia] - Não foi possível remover item de estratigrafia com tipo = {TipoEstratigrafia.ObterPeloTipo(tipo).Nome} e PM = {outroPm}.");
        }

        [Test]
        public async Task NãoConseguiuAtualizarEstratigrafiaNoBanco()
        {
            // Arrange
            const string idPoço = "id";
            const string tipo = "FM";
            Profundidade pm = new Profundidade(1000.0);
            const string sigla = "sigla";
            const string descrição = "descrição";
            const string idade = "idade";
            var estratigrafia = new Estratigrafia();
            estratigrafia.CriarItemEstratigrafia(null, tipo, pm, sigla, descrição, idade);

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterEstratigrafia(idPoço)).Returns(estratigrafia);
            A.CallTo(() => poçoWriteOnlyRepository.AtualizarEstratigrafia(idPoço, A<Estratigrafia>.Ignored))
                .Returns(false);

            var removerItemEstratigrafiaUseCase =
                new RemoverItemEstratigrafiaUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await removerItemEstratigrafiaUseCase.Execute(idPoço, tipo, pm.Valor);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(RemoverItemEstratigrafiaStatus.ItemEstratigrafiaNãoRemovido);
            Check.That(result.Mensagem)
                .IsEqualTo(
                    $"[RemoverItemEstratigrafia] - Não foi possível remover item de estratigrafia com tipo = {TipoEstratigrafia.ObterPeloTipo(tipo).Nome} e PM = {pm} do banco.");
        }

        [Test]
        public async Task NãoConseguiuObterEstratigrafiaDoPoço()
        {
            // Arrange
            const string idPoço = "id";
            const string tipo = "FM";
            const double pm = 1000.0;

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterEstratigrafia(idPoço))
                .Returns(Task.FromResult<Estratigrafia>(null));

            var removerItemEstratigrafiaUseCase =
                new RemoverItemEstratigrafiaUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await removerItemEstratigrafiaUseCase.Execute(idPoço, tipo, pm);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(RemoverItemEstratigrafiaStatus.ItemEstratigrafiaNãoRemovido);
            Check.That(result.Mensagem)
                .IsEqualTo($"[RemoverItemEstratigrafia] - Não foi possível obter estratigrafia do poço {idPoço}.");
        }

        [Test]
        public async Task OcorreuExceçãoDuranteAtualizaçãoEstratigrafia()
        {
            // Arrange
            const string idPoço = "id";
            const string tipo = "FM";
            Profundidade pm = new Profundidade(1000.0);
            const string sigla = "sigla";
            const string descrição = "descrição";
            const string idade = "idade";
            var estratigrafia = new Estratigrafia();
            estratigrafia.CriarItemEstratigrafia(null, tipo, pm, sigla, descrição, idade);

            const string mensagem = "Mensagem de erro";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterEstratigrafia(idPoço)).Returns(estratigrafia);
            A.CallTo(() => poçoWriteOnlyRepository.AtualizarEstratigrafia(idPoço, A<Estratigrafia>.Ignored))
                .ThrowsAsync(new Exception(mensagem));

            var removerItemEstratigrafiaUseCase =
                new RemoverItemEstratigrafiaUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await removerItemEstratigrafiaUseCase.Execute(idPoço, tipo, pm.Valor);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(RemoverItemEstratigrafiaStatus.ItemEstratigrafiaNãoRemovido);
            Check.That(result.Mensagem).IsEqualTo($"[RemoverItemEstratigrafia] - {mensagem}");
        }
    }
}