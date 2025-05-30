using System;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.ObjetivoUseCases.RemoverObjetivo;

namespace SestWeb.Application.Tests.UseCases.RemoverObjetivo
{
    [TestFixture]
    public class RemoverObjetivoTest
    {
        [Test]
        public async Task DeveRetornarObjetivoRemovidaSeRemoverObjetivo()
        {
            // Arrange
            const string id = "id";
            const double pm = 2000.0;

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(true);
            A.CallTo(() => poçoWriteOnlyRepository.RemoverObjetivo(A<string>.Ignored, A<double>.Ignored)).Returns(true);

            var removerObjetivoUseCase = new RemoverObjetivoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await removerObjetivoUseCase.Execute(id, pm);

            // Assert
            Check.That(result.Status).IsEqualTo(RemoverObjetivoStatus.ObjetivoRemovido);
            Check.That(result.Mensagem).IsEqualTo("Objetivo removido com sucesso.");
        }

        [Test]
        public void DeveRetornarPoçoNãoEncontradoSePoçoNãoExiste()
        {
            // Arrange
            const string id = "id";
            const double pm = 2000.0;

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(false);

            var removerObjetivoUseCase = new RemoverObjetivoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = removerObjetivoUseCase.Execute(id, pm);

            // Assert
            Check.That(result.Result.Status).IsEqualTo(RemoverObjetivoStatus.PoçoNãoEncontrado);
            Check.That(result.Result.Mensagem).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
            A.CallTo(() => poçoWriteOnlyRepository.RemoverPoço(A<string>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public async Task DeveRetornarObjetivoNãoRemovidoSeNãoConseguirRemoverObjetivo()
        {
            // Arrange
            const string id = "id";
            const double pm = 2000.0;

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(true);
            A.CallTo(() => poçoWriteOnlyRepository.RemoverObjetivo(A<string>.Ignored, A<double>.Ignored)).Returns(false);

            var removerObjetivoUseCase = new RemoverObjetivoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await removerObjetivoUseCase.Execute(id, pm);

            // Assert
            Check.That(result.Status).IsEqualTo(RemoverObjetivoStatus.ObjetivoNãoRemovido);
            Check.That(result.Mensagem).IsEqualTo("Não foi possível remover o objetivo. ");
        }

        [Test]
        public async Task DeveRetornarObjetivoNãoRemovidoSeRepositórioLançarException()
        {
            // Arrange
            const string id = "id";
            const double pm = 2000.0;
            const string mensagemDeErro = "Mensagem de erro.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).ThrowsAsync(new Exception(mensagemDeErro));

            var removerObjetivoUseCase = new RemoverObjetivoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await removerObjetivoUseCase.Execute(id, pm);

            // Assert
            Check.That(result.Status).IsEqualTo(RemoverObjetivoStatus.ObjetivoNãoRemovido);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível remover o objetivo. {mensagemDeErro}");
        }
    }
}