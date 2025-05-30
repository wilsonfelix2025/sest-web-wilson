using System.Net;
using System.Threading.Tasks;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.ObterEstratigrafia
{
    [TestFixture]
    public class ObterEstratigrafiaTest : IntegrationTestBase
    {
        [Test]
        public async Task IntegrationTest_DeveReceberStatus200SeObteveEstratigrafia()
        {
            // Arrange
            var id = await CriarPoçoDebug();

            // Act
            var response = await Client.GetAsync($"/api/pocos/{id}/obter-estratigrafia");

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo("Estratigrafia obtida com sucesso.");
            Check.That(content["estratigrafia"]).IsNotNull();
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus400SeHouveErro()
        {
            // Arrange
            const string id = "id";

            // Act
            var response = await Client.GetAsync($"/api/pocos/{id}/obter-estratigrafia");

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo($"[ObterEstratigrafia] - O id {id} não está em um formato válido.");
            Check.That(content["estratigrafia"]).IsNull();
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus400SeNãoEncontrouPoço()
        {
            // Arrange
            var id = ObjectId.GenerateNewId();

            // Act
            var response = await Client.GetAsync($"/api/pocos/{id}/obter-estratigrafia");

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo($"[ObterEstratigrafia] - Não foi possível obter estratigrafia do poço {id}.");
            Check.That(content["estratigrafia"]).IsNull();
        }
    }
}