using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using System.Threading.Tasks;
using SestWeb.Application.UseCases.UsuárioUseCases.RegistrarUsuario;

namespace SestWeb.Application.Tests.UseCases.RegistrarUsuario
{
    [TestFixture]
    class RegistrarUsuarioTest
    {
        // TODO: Estudar sobre testes de aplicação
        public async Task DeveRetornarUsuarioOutputSeUsuarioFoiCriado()
        {
            // Arrange
            var usuarioWriteOnlyRepository = A.Fake<IUsuarioWriteOnlyRepository>();
            var registrarUsuarioUseCase = new RegistrarUsuarioUseCase(usuarioWriteOnlyRepository);

            // Act
            var result = await registrarUsuarioUseCase.Execute("Cleiton", "Rodrigues", "cleiton@puc-rio.br", "Gtep@1234");

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(RegistrarUsuarioStatus.UsuarioCriado);
            Check.That(result.Email).IsNotNull().And.IsNotEmpty();
            Check.That(result.Email).IsEqualTo("cleiton@puc-rio.br");
            Check.That(result.Mensagem).IsEqualTo("Usuário criado com sucesso.");
        }
    }
}
