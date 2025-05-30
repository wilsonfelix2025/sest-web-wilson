using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.ObterPoços
{
    [TestFixture]
    public class ObterPoçosTest : IntegrationTestBase
    {
        [Test]
        public async Task IntegrationTest_DeveReceberStatus200SeObtevePoçosExisteZeroPoço()
        {
            // Arrange
            var responseApagar = await Client.PostAsync("/api/debug/apagar-database", null);
            responseApagar.EnsureSuccessStatusCode();

            // Act
            var response = await Client.GetAsync("/api/pocos");

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo("Poços obtidos com sucesso.");
            Check.That(content["poços"].ToArray()).CountIs(0);
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus200SeObtevePoçosExisteUmPoço()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            // Act
            var response = await Client.GetAsync("/api/pocos");

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo("Poços obtidos com sucesso.");
            Check.That(content["poços"].ToArray()).CountIs(1);
            Check.That(content["poços"].First["id"].ToString()).IsEqualTo(idPoço);
            Check.That(content["poços"].First["nome"].ToString()).IsEqualTo("6-MRL-199D-RJS");
            Check.That(content["poços"].First["tipoPoço"].ToString()).IsEqualTo("Retroanalise");
        }
    }
}