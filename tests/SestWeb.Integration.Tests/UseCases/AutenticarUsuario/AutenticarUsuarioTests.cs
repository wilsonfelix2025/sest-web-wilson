using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;
using SestWeb.Integration.Tests.Helpers;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SestWeb.Integration.Tests.UseCases.AutenticarUsuario
{
    public class AutenticarUsuarioTests : IntegrationTestBase
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

        [Test]
        public async Task IntegrationTest_DeveAutenticarUsuario()
        {
            var response = await RegistrarAutenticando();

            var content = await response.ToJObject();

            Check.That(content["email"].Value<string>()).IsEqualTo("cleiton@puc-rio.br");
            Check.That(content["username"].Value<string>()).IsEqualTo("Cleiton Rodrigues");
            Check.That(content["emailConfirmado"].Value<bool>()).IsEqualTo(true);
            Check.That(content["token"].Value<string>().Length).IsStrictlyGreaterThan(150);
        }

        [Test]
        public async Task IntegrationTest_NãoDeveAutenticarUsuarioInvalido()
        {
            var response = await RegistrarUsuario();

            response = await AutenticarUsuario("usuario@petrobras.com.br", "Gtep@1234");

            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content = await response.ToJObject();
            Check.That(content["mensagem"].Value<string>()).IsEqualTo("Usuário inválido!");
        }

        [Test]
        public async Task IntegrationTest_NãoDeveAutenticarSenhaInvalida()
        {
            var response = await RegistrarUsuario();

            response = await AutenticarUsuario("cleiton@puc-rio.br", "Gtep@123456789");

            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var content = await response.ToJObject();
            Check.That(content["mensagem"].Value<string>()).IsEqualTo("Senha incorreta!");
        }
    }
}
