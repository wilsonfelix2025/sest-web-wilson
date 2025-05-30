using System.Net;
using System.Threading.Tasks;
using MongoDB.Bson;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.ObterLitologias
{
    [TestFixture]
    public class ObterLitologiasTest : IntegrationTestBase
    {
        [Test]
        public async Task IntegrationTest_DeveReceberStatus200SeObteveLitologiasComSucesso()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            // Act
            var response = await Client.GetAsync($"/api/pocos/{idPoço}/obter-litologias");

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus400SeHouveErro()
        {
            // Arrange
            const string id = "id";

            // Act
            var response = await Client.GetAsync($"/api/pocos/{id}/obter-litologias");

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            Check.That(content).StartsWith("Litologias não obtidas.");
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus404SeNãoEncontrouPoço()
        {
            // Arrange
            var id = ObjectId.GenerateNewId();

            // Act
            var response = await Client.GetAsync($"/api/pocos/{id}/obter-litologias");

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
            var content = await response.Content.ReadAsStringAsync();
            Check.That(content).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
        }
    }
}