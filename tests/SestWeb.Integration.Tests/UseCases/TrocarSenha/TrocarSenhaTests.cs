using System.Net;
using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;
using SestWeb.Integration.Tests.Helpers;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace SestWeb.Integration.Tests.UseCases.TrocarSenha
{
    [TestFixture]
    public class TrocarSenhaTests : IntegrationTestBase
    {
        private async Task<HttpResponseMessage> RegistrarUsuario(string nome = "Cleiton", string sobreNome = "Rodrigues", string email = "cleiton@puc-rio.br", string senha = "Gtep@1234")
        {
            // Registrar
            var json = new JObject
            {
                { "nome", nome },
                { "sobrenome", sobreNome },
                { "email", email },
                { "senha", senha }
            };

            var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            var response = await Client.PostAsync($"/api/usuarios", httpContent);
            response.EnsureSuccessStatusCode();

            // Confirmar email
            var content = await response.ToJObject();
            var idUsuario = content["idUsuario"].ToString();
            var codigo = content["codigoConfirmaçãoEmail"].ToString();

            dynamic request = new JObject();
            request.IdUsuario = idUsuario;
            request.Codigo = codigo;

            httpContent = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
            var result = await Client.PostAsync("/api/usuarios/confirmarEmail", httpContent);
            return result;
        }

        private async Task<HttpResponseMessage> AutenticarUsuario(string email = "cleiton@puc-rio.br", string senha = "Gtep@1234")
        {
            // Autenticar
            dynamic request = new JObject();
            request.email = email;
            request.senha = senha;

            var httpContent = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
            return await Client.PostAsync("/api/usuario/autenticar", httpContent);
        }

        private async Task<HttpResponseMessage> RegistrarAutenticando(string nome = "Cleiton", string sobreNome = "Rodrigues", string email = "cleiton@puc-rio.br", string senha = "Gtep@1234")
        {
            var response = await RegistrarUsuario();
            return await AutenticarUsuario();
        }

        private async Task<HttpResponseMessage> EsquecerSenha(string email = "cleiton@puc-rio.br")
        {
            dynamic request = new JObject();
            request.email = email;

            var httpContent = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
            return await Client.PostAsync("/api/usuarios/esqueceu-senha", httpContent);
        }

        private async Task<HttpResponseMessage> ResetarSenha(string codigo, string email = "cleiton@puc-rio.br", string novaSenha = "Gtep@4321")
        {
            dynamic request = new JObject();
            request.Email = email;
            request.Senha = novaSenha;
            request.Codigo = codigo;

            var httpContent = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
            return await Client.PostAsync("/api/usuarios/resetar-senha", httpContent);
        }

        private async Task<HttpResponseMessage> TrocarSenha(string token, string senhaAntiga, string senhaNova)
        {

            Client.DefaultRequestHeaders.Remove("Authorization");
            Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            dynamic request = new JObject();
            request.SenhaAntiga = senhaAntiga;
            request.NovaSenha = senhaNova;

            var httpContent = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
            return await Client.PostAsync("/api/usuario/trocar-senha", httpContent);
        }

        [Test]
        public async Task DeveResetarSenha()
        {
            var response = await RegistrarAutenticando();

            response = await TrocarSenha((await response.ToJObject())["token"].ToString(), "Gtep@1234", "Gtep@4321");
            response.EnsureSuccessStatusCode();

            var mensagem = (await response.ToJObject())["mensagem"].ToString();

            Check.That(mensagem).IsEqualTo("Senha trocada com sucesso!");
        }

        [TestCase("abcde")]
        public async Task DeveValidarSenhaNova(string senha)
        {
            var response = await RegistrarAutenticando();

            response = await TrocarSenha((await response.ToJObject())["token"].ToString(), "Gtep@1234", senha);

            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content = await response.ToJObject();
            var senhaMessage = content["NovaSenha"];

            Check.That(senhaMessage.Count()).IsEqualTo(4);
            Check.That(senhaMessage[0].Value<string>()).IsEqualTo("Senha deve ter, pelo menos, 6 caracteres alfanuméricos.");
            Check.That(senhaMessage[1].Value<string>()).IsEqualTo("Senha deve conter, pelo menos, uma letra maiúscula.");
            Check.That(senhaMessage[2].Value<string>()).IsEqualTo("Senha deve conter, pelo menos, um número.");
            Check.That(senhaMessage[3].Value<string>()).IsEqualTo("Senha deve conter, pelo menos um, caracter especial.");
        }
    }
}
