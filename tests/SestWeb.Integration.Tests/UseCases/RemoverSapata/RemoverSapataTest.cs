using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.RemoverSapata
{
    [TestFixture]
    public class RemoverSapataTest : IntegrationTestBase
    {
        private async Task CriarSapata(string idPoço)
        {
            // Arrange
            var json = new JObject
            {
                { "ProfundidadeMedida", "3000" },
                { "DiâmetroSapata", "12" }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/pocos/{idPoço}/criar-sapata", httpContent);

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus204SeRemoveuSapataComSucesso()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();
            await CriarSapata(idPoço);

            var json = new JObject
            {
                { "ProfundidadeMedida", "3000.0" }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/pocos/{idPoço}/remover-sapata", httpContent);

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus400SeNãoConseguiuRemoverSapata()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            var json = new JObject
            {
                { "ProfundidadeMedida", "3000.0" }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/pocos/{idPoço}/remover-sapata", httpContent);

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            Check.That(content).StartsWith("Não foi possível remover a sapata.");
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus404SeNãoEncontrouPoço()
        {
            // Arrange
            var outroId = ObjectId.GenerateNewId().ToString();

            var json = new JObject
            {
                { "ProfundidadeMedida", "3000.0" }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/pocos/{outroId}/remover-sapata", httpContent);

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
            var content = await response.Content.ReadAsStringAsync();
            Check.That(content).IsEqualTo($"Não foi possível encontrar poço com id {outroId}.");
        }
    }
}