using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.ObterPerfil
{
    [TestFixture]
    public class ObterPerfilTest : IntegrationTestBase
    {
        private async Task<string> CriarPerfil(string idPoço)
        {
            // Arrange
            var json = new JObject
            {
                { "idPoço", idPoço },
                { "nome", "NovoPerfil"},
                { "mnemônico", "DTC"}
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/api/perfis", httpContent);

            // Assert
            response.EnsureSuccessStatusCode();

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            return content["id"].ToString();
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus201SeObtevePerfil()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();
            var idPerfil = await CriarPerfil(idPoço);

            // Act
            var response = await Client.GetAsync($"/api/perfis/{idPerfil}");

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo("Perfil obtido com sucesso.");
            Check.That(content["perfil"]["id"].ToString()).IsEqualTo(idPerfil);
            Check.That(content["perfil"]["idPoço"].ToString()).IsEqualTo(idPoço);
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus404SeNãoEncontrouPerfil()
        {
            // Arrange
            var idPerfil = ObjectId.GenerateNewId().ToString();

            // Act
            var response = await Client.GetAsync($"/api/perfis/{idPerfil}");

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo($"Não foi possível encontrar perfil com id {idPerfil}.");
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus400SeHouveErro()
        {
            // Arrange
            const string idPerfil = "id";

            // Act
            var response = await Client.GetAsync($"/api/perfis/{idPerfil}");

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo($"Não foi possível obter perfil. O id {idPerfil} não está em um formato válido.");
        }
    }
}