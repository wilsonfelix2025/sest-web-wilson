using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.CriarObjetivo
{
    [TestFixture]
    public class CriarObjetivoTest : IntegrationTestBase
    {
        [Test]
        public async Task IntegrationTest_DeveCriarObjetivoComSucesso()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            var json = new JObject
            {
                { "ProfundidadeMedida", "3000" },
                { "TipoObjetivo", "Primário" }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/pocos/{idPoço}/criar-objetivo", httpContent);

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        }

        [Test]
        public async Task IntegrationTest_NãoDeveCriarObjetivoSeJáExiste()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            var json = new JObject
            {
                { "ProfundidadeMedida", "3000" },
                { "TipoObjetivo", "Primário" }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/pocos/{idPoço}/criar-objetivo", httpContent);

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);

            var response2 = await Client.PostAsync($"/api/pocos/{idPoço}/criar-objetivo", httpContent);
            Check.That(response2.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task IntegrationTest_NãoDeveCriarObjetivoSeHouveErro()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            var json = new JObject
            {
                { "ProfundidadeMedida", "3000" },
                { "TipoObjetivo", "" }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/pocos/{idPoço}/criar-objetivo", httpContent);

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        }
    }
}