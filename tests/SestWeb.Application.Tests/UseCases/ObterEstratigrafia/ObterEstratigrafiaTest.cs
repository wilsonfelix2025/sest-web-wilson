using System;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.EstratigrafiaUseCases.ObterEstratigrafia;
using SestWeb.Domain.Entities.Poço.EstratigrafiaDoPoço;

namespace SestWeb.Application.Tests.UseCases.ObterEstratigrafia
{
    [TestFixture]
    public class ObterEstratigrafiaTest
    {
        [Test]
        public async Task DeveRetornarEstratigrafiaObtidaSeEncontrouPoço()
        {
            // Arrange
            const string id = "id";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterEstratigrafia(A<string>.Ignored)).Returns(new Estratigrafia());

            var obterEstratigrafiaUseCase = new ObterEstratigrafiaUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterEstratigrafiaUseCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterEstratigrafiaStatus.EstratigrafiaObtida);
            Check.That(result.Estratigrafia).IsNotNull();
            Check.That(result.Mensagem).IsEqualTo("Estratigrafia obtida com sucesso.");
        }

        [Test]
        public async Task DeveRetornarPoçoNãoEncontradoSeNãoExistePoço()
        {
            // Arrange
            const string id = "id";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterEstratigrafia(A<string>.Ignored)).Returns(Task.FromResult<Estratigrafia>(null));

            var obterEstratigrafiaUseCase = new ObterEstratigrafiaUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterEstratigrafiaUseCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterEstratigrafiaStatus.EstratigrafiaNãoObtida);
            Check.That(result.Estratigrafia).IsNull();
            Check.That(result.Mensagem).IsEqualTo($"[ObterEstratigrafia] - Não foi possível obter estratigrafia do poço {id}.");
        }

        [Test]
        public async Task DeveRetornarTrajetóriaNãoObtidaSeHouveExceção()
        {
            // Arrange
            const string mensagemErro = "Mensagem da exceção.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterEstratigrafia(A<string>.Ignored)).ThrowsAsync(new Exception(mensagemErro));

            var obterEstratigrafiaUseCase = new ObterEstratigrafiaUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterEstratigrafiaUseCase.Execute(string.Empty);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterEstratigrafiaStatus.EstratigrafiaNãoObtida);
            Check.That(result.Estratigrafia).IsNull();
            Check.That(result.Mensagem).IsEqualTo($"[ObterEstratigrafia] - {mensagemErro}");
        }
    }
}