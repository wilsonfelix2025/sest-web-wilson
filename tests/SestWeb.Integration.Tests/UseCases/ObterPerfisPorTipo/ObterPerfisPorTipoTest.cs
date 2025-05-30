using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.ObterPerfisPorTipo
{
    [TestFixture]
    public class ObterPerfisPorTipoTest : IntegrationTestBase
    {
        [Test]
        public async Task IntegrationTest_DeveReceberStatus200SeObtevePerfis()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();
            const string mnemônico = "DTC";

            var json = new JObject
            {
                { "IdPoço", idPoço },
                { "Mnemônico", mnemônico }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/api/perfis/obter-perfis-por-tipo", httpContent);

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus400SeNãoObtevePerfis()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();
            const string mnemônico = "ABC";

            var json = new JObject
            {
                { "IdPoço", idPoço },
                { "Mnemônico", mnemônico }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/api/perfis/obter-perfis-por-tipo", httpContent);

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            Check.That(content).StartsWith($"Não foi possível obter perfis. Mnemônico inválido.");
        }
    }
}