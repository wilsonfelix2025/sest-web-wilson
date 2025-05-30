using System.Net;
using System.Threading.Tasks;
using MongoDB.Bson;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.RemoverPoço
{
    [TestFixture]
    public class RemoverPoçoTest : IntegrationTestBase
    {
        [Test]
        public async Task IntegrationTest_DeveReceberStatus204SeRemoveuPoçoComSucesso()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            // Act
            var response = await Client.DeleteAsync($"/api/pocos/{idPoço}");

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus400SeNãoConseguiuRemoverPoço()
        {
            // Arrange
            const string id = "id";

            // Act
            var response = await Client.DeleteAsync($"/api/pocos/{id}");

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            Check.That(content).IsEqualTo($"Não foi possível remover poço. O id {id} não está em um formato válido.");
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus404SeNãoEncontrouPoço()
        {
            // Arrange
            var outroId = ObjectId.GenerateNewId().ToString();

            // Act
            var response = await Client.DeleteAsync($"/api/pocos/{outroId}");

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
            var content = await response.Content.ReadAsStringAsync();
            Check.That(content).IsEqualTo($"Não foi possível encontrar poço com id {outroId}.");
        }
    }
}