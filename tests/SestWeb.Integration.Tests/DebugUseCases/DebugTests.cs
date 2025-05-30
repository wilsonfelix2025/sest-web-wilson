using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.DebugUseCases
{
    [TestFixture]
    public class DebugTests : IntegrationTestBase
    {
        [Test]
        public async Task IntegrationTest_DeveAlimentarBancoDados()
        {
            var response = await Client.PostAsync("/api/debug/alimentar-database", null);

            response.EnsureSuccessStatusCode();

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["status"].Value<string>()).IsEqualTo("OK");
            Check.That(content["mensagem"].Value<string>()).IsEqualTo("Banco de dados carregado com sucesso");
        }

        [Test]
        public async Task IntegrationTest_DeveApagarBancoDados()
        {
            var response = await Client.PostAsync("/api/debug/apagar-database", null);

            response.EnsureSuccessStatusCode();

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["status"].Value<string>()).IsEqualTo("OK");
            Check.That(content["mensagem"].Value<string>()).IsEqualTo("Banco de dados deletado com sucesso");
        }
    }
}