using System.Net;
using System.Threading.Tasks;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.ObterLitologia
{
    [TestFixture]
    public class ObterLitologiaTests : IntegrationTestBase
    {
        [Test]
        public async Task IntegrationTest_DeveReceberStatus200SeObteveLitologiaComSucesso()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            var litos = await Client.GetAsync($"/api/pocos/{idPoço}/obter-litologias");
            litos.EnsureSuccessStatusCode();

            var contentLito = (JArray) JsonConvert.DeserializeObject(await litos.Content.ReadAsStringAsync());
            var primeiraLito = contentLito[0];
            var idLito = primeiraLito["id"];

            // Act
            var response = await Client.GetAsync($"api/pocos/{idPoço}/obter-litologia/{idLito}");

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus400SeHouveErro()
        {
            // Arrange
            const string id = "id";
            const string idLito = "idLito";

            // Act
            var response = await Client.GetAsync($"api/pocos/{id}/obter-litologia/{idLito}");

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            Check.That(content).StartsWith("Litologia não obtida.");
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus404SeNãoEncontrouPoço()
        {
            // Arrange
            var id = ObjectId.GenerateNewId();
            var idLito = ObjectId.GenerateNewId();

            // Act
            var response = await Client.GetAsync($"api/pocos/{id}/obter-litologia/{idLito}");

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
            var content = await response.Content.ReadAsStringAsync();
            Check.That(content).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
        }
    }
}