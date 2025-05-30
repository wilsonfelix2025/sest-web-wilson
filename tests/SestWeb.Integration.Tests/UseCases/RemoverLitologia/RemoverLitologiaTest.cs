using System.Net;
using System.Threading.Tasks;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.RemoverLitologia
{
    [TestFixture]
    public class RemoverLitologiaTest : IntegrationTestBase
    {
        [Test]
        public async Task IntegrationTest_DeveReceberStatus204SeRemoveuLitologiaComSucesso()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            var litos = await Client.GetAsync($"/api/pocos/{idPoço}/obter-litologias");
            litos.EnsureSuccessStatusCode();

            var contentLito = (JArray)JsonConvert.DeserializeObject(await litos.Content.ReadAsStringAsync());
            var primeiraLito = contentLito[0];
            var idLito = primeiraLito["id"];

            // Act
            var response = await Client.PostAsync($"api/pocos/{idPoço}/remover-litologia/{idLito}", null);

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus400SeNãoConseguiuRemoverLitologia()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();
            const string idLito = "idLitologia";

            // Act
            var response = await Client.PostAsync($"api/pocos/{idPoço}/remover-litologia/{idLito}", null);

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content = await response.Content.ReadAsStringAsync();
            Check.That(content).IsEqualTo($"Não foi possível remover a litologia. O idLitologia {idLito} não está em um formato válido.");
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus404SeNãoEncontrouPoço()
        {
            // Arrange
            var outroId = ObjectId.GenerateNewId().ToString();
            var outroIdLito = ObjectId.GenerateNewId().ToString();

            // Act
            var response = await Client.PostAsync($"api/pocos/{outroId}/remover-litologia/{outroIdLito}", null);

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);

            var content = await response.Content.ReadAsStringAsync();
            Check.That(content).IsEqualTo($"Não foi possível encontrar poço com id {outroId}.");
        }
    }
}