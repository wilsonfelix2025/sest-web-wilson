using System;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.PoçoUseCases.RemoverPoço;

namespace SestWeb.Application.Tests.UseCases.RemoverPoço
{
    [TestFixture]
    public class RemoverPoçoTest
    {
        [Test]
        public void DeveRetornarPoçoNãoEncontradoSePoçoNãoExiste()
        {
            // Arrange
            const string id = "id";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(false);
            
            var removerPoçoUseCase = new RemoverPoçoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = removerPoçoUseCase.Execute(id);

            // Assert
            Check.That(result.Result.Status).IsEqualTo(RemoverPoçoStatus.PoçoNãoEncontrado);
            Check.That(result.Result.Mensagem).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
            A.CallTo(() => poçoWriteOnlyRepository.RemoverPoço(A<string>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public async Task DeveRetornarPoçoRemovidoSeRemoverPoço()
        {
            // Arrange
            const string id = "id";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(true);
            A.CallTo(() => poçoWriteOnlyRepository.RemoverPoço(A<string>.Ignored)).Returns(true);

            var removerPoçoUseCase = new RemoverPoçoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await removerPoçoUseCase.Execute(id);

            // Assert
            Check.That(result.Status).IsEqualTo(RemoverPoçoStatus.PoçoRemovido);
        }

        [Test]
        public async Task DeveRetornarPoçoNãoRemovidoSeNãoConseguirRemoverPoço()
        {
            // Arrange
            const string id = "id";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(true);
            A.CallTo(() => poçoWriteOnlyRepository.RemoverPoço(A<string>.Ignored)).Returns(false);

            var removerPoçoUseCase = new RemoverPoçoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await removerPoçoUseCase.Execute(id);

            // Assert
            Check.That(result.Status).IsEqualTo(RemoverPoçoStatus.PoçoNãoRemovido);
            Check.That(result.Mensagem).IsEqualTo("Não foi possível remover poço. ");
        }

        [Test]
        public async Task DeveRetornarPoçoNãoRemovidoSeRepositórioLançarException()
        {
            // Arrange
            const string id = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).ThrowsAsync(new Exception(mensagemDeErro));

            var removerPoçoUseCase = new RemoverPoçoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await removerPoçoUseCase.Execute(id);

            // Assert
            Check.That(result.Status).IsEqualTo(RemoverPoçoStatus.PoçoNãoRemovido);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível remover poço. {mensagemDeErro}");
        }
    }
}