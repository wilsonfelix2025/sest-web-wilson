using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;
using SestWeb.Integration.Tests.Helpers;

namespace SestWeb.Integration.Tests.UseCases.RegistrarUsuario
{
    [TestFixture]
    public class RegistrarUsuarioTests : IntegrationTestBase
    {
        [Test]
        public async Task IntegrationTest_DeveRegistrarUsuário()
        {
            // Arrange
            var json = new JObject
            {
                { "nome", "Cleiton" },
                { "sobrenome", "Rodrigues" },
                { "email", "cleiton@puc-rio.br" },
                { "senha", "Gtep@1234" }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/usuarios", httpContent);

            // Assert
            response.EnsureSuccessStatusCode();
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        }

        [TestCase("cleiton@gmail.com")]
        [TestCase("cleiton@yahoo.com.br")]
        [TestCase("cleiton@hotmail.com")]
        public async Task IntegrationTest_NãoDeveRegistrarUsuarioComEmailDiferentePucOuPetrobras(string email)
        {
            // Arrange
            var json = new JObject
            {
                { "nome", "Cleiton" },
                { "sobrenome", "Rodrigues" },
                { "email", email },
                { "senha", "Gtep@1234" }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/usuarios", httpContent);

            // Assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content = await response.ToJObject();
            var mensagem = content["Email"][0].ToString();
            Check.That(mensagem).IsEqualTo($"E-mail: '{email}' não é válido. Apenas e-mails com domínio @petrobras.com.br ou @puc-rio.br são permitidos.");
        }

        [Test]
        public async Task IntegrationTest_NãoDeveRegistrarUsuarioComEmailEmUso()
        {
            // Arrange
            var json = new JObject
            {
                { "nome", "Cleiton" },
                { "sobrenome", "Rodrigues" },
                { "email", "cleiton@puc-rio.br" },
                { "senha", "Gtep@1234" }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/usuarios", httpContent);
            response.EnsureSuccessStatusCode();

            // Try register the same object again
            response = await Client.PostAsync($"/api/usuarios", httpContent);


            // Get the response
            var content = await response.ToJObject();

            Check.That(content["mensagem"].Value<string>()).IsEqualTo("Este e-mail já está sendo utilizado.");
        }

        [TestCase("abcde")]
        public async Task IntegrationTest_NãoDeveRegistrarUsuarioComSenhaForaDoPadrão(string senha)
        {
            // Arrange
            var json = new JObject
            {
                { "nome", "Cleiton" },
                { "sobrenome", "Rodrigues" },
                { "email", "cleiton@puc-rio.br" },
                { "senha", senha }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync($"/api/usuarios", httpContent);

            // Check
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content = await response.ToJObject();

            var senhaMessage = content["Senha"];

            Check.That(senhaMessage.Count()).IsEqualTo(4);

            Check.That(senhaMessage[0].Value<string>()).IsEqualTo("Senha deve ter, pelo menos, 6 caracteres alfanuméricos.");
            Check.That(senhaMessage[1].Value<string>()).IsEqualTo("Senha deve conter, pelo menos, uma letra maiúscula.");
            Check.That(senhaMessage[2].Value<string>()).IsEqualTo("Senha deve conter, pelo menos, um número.");
            Check.That(senhaMessage[3].Value<string>()).IsEqualTo("Senha deve conter, pelo menos um, caracter especial.");
        }
    }
}