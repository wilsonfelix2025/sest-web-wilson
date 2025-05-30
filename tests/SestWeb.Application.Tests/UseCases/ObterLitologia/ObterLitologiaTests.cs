using System;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.LitologiaUseCases.ObterLitologia;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Application.Tests.UseCases.ObterLitologia
{
    [TestFixture]
    public class ObterLitologiaTests
    {
        [Test]
        public async Task DeveRetornarLitologiaSeEncontrou()
        {
            // Arrange
            const string id = "id";
            const string idLitologia = "idLitologia";
            var traj = A.Fake<IConversorProfundidade>();
            var lito1 = new Litologia(TipoLitologia.Prevista, traj);

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(true);
            A.CallTo(() => poçoReadOnlyRepository.ObterLitologia(A<string>.Ignored, A<string>.Ignored)).Returns(lito1);

            var obterLitologiaUseCase = new ObterLitologiaUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterLitologiaUseCase.Execute(id, idLitologia);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterLitologiaStatus.LitologiaObtida);
            Check.That(result.Litologia).IsNotNull();
            Check.That(result.Mensagem).IsEqualTo("Litologia obtida com sucesso.");
        }

        [Test]
        public async Task DeveRetornarPoçoNãoEncontradoSeNãoEncontrouPoço()
        {
            // Arrange
            const string id = "id";
            const string idLitologia = "idLitologia";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(false);

            var obterLitologiaUseCase = new ObterLitologiaUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterLitologiaUseCase.Execute(id, idLitologia);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterLitologiaStatus.PoçoNãoEncontrado);
            Check.That(result.Litologia).IsNull();
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
        }

        [Test]
        public async Task DeveRetornarLitologiaNãoObtidasSeNãoEncontrouLitologia()
        {
            // Arrange
            const string id = "id";
            const string idLitologia = "idLitologia";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(true);
            A.CallTo(() => poçoReadOnlyRepository.ObterLitologia(A<string>.Ignored, A<string>.Ignored)).Returns(Task.FromResult<Litologia>(null));

            var obterLitologiaUseCase = new ObterLitologiaUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterLitologiaUseCase.Execute(id, idLitologia);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterLitologiaStatus.LitologiaNãoObtida);
            Check.That(result.Litologia).IsNull();
            Check.That(result.Mensagem).StartsWith("Litologia não obtida.");
        }

        [Test]
        public async Task DeveRetornarLitologiasNãoObtidasSeLançouExceção()
        {
            // Arrange
            const string id = "id";
            const string idLitologia = "idLitologia";
            const string mensagemDeErro = "Mensagem de erro.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(true);
            A.CallTo(() => poçoReadOnlyRepository.ObterLitologia(A<string>.Ignored, A<string>.Ignored)).ThrowsAsync(new Exception(mensagemDeErro));

            var obterLitologiasUseCase = new ObterLitologiaUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterLitologiasUseCase.Execute(id, idLitologia);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterLitologiaStatus.LitologiaNãoObtida);
            Check.That(result.Litologia).IsNull();
            Check.That(result.Mensagem).IsEqualTo($"Litologia não obtida. {mensagemDeErro}");
        }
    }
}