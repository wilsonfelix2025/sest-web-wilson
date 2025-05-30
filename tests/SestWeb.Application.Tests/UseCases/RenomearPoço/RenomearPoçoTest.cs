using System;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.PoçoUseCases.RenomearPoço;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.Tests.UseCases.RenomearPoço
{
    [TestFixture]
    public class RenomearPoçoTest
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public async Task NãoDevePermitirNomeDoPoçoInválido(string nomePoço)
        {
            // Arrange
            const string id = "id";
            var poço = PoçoFactory.CriarPoço("Poço", "Poço", TipoPoço.Projeto);

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(id)).Returns(poço);

            var alterarPoçoUseCase = new RenomearPoçoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await alterarPoçoUseCase.Execute(id, nomePoço);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(RenomearPoçoStatus.PoçoNãoRenomeado);
            Check.That(result.Mensagem).IsEqualTo("Não foi possível renomear o poço. ");
        }

        [Test]
        public async Task DeveRetornarPoçoNãoAtualizadoSeHouveExceção()
        {
            // Arrange
            const string id = "id";
            const string nomePoço = "NovoNome";
            const string mensagemErro = "mensagem de erro.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored))
                .ThrowsAsync(new Exception(mensagemErro));

            var alterarPoçoUseCase = new RenomearPoçoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await alterarPoçoUseCase.Execute(id, nomePoço);

            // Assert
            Check.That(result.Status).IsEqualTo(RenomearPoçoStatus.PoçoNãoRenomeado);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível renomear o poço. {mensagemErro}");
        }

        [Test]
        public async Task DeveRetornarPoçoNãoEncontradoSeNãoExistePoço()
        {
            // Arrange
            const string id = "id";
            const string nomePoço = "NovoNome";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored))
                .Returns(Task.FromResult<Poço>(null));

            var alterarPoçoUseCase = new RenomearPoçoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await alterarPoçoUseCase.Execute(id, nomePoço);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(RenomearPoçoStatus.PoçoNãoEncontrado);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
        }

        [Test]
        public async Task DeveRetornarPoçoAtualizadoSePoçoFoiAlterado()
        {
            // Arrange
            const string id = "id";
            var poço = PoçoFactory.CriarPoço("Poço", "Poço", TipoPoço.Projeto);
            const string nomePoço = "NovoNome";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(id)).Returns(poço);
            A.CallTo(() => poçoWriteOnlyRepository.AtualizarPoço(A<Poço>.Ignored)).Returns(true);

            var alterarPoçoUseCase = new RenomearPoçoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await alterarPoçoUseCase.Execute(id, nomePoço);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(RenomearPoçoStatus.PoçoRenomeado);
        }

        [Test]
        public async Task NãoDeveAlterarPoçoSeJáExistePoçoComMesmoNome()
        {
            // Arrange
            const string id = "id";
            const string nomePoço = "NovoNome";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoçoComMesmoNome(nomePoço)).Returns(true);

            var alterarPoçoUseCase = new RenomearPoçoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await alterarPoçoUseCase.Execute(id, nomePoço);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(RenomearPoçoStatus.PoçoNãoRenomeado);
            Check.That(result.Mensagem).IsEqualTo("Não foi possível criar poço. Já existe poço com esse nome.");
        }

        [Test]
        public async Task SeTudoOkEntãoDevePassarUmPoçoComNovoNomeParaRepositórioUmaVez()
        {
            // Arrange
            const string id = "id";
            var poço = PoçoFactory.CriarPoço("Poço", "Poço", TipoPoço.Projeto);
            const string nomePoço = "NovoNome";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(id)).Returns(poço);

            var alterarPoçoUseCase = new RenomearPoçoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            await alterarPoçoUseCase.Execute(id, nomePoço);

            // Assert
            A.CallTo(() => poçoWriteOnlyRepository.AtualizarPoço(A<Poço>.That.Matches(p => p.Nome == nomePoço)))
                .MustHaveHappenedOnceExactly();
        }
    }
}