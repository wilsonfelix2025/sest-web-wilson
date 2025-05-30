using System;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.PerfilUseCases.RemoverPerfil;
using SestWeb.Domain.Entities.Poço;

namespace SestWeb.Application.Tests.UseCases.RemoverPerfil
{
    [TestFixture]
    public class RemoverPerfilTest
    {
        [Test]
        public async Task DeveRetornarPerfilRemovidoSeRemoverPerfil()
        {
            // Arrange
            const string id = "id";

            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();
            var poçoReaOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();

            A.CallTo(() => perfilWriteOnlyRepository.RemoverPerfil(A<string>.Ignored, A<Poço>.Ignored)).Returns(true);

            var removerPerfilUseCase = new RemoverPerfilUseCase(perfilWriteOnlyRepository, poçoReaOnlyRepository);

            // Act
            var result = await removerPerfilUseCase.Execute(id,"id");

            // Assert
            Check.That(result.Status).IsEqualTo(RemoverPerfilStatus.PerfilRemovido);
            Check.That(result.Mensagem).IsEqualTo("Perfil removido com sucesso.");
        }

        [Test]
        public void DeveRetornarPerfilNãoEncontradoSePerfilNãoExiste()
        {
            // Arrange
            const string id = "id";

            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();
            var poçoReaOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();

            A.CallTo(() => perfilWriteOnlyRepository.RemoverPerfil(A<string>.Ignored, A<Poço>.Ignored)).Returns(false);

            var removerPerfilUseCase = new RemoverPerfilUseCase(perfilWriteOnlyRepository, poçoReaOnlyRepository);

            // Act
            var result = removerPerfilUseCase.Execute(id,"id");

            // Assert
            Check.That(result.Result.Status).IsEqualTo(RemoverPerfilStatus.PerfilNãoEncontrado);
            Check.That(result.Result.Mensagem).IsEqualTo($"Não foi possível encontrar perfil com id {id}.");
            A.CallTo(() => perfilWriteOnlyRepository.RemoverPerfil(A<string>.Ignored, A<Poço>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task DeveRetornarPoçoNãoRemovidoSeRepositórioLançarException()
        {
            // Arrange
            const string id = "id";
            const string idpoço = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();
            var poçoReaOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => perfilWriteOnlyRepository.RemoverPerfil(A<string>.Ignored, A<Poço>.Ignored)).ThrowsAsync(new Exception(mensagemDeErro));

            var removerPoçoUseCase = new RemoverPerfilUseCase(perfilWriteOnlyRepository, poçoReaOnlyRepository);

            // Act
            var result = await removerPoçoUseCase.Execute(id, idpoço);

            // Assert
            Check.That(result.Status).IsEqualTo(RemoverPerfilStatus.PerfilNãoRemovido);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível remover o perfil. {mensagemDeErro}");
        }
    }
}