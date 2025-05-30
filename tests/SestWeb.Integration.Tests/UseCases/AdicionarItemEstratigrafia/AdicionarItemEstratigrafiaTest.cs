using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.AdicionarItemEstratigrafia
{
    [TestFixture]
    public class AdicionarItemEstratigrafiaTest : IntegrationTestBase
    {
        [Test]
        public async Task IntegrationTest_DeveAdicionarItemEstratigrafiaComSucesso()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            const string jsonString = @"
            {
                'pm': 1000.0,
                'sigla': 'sigla1',
                'descrição': 'descrição1',
                'tipo': 'FM',
                'idade': 'idade1'
            }";

            var json = JObject.Parse(jsonString);

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/pocos/{idPoço}/adicionar-item-estratigrafia", httpContent);

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo("Item de estratigrafia adicionado com sucesso.");
        }

        [Test]
        public async Task IntegrationTest_NãoDeveAdicionarItemEstratigrafiaSeHouveErro()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            const string jsonString = @"
            {
                'pm': 1000.0,
                'sigla': 'sigla1',
                'descrição': 'descrição1',
                'tipo': 'AA',
                'idade': 'idade1'
            }";

            var json = JObject.Parse(jsonString);

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/pocos/{idPoço}/adicionar-item-estratigrafia", httpContent);

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo("[AdicionarItemEstratigrafia] - TipoEstratigrafia - Tipo inválido.");
        }

        [Test]
        public async Task IntegrationTest_NãoDeveAdicionarItemEstratigrafiaSePoçoNãoEncontrado()
        {
            // Arrange
            var idPoço = ObjectId.GenerateNewId();

            const string jsonString = @"
            {
                'pm': 1000.0,
                'sigla': 'sigla1',
                'descrição': 'descrição1',
                'tipo': 'AA',
                'idade': 'idade1'
            }";

            var json = JObject.Parse(jsonString);

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/pocos/{idPoço}/adicionar-item-estratigrafia", httpContent);

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo($"[AdicionarItemEstratigrafia] - Não foi possível obter estratigrafia do poço {idPoço}.");
        }
    }
}