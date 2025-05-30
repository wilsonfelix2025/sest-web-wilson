using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.CriarPerfil
{
    [TestFixture]
    public class CriarPerfilTest : IntegrationTestBase
    {
        [TearDown]
        public async Task LimparBanco()
        {
            var response = await Client.PostAsync("/api/debug/apagar-database", null);

            response.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task IntegrationTest_DeveCriarPerfilComSucesso()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

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
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        }

        [Test]
        public async Task IntegrationTest_NãoDeveCriarPerfilComIdPoçoInválido()
        {
            // Arrange
            var idPoço = ObjectId.GenerateNewId().ToString();

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
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var mensagem = await response.Content.ReadAsStringAsync();
            Check.That(mensagem).IsEqualTo($"Não foi possível criar perfil. Não foi possível encontrar poço com id {idPoço}.");
        }

        [Test]
        public async Task IntegrationTest_NãoDeveCriarPerfilCasoOcorraErro()
        {
            // Arrange
            var idPoço = "id";

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
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var mensagem = await response.Content.ReadAsStringAsync();
            Check.That(mensagem).IsEqualTo($"Não foi possível criar perfil. O id {idPoço} não está em um formato válido.");
        }
    }
}