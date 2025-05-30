using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.RemoverItemEstratigrafia
{
    [TestFixture]
    public class RemoverItemEstratigrafiaTests : IntegrationTestBase
    {
        [Test]
        public async Task IntegrationTest_DeveRemoverItemEstratigrafiaComSucesso()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            const string jsonString = @"
            {
                'tipo': 'MB',
                'pm': 5090.0
            }";

            var json = JObject.Parse(jsonString);

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/pocos/{idPoço}/remover-item-estratigrafia", httpContent);

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo("Item de estratigrafia removido com sucesso.");
        }

        [Test]
        public async Task IntegrationTest_NãoDeveRemoverItemEstratigrafiaSeHouveErro()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            const string jsonString = @"
            {
                'tipo': 'AA',
                'pm': 1000.0
            }";

            var json = JObject.Parse(jsonString);

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/pocos/{idPoço}/remover-item-estratigrafia", httpContent);

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo("[RemoverItemEstratigrafia] - TipoEstratigrafia - Tipo inválido.");
        }
    }
}