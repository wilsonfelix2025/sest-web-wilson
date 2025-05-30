using Newtonsoft.Json.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Integration.Tests.Base;
using SestWeb.Integration.Tests.Helpers;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SestWeb.Integration.Tests.UseCases.ResetarSenha
{
    [TestFixture]
    public class ResetarSenhaTests : IntegrationTestBase
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
        public async Task IntegrationTest_DeveResetarSenha()
        {
            var novaSenha = "Gtep@4321";
            var email = "cleiton@puc-rio.br";

            var response = await RegistrarAutenticando();
            response.EnsureSuccessStatusCode();

            response = await EsquecerSenha();
            response.EnsureSuccessStatusCode();

            var codigoReset = (await response.ToJObject())["codigoReset"].ToString();
            response = await ResetarSenha(codigoReset, email: email);

            response.EnsureSuccessStatusCode();

            var mensagem = (await response.ToJObject())["mensagem"].ToString();

            Check.That(mensagem).IsEqualTo("Senha resetada com sucesso!");
        }
        
        [Test]
        public async Task IntegrationTest_NãoDeveResetarSenhaParaUsuarioNaoEncontrado()
        {
            var response = await RegistrarAutenticando();
            response.EnsureSuccessStatusCode();

            response = await EsquecerSenha();
            response.EnsureSuccessStatusCode();

            var codigoReset = (await response.ToJObject())["codigoReset"].ToString();

            response = await ResetarSenha(codigoReset, email: "outroemail@puc-rio.br");

            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
            var mensagem = (await response.ToJObject())["mensagem"].ToString();
            Check.That(mensagem).IsEqualTo("A senha não pôde ser resetada!");
        }
        
        [Test]
        public async Task IntegrationTest_NãoDeveResetarCasoCodigoInválido()
        {
            var response = await RegistrarAutenticando();
            response.EnsureSuccessStatusCode();

            response = await EsquecerSenha();
            response.EnsureSuccessStatusCode();

            var codigoReset = (await response.ToJObject())["codigoReset"].ToString();

            response = await ResetarSenha(codigoReset + "aaa");

            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
            var mensagem = (await response.ToJObject())["mensagem"].ToString();
            Check.That(mensagem).IsEqualTo("A senha não pôde ser resetada!");
        }
    }
}
