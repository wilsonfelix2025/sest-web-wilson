using System.Net;
using System.Threading.Tasks;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.ObterPoço
{
    [TestFixture]
    public class ObterPoçoTest : IntegrationTestBase
    {
        [Test]
        public async Task IntegrationTest_DeveReceberStatus201SeObtevePoçoCriado()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            // Act
            var response = await Client.GetAsync($"/api/pocos/{idPoço}");

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo("Poço obtido com sucesso.");
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus400SeHouveErroAoObterPoço()
        {
            // Arrange
            const string id = "id";

            // Act
            var response = await Client.GetAsync($"/api/pocos/{id}");

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo($"Não foi possível obter poço. O id {id} não está em um formato válido.");
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus404SeNãoEncontrouPoço()
        {
            // Arrange
            var id = ObjectId.GenerateNewId();

            // Act
            var response = await Client.GetAsync($"/api/pocos/{id}");

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
        }
    }
}