using System;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.TrajetóriaUseCases.ObterTrajetória;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Application.Tests.UseCases.ObterTrajetória
{
    [TestFixture]
    public class ObterTrajetóriaTest
    {
        [Test]
        public async Task DeveRetornarTrajetóriaObtidaSeEncontrouPoço()
        {
            // Arrange
            const string id = "id";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterTrajetória(A<string>.Ignored)).Returns(new Trajetória(MétodoDeCálculoDaTrajetória.MinimaCurvatura));

            var obterTrajetóriaUseCase = new ObterTrajetóriaUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterTrajetóriaUseCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterTrajetóriaStatus.TrajetóriaObtida);
            Check.That(result.Trajetória).IsNotNull();
            Check.That(result.Mensagem).IsEqualTo("Trajetória obtida com sucesso.");
        }

        [Test]
        public async Task DeveRetornarPoçoNãoEncontradoSeNãoExistePoço()
        {
            // Arrange
            const string id = "id";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterTrajetória(A<string>.Ignored)).Returns(Task.FromResult<Trajetória>(null));

            var obterTrajetóriaUseCase = new ObterTrajetóriaUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterTrajetóriaUseCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterTrajetóriaStatus.PoçoNãoEncontrado);
            Check.That(result.Trajetória).IsNull();
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
        }

        [Test]
        public async Task DeveRetornarTrajetóriaNãoObtidaSeHouveExceção()
        {
            // Arrange
            const string mensagemErro = "Mensagem da exceção.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterTrajetória(A<string>.Ignored)).ThrowsAsync(new Exception(mensagemErro));

            var obterTrajetóriaUseCase = new ObterTrajetóriaUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterTrajetóriaUseCase.Execute(string.Empty);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterTrajetóriaStatus.TrajetóriaNãoObtida);
            Check.That(result.Trajetória).IsNull();
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível obter trajetória. {mensagemErro}");
        }
    }
}