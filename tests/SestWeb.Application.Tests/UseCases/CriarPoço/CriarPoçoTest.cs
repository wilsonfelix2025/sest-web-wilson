using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.PoçoUseCases.CriarPoço;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.Poço;

namespace SestWeb.Application.Tests.UseCases.CriarPoço
{
    [TestFixture]
    public class CriarPoçoTest
    {
        [TestCase(null, TipoPoço.Projeto)]
        [TestCase("", TipoPoço.Projeto)]
        [TestCase(" ", TipoPoço.Projeto)]
        public async Task NãoDevePermitirNomeDoPoçoInválido(string nome, TipoPoço tipoPoço)
        {
            // Arrange
            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            var criarPoçoUseCase = new CriarPoçoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await criarPoçoUseCase.Execute(nome, tipoPoço);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarPoçoStatus.PoçoNãoCriado);
            Check.That(result.Mensagem).IsEqualTo("Não foi possível criar poço. Nome do poço inválido.");
        }

        [Test]
        public async Task SeTudoOkEntãoDevePassarUmPoçoParaRepositórioUmaVez()
        {
            // Arrange
            const string nome = "Poço1";
            const TipoPoço tipoPoço = TipoPoço.Projeto;

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            var criarPoçoUseCase = new CriarPoçoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            await criarPoçoUseCase.Execute(nome, tipoPoço);

            // Assert
            A.CallTo(() => poçoWriteOnlyRepository.CriarPoço(A<Poço>.That.Matches(p => p.Nome == nome && p.TipoPoço == tipoPoço))).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task DeveRetornarPoçoOutputSePoçoFoiCriado()
        {
            // Arrange
            const string nome = "Poço1";
            const TipoPoço tipoPoço = TipoPoço.Projeto;

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            var criarPoçoUseCase = new CriarPoçoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await criarPoçoUseCase.Execute(nome, tipoPoço);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarPoçoStatus.PoçoCriado);
            Check.That(result.Poço.Id).IsNotNull().And.IsNotEmpty();
            Check.That(result.Poço.Nome).IsEqualTo(nome);
            Check.That(result.Poço.TipoPoço).IsEqualTo(tipoPoço);
        }

        [Test]
        public async Task NãoDeveCriarPoçoSeJáExistePoçoComMesmoNome()
        {
            // Arrange
            const string nome = "Poço1";
            const TipoPoço tipoPoço = TipoPoço.Projeto;

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoçoComMesmoNome(nome)).Returns(Task.FromResult(true));

            var criarPoçoUseCase = new CriarPoçoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await criarPoçoUseCase.Execute(nome, tipoPoço);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarPoçoStatus.PoçoNãoCriado);
            Check.That(result.Mensagem).IsEqualTo("Não foi possível criar poço. Já existe poço com esse nome.");
        }
    }
}