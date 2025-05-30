using System;
using System.Net;
using System.Threading.Tasks;
using MongoDB.Bson;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.ObterDadosGerais
{
    [TestFixture]
    public class ObterDadosGeraisTest : IntegrationTestBase
    {
        [Test]
        public async Task IntegrationTest_DeveReceberStatus200SeEncontrouPoço()
        {
            // Arrange
            var idPoço = await CriarPoçoDebug();

            // Act
            var response = await Client.GetAsync($"/api/pocos/{idPoço}/obter-dados-gerais");

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus400SeHouveErro()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();

            // Act
            var response = await Client.GetAsync($"/api/pocos/{id}/obter-dados-gerais");

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            Check.That(content).IsEqualTo($"Não foi possível obter dados gerais. O id {id} não está em um formato válido.");
        }

        [Test]
        public async Task IntegrationTest_DeveReceberStatus404SeNãoEncontrouPoço()
        {
            // Arrange
            var id = ObjectId.GenerateNewId();

            // Act
            var response = await Client.GetAsync($"/api/pocos/{id}/obter-dados-gerais");

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
            var content = await response.Content.ReadAsStringAsync();
            Check.That(content).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
        }
    }
}