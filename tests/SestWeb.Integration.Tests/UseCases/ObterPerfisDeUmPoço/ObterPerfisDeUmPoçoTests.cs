using System.Net;
using System.Threading.Tasks;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.ObterPerfisDeUmPoço
{
    [TestFixture]
    public class ObterPerfisDeUmPoçoTests : IntegrationTestBase
    {
        [Test]
        public async Task IntegrationTest_DeveReceberStatus200SeObtevePerfis()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            // Act
            var response = await Client.GetAsync($"api/pocos/{idPoço}/obter-perfis");

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo("Perfis obtidos com sucesso.");
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus400SeNãoObtevePerfis()
        {
            // Arrange
            var idPoço = ObjectId.GenerateNewId();

            // Act
            var response = await Client.GetAsync($"api/pocos/{idPoço}/obter-perfis");

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            Check.That(content).StartsWith($"[ObterPerfisDeUmPoço] - Não foi possível encontrar poço com id {idPoço}.");
        }
    }
}