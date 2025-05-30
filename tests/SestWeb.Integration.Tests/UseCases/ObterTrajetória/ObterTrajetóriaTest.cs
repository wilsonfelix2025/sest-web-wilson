using System.Net;
using System.Threading.Tasks;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.ObterTrajetória
{
    [TestFixture]
    public class ObterTrajetóriaTest : IntegrationTestBase
    {
        [Test]
        public async Task IntegrationTest_DeveReceberStatus200SeObteveTrajetória()
        {
            // Arrange
            var id = await CriarPoçoDebug();

            // Act
            var response = await Client.GetAsync($"/api/pocos/{id}/obter-trajetoria");

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo("Trajetória obtida com sucesso.");
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus400SeHouveErro()
        {
            // Arrange
            const string id = "id";

            // Act
            var response = await Client.GetAsync($"/api/pocos/{id}/obter-trajetoria");

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo($"Não foi possível obter trajetória. O id {id} não está em um formato válido.");
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus404SeNãoEncontrouPoço()
        {
            // Arrange
            var id = ObjectId.GenerateNewId();

            // Act
            var response = await Client.GetAsync($"/api/pocos/{id}/obter-trajetoria");

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
        }
    }
}