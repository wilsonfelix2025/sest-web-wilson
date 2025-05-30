using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.CriarLitologia
{
    [TestFixture]
    public class CriarLitologiaTest : IntegrationTestBase
    {
        [Test]
        public async Task IntegrationTest_DeveCriarLitologiaComSucesso()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            var json = new JObject
            {
                { "TipoLitologia", "Prevista" }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/pocos/{idPoço}/criar-litologia", httpContent);

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        }

        [Test]
        public async Task IntegrationTest_NãoDeveCriarLitologiaSeHouveErro()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            var json = new JObject
            {
                { "TipoLitologia", "xpto" }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/pocos/{idPoço}/criar-litologia", httpContent);

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        }
    }
}