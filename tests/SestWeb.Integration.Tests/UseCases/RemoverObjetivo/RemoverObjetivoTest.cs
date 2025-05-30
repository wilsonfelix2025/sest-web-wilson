using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.RemoverObjetivo
{
    [TestFixture]
    public class RemoverObjetivoTest : IntegrationTestBase
    {
        private async Task CriarObjetivo(string idPoço)
        {
            // Arrange
            var json = new JObject
            {
                { "ProfundidadeMedida", "3000" },
                { "DiâmetroSapata", "12" }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/pocos/{idPoço}/criar-objetivo", httpContent);

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus204SeRemoveuObjetivoComSucesso()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();
            await CriarObjetivo(idPoço);

            var json = new JObject
            {
                { "ProfundidadeMedida", "3000.0" }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/pocos/{idPoço}/remover-objetivo", httpContent);

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus400SeNãoConseguiuRemoverObjetivo()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            var json = new JObject
            {
                { "ProfundidadeMedida", "3000.0" }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/pocos/{idPoço}/remover-objetivo", httpContent);

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            Check.That(content).StartsWith("Não foi possível remover o objetivo.");
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
            var response = await Client.PostAsync($"/api/pocos/{outroId}/remover-objetivo", httpContent);

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);

            var content = await response.Content.ReadAsStringAsync();
            Check.That(content).IsEqualTo($"Não foi possível encontrar poço com id {outroId}.");
        }
    }
}