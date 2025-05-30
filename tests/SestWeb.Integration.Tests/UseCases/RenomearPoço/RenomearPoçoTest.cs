using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.RenomearPoço
{
    [TestFixture]
    public class RenomearPoçoTest : IntegrationTestBase
    {
        [Test]
        public async Task IntegrationTest_DeveReceberStatus200SeRenomeouPoçoComSucesso()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            const string nome = "Novo nome";

            var json = new JObject
            {
                { "nomePoço", nome }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PutAsync($"api/pocos/{idPoço}/renomear-poco", httpContent);

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo("Poço renomeado com sucesso.");
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus400SeNãoConseguiuRenomearPoço()
        {
            // Arrange
            const string idPoço = "id";

            const string nome = "Novo nome";

            var json = new JObject
            {
                { "nomePoço", nome }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PutAsync($"api/pocos/{idPoço}/renomear-poco", httpContent);

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo($"Não foi possível renomear o poço. O id {idPoço} não está em um formato válido.");
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus400SeJáExistePoçoComEsseNome()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            const string nome = "6-MRL-199D-RJS";

            var json = new JObject
            {
                { "nomePoço", nome }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PutAsync($"api/pocos/{idPoço}/renomear-poco", httpContent);

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo("Não foi possível criar poço. Já existe poço com esse nome.");
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus404SeNãoConseguiuEncontrarPoço()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();

            const string nome = "Novo nome";

            var json = new JObject
            {
                { "nomePoço", nome }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PutAsync($"api/pocos/{id}/renomear-poco", httpContent);

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
        }
    }
}