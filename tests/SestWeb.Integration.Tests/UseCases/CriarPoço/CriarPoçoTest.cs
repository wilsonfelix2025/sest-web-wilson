using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;

namespace SestWeb.Integration.Tests.UseCases.CriarPoço
{
    [TestFixture]
    public class CriarPoçoTest : IntegrationTestBase
    {
        [TestCase("Projeto")]
        [TestCase("Monitoramento")]
        [TestCase("Retroanalise")]
        public async Task IntegrationTest_DeveCriarPoçoComSucesso(string tipoPoço)
        {
            // Arrange
            const string nome = "Novo poço";

            var json = new JObject
            {
                { "Nome", nome },
                { "TipoPoço", tipoPoço }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/api/pocos", httpContent);

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["id"].ToString()).IsNotEmpty();
            Check.That(content["nome"].ToString()).IsEqualTo(nome);
            Check.That(content["tipoPoço"].ToString()).IsEqualTo(tipoPoço);
        }

        [Test]
        public async Task IntegrationTest_NãoDeveCriarPoçoSeExistePoçoComMesmoNome()
        {
            // Arrange
            var json = new JObject
            {
                { "Nome", "Novo poço" },
                { "TipoPoço", "Projeto" }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            var response = await Client.PostAsync("/api/pocos", httpContent);
            response.EnsureSuccessStatusCode();

            // Act
            var response2 = await Client.PostAsync("/api/pocos", httpContent);

            // Assert
            Check.That(response2.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content2 = JObject.Parse(await response2.Content.ReadAsStringAsync());
            Check.That(content2["mensagem"].ToString()).IsEqualTo("Não foi possível criar poço. Já existe poço com esse nome.");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public async Task IntegrationTest_NãoDeveCriarPoçoSemCampoNome(string nome)
        {
            // Arrange
            var json = new JObject
            {
                { "Nome", nome },
                { "TipoPoço", "Projeto" }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/api/pocos", httpContent);

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content = JObject.Parse(await response.Content.ReadAsStringAsync());
            Check.That(content["mensagem"].ToString()).IsEqualTo("Não foi possível criar poço. Nome do poço inválido.");
        }

        [Test]
        public async Task IntegrationTest_CasoNãoInformeTipoDeveCriarPoçoDeProjeto()
        {
            // Arrange
            var json = new JObject
            {
                { "Nome", "Novo poço" }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/api/pocos", httpContent);

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);

            var mensagem = await response.Content.ReadAsStringAsync();
            Check.That(mensagem).Contains(@"""tipoPoço"":""Projeto""");
        }
    }
}