using System;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.LitologiaUseCases.CriarLitologia;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Application.Tests.UseCases.CriarLitologia
{
    [TestFixture]
    public class CriarLitologiaTest
    {
        [Test]
        public async Task DeveCriarLitologia()
        {
            // Arrange
            const string idPoço = "id";
            const string tipoLitologia = "Adaptada";

            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoWriteOnlyRepository.CriarLitologia(A<string>.Ignored, A<Litologia>.Ignored)).Returns(true);

            var criarObjetivoUseCase = new CriarLitologiaUseCase(poçoWriteOnlyRepository);

            // Act
            var result = await criarObjetivoUseCase.Execute(idPoço, tipoLitologia);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarLitologiaStatus.LitologiaCriada);
            Check.That(result.Mensagem).IsEqualTo("Litologia criada com sucesso.");
        }

        [Test]
        public async Task NãoDeveCriarLitologiaComTipoInválido()
        {
            // Arrange
            const string idPoço = "id";
            const string tipoLitologia = "xpto";

            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();

            var criarObjetivoUseCase = new CriarLitologiaUseCase(poçoWriteOnlyRepository);

            // Act
            var result = await criarObjetivoUseCase.Execute(idPoço, tipoLitologia);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarLitologiaStatus.LitologiaNãoCriada);
            Check.That(result.Mensagem).IsEqualTo("Não foi possível criar a litologia. Tipo de litologia inválido.");
        }

        [Test]
        public async Task NãoConseguiuCriarLitologia()
        {
            // Arrange
            const string idPoço = "id";
            const string tipoLitologia = "Adaptada";

            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoWriteOnlyRepository.CriarLitologia(A<string>.Ignored, A<Litologia>.Ignored)).Returns(false);

            var criarObjetivoUseCase = new CriarLitologiaUseCase(poçoWriteOnlyRepository);

            // Act
            var result = await criarObjetivoUseCase.Execute(idPoço, tipoLitologia);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarLitologiaStatus.LitologiaNãoCriada);
            Check.That(result.Mensagem).StartsWith("Não foi possível criar a litologia.");
        }

        [Test]
        public async Task NãoConseguiuCriarLitologiaPorqueLançouExceção()
        {
            // Arrange
            const string idPoço = "id";
            const string tipoLitologia = "Adaptada";
            const string mensagem = "Mensagem da exceção.";

            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoWriteOnlyRepository.CriarLitologia(A<string>.Ignored, A<Litologia>.Ignored)).ThrowsAsync(new Exception(mensagem));

            var criarObjetivoUseCase = new CriarLitologiaUseCase(poçoWriteOnlyRepository);

            // Act
            var result = await criarObjetivoUseCase.Execute(idPoço, tipoLitologia);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarLitologiaStatus.LitologiaNãoCriada);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível criar a litologia. {mensagem}");
        }
    }
}