using System;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.EstratigrafiaUseCases.AdicionarItemEstratigrafia;
using SestWeb.Domain.Entities.Poço.EstratigrafiaDoPoço;

namespace SestWeb.Application.Tests.UseCases.AdicionarItemEstratigrafia
{
    [TestFixture]
    public class AdicionarItemEstratigrafiaTests
    {
        [Test]
        public async Task DeveAdicionarItemEstratigrafiaComSucesso()
        {
            // Arrange
            const string idPoço = "id";
            const double pm = 1000.0;
            const string sigla = "sigla";
            const string descrição = "descrição";
            const string tipo = "FM";
            const string idade = "idade";
            var input = new AdicionarItemEstratigrafiaInput(idPoço, tipo, pm, sigla, descrição, idade);
            var estratigrafia = new Estratigrafia();

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterEstratigrafia(idPoço)).Returns(estratigrafia);
            A.CallTo(() => poçoWriteOnlyRepository.AtualizarEstratigrafia(idPoço, estratigrafia)).Returns(true);

            var adicionarItemEstratigrafiaUseCase = new AdicionarItemEstratigrafiaUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await adicionarItemEstratigrafiaUseCase.Execute(input);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(AdicionarItemEstratigrafiaStatus.ItemAdicionado);
            Check.That(result.Mensagem).IsEqualTo("Item de estratigrafia adicionado com sucesso.");
        }

        [Test]
        public async Task NãoConseguiuObterEstratigrafiaDoPoço()
        {
            // Arrange
            const string idPoço = "id";
            const double pm = 1000.0;
            const string sigla = "sigla";
            const string descrição = "descrição";
            const string tipo = "FM";
            const string idade = "idade";
            var input = new AdicionarItemEstratigrafiaInput(idPoço, tipo, pm, sigla, descrição, idade);
            
            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterEstratigrafia(idPoço)).Returns(Task.FromResult<Estratigrafia>(null));
            A.CallTo(() => poçoWriteOnlyRepository.AtualizarEstratigrafia(idPoço, A<Estratigrafia>.Ignored)).Returns(true);

            var adicionarItemEstratigrafiaUseCase = new AdicionarItemEstratigrafiaUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await adicionarItemEstratigrafiaUseCase.Execute(input);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(AdicionarItemEstratigrafiaStatus.ItemNãoAdicionado);
            Check.That(result.Mensagem).IsEqualTo($"[AdicionarItemEstratigrafia] - Não foi possível obter estratigrafia do poço {idPoço}.");
        }

        [Test]
        public async Task NãoConseguiuAtualizarEstratigrafia()
        {
            // Arrange
            const string idPoço = "id";
            const double pm = 1000.0;
            const string sigla = "sigla";
            const string descrição = "descrição";
            const string tipo = "FM";
            const string idade = "idade";
            var input = new AdicionarItemEstratigrafiaInput(idPoço, tipo, pm, sigla, descrição, idade);
            var estratigrafia = new Estratigrafia();

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterEstratigrafia(idPoço)).Returns(estratigrafia);
            A.CallTo(() => poçoWriteOnlyRepository.AtualizarEstratigrafia(idPoço, A<Estratigrafia>.Ignored)).Returns(false);

            var adicionarItemEstratigrafiaUseCase = new AdicionarItemEstratigrafiaUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await adicionarItemEstratigrafiaUseCase.Execute(input);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(AdicionarItemEstratigrafiaStatus.ItemNãoAdicionado);
            Check.That(result.Mensagem).IsEqualTo($"[AdicionarItemEstratigrafia] - Não foi possível adicionar item de estratigrafia com PM = {input.PM}");
        }

        [Test]
        public async Task OcorreuExceçãoDuranteAtualizaçãoEstratigrafia()
        {
            // Arrange
            const string idPoço = "id";
            const double pm = 1000.0;
            const string sigla = "sigla";
            const string descrição = "descrição";
            const string tipo = "FM";
            const string idade = "idade";
            var input = new AdicionarItemEstratigrafiaInput(idPoço, tipo, pm, sigla, descrição, idade);
            var estratigrafia = new Estratigrafia();
            const string mensagem = "Mensagem de erro.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterEstratigrafia(idPoço)).Returns(estratigrafia);
            A.CallTo(() => poçoWriteOnlyRepository.AtualizarEstratigrafia(idPoço, A<Estratigrafia>.Ignored)).ThrowsAsync(new Exception(mensagem));

            var adicionarItemEstratigrafiaUseCase = new AdicionarItemEstratigrafiaUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await adicionarItemEstratigrafiaUseCase.Execute(input);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(AdicionarItemEstratigrafiaStatus.ItemNãoAdicionado);
            Check.That(result.Mensagem).IsEqualTo($"[AdicionarItemEstratigrafia] - {mensagem}");
        }
    }
}