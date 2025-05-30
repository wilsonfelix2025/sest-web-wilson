using System;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.SapataUseCases.RemoverSapata;

namespace SestWeb.Application.Tests.UseCases.RemoverSapata
{
    [TestFixture]
    public class RemoverSapataTest
    {
        [Test]
        public async Task DeveRetornarSapataRemovidaSeRemoverSapata()
        {
            // Arrange
            const string id = "id";
            const double pm = 2000.0;

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(true);
            A.CallTo(() => poçoWriteOnlyRepository.RemoverSapata(A<string>.Ignored, A<double>.Ignored)).Returns(true);

            var removerSapataUseCase = new RemoverSapataUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await removerSapataUseCase.Execute(id, pm);

            // Assert
            Check.That(result.Status).IsEqualTo(RemoverSapataStatus.SapataRemovida);
            Check.That(result.Mensagem).IsEqualTo("Sapata removida com sucesso.");
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

            var removerSapataUseCase = new RemoverSapataUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = removerSapataUseCase.Execute(id, pm);

            // Assert
            Check.That(result.Result.Status).IsEqualTo(RemoverSapataStatus.PoçoNãoEncontrado);
            Check.That(result.Result.Mensagem).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
            A.CallTo(() => poçoWriteOnlyRepository.RemoverPoço(A<string>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public async Task DeveRetornarSapataNãoRemovidaSeNãoConseguirRemoverSapata()
        {
            // Arrange
            const string id = "id";
            const double pm = 2000.0;

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(true);
            A.CallTo(() => poçoWriteOnlyRepository.RemoverSapata(A<string>.Ignored, A<double>.Ignored)).Returns(false);

            var removerSapataUseCase = new RemoverSapataUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await removerSapataUseCase.Execute(id, pm);

            // Assert
            Check.That(result.Status).IsEqualTo(RemoverSapataStatus.SapataNãoRemovida);
            Check.That(result.Mensagem).IsEqualTo("Não foi possível remover a sapata. ");
        }

        [Test]
        public async Task DeveRetornarSapataNãoRemovidaSeRepositórioLançarException()
        {
            // Arrange
            const string id = "id";
            const double pm = 2000.0;
            const string mensagemDeErro = "Mensagem de erro.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).ThrowsAsync(new Exception(mensagemDeErro));

            var removerSapataUseCase = new RemoverSapataUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await removerSapataUseCase.Execute(id, pm);

            // Assert
            Check.That(result.Status).IsEqualTo(RemoverSapataStatus.SapataNãoRemovida);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível remover a sapata. {mensagemDeErro}");
        }
    }
}