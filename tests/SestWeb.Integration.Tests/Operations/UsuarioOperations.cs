using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SestWeb.Integration.Tests.Helpers;

namespace SestWeb.Integration.Tests.Operations
{
    public class UsuarioOperations
    {
        private readonly HttpClient _client;

        public UsuarioOperations(HttpClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> RegistrarUsuario(string nome = "Cleiton", string sobreNome = "Rodrigues", string email = "cleiton@puc-rio.br", string senha = "Gtep@1234")
        {
            dynamic request = new JObject();
            request.nome = nome;
            request.sobrenome = sobreNome;
            request.email = email;
            request.senha = senha;

            var httpContent = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
            return await _client.PostAsync("/api/Usuario/", httpContent);
        }

        public async Task<HttpResponseMessage> AutenticarUsuario(string email = "cleiton@puc-rio.br", string senha = "Gtep@1234")
        {
            dynamic request = new JObject();
            request.email = email;
            request.senha = senha;

            var httpContent = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
            return await _client.PostAsync("/api/usuario/autenticar", httpContent);
        }

        public async Task<HttpResponseMessage> RegistrarAutenticando(string nome = "Cleiton", string sobreNome = "Rodrigues", string email = "cleiton@puc-rio.br", string senha = "Gtep@1234")
        {
            var response = await RegistrarUsuario(nome, sobreNome, email);
            var content = await response.ToJObject();
            var idUsuario = content["idUsuario"].ToString();
            var codigo = content["codigoConfirmaçãoEmail"].ToString();

            dynamic request = new JObject();
            request.IdUsuario = idUsuario;
            request.Codigo = codigo;

            var httpContent = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
            response = await _client.PostAsync("/api/usuario/confirmarEmail", httpContent);

            response.EnsureSuccessStatusCode();

            request = new JObject();
            request.email = email;
            request.senha = senha;

            httpContent = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
            return await _client.PostAsync("/api/usuario/autenticar", httpContent);
        }

        public async Task<HttpResponseMessage> EsquecerSenha(string email = "cleiton@puc-rio.br")
        {
            dynamic request = new JObject();
            request.email = email;

            var httpContent = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
            return await _client.PostAsync("/api/usuario/esqueceu-senha", httpContent);
        }

        public async Task<HttpResponseMessage> ResetarSenha(string codigo, string email = "cleiton@puc-rio.br", string novaSenha = "Gtep@1234")
        {
            dynamic request = new JObject();
            request.email = email;
            request.Senha = novaSenha;
            request.Codigo = codigo;

            var httpContent = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
            return await _client.PostAsync("/api/usuario/resetar-senha", httpContent);
        }

        public async Task<HttpResponseMessage> TrocarSenha(string token, string senhaAntiga, string senhaNova)
        {

            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            dynamic request = new JObject();
            request.SenhaAntiga = senhaAntiga;
            request.NovaSenha = senhaNova;

            var httpContent = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
            return await _client.PostAsync("/api/usuario/trocar-senha", httpContent);
        }

        public async Task<HttpResponseMessage> ObterUsuario(string token, string email)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);


            return await _client.GetAsync("/api/admin/obter-usuario?email="+email);
        }
    }
}
