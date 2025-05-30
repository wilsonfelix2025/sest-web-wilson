using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using SestWeb.Api.Helpers.Web;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.PoçoWeb.RequestForwarding
{
    public class PoçoWeb
    {
        /// <summary>
        /// Método privado que faz um request para a API do PoçoWeb.
        /// </summary>
        /// <param name="requestOriginal">Objeto contendo o request inicial recebido pela API do SEST.</param>
        /// <param name="endpointDestino">String contendo o endpoint alvo (não deve conter barras no início nem no final).
        /// </param>
        /// <param name="metodo">Método HTTP sendo feito (GET, POST, PUT).</param>
        /// <returns>Uma Task cujo resultado é um JToken, que é a resposta do request.</returns>
        private static async Task<JToken> FazRequest(HttpRequest requestOriginal, string endpointDestino, string metodo)
        {
             //*
             //* Quando um endpoint retorna um status de redirect, o ASP.NET remove o 
             //* header Authorization por questões de segurança.
             //* 
             //* Se um request for feito para um endpoint do PoçoWeb sem a barra no final, 
             //* será feito um redirecionamento para o endpoint real que possui uma barra
             //* no final. Esse redirecionamento faz com que o header Authorization seja 
             //* perdido, e portanto, torna impossível a autenticação.
             //* 
             //* Entretanto, em algumas URLs, é necessário passar parâmetros adicionais 
             //* na URL, como por exemplo /files?format=tree. Nesse caso, não é razoável 
             //* colocar uma barra no final da URL.
             //* 
             //* O if abaixo faz essa checagem e garante que não haverá redirecionamentos,
             //* e que, portanto, a autenticação com o PoçoWeb será feita corretamente.
             //*
            if (!endpointDestino.Contains("?"))
            {
                endpointDestino += "/";
            }

            // Monta o objeto Request que será feito para o PoçoWeb
            string urlDestino = $"https://pocoweb.petro.intelie.net/{endpointDestino}";
            WebRequest request = WebRequest.Create(urlDestino);
            request.Method = metodo;            

            // Extrai o token do request original e adiciona ao novo
            if (requestOriginal.Headers.ContainsKey("Authorization"))
            {
                request.Headers.Add("Authorization", requestOriginal.Headers["Authorization"].ToString());
            }

            if (metodo == "POST")
            {
                // O
                Stream requestStream = request.GetRequestStream();
                string formEmString = Form.EncodeAsUrl(requestOriginal.Form);
                byte[] formEmBinário = Encoding.GetEncoding("UTF-8").GetBytes(formEmString);
                requestStream.Write(formEmBinário, 0, formEmBinário.Length);
                requestStream.Flush();
                requestStream.Close();

                request.Headers.Add("Content-Type", requestOriginal.Headers["Content-Type"].ToString());
            }

            try
            {
                // Faz o request HTTP e retorna a resposta
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                return Response.ToJson(response);
            }
            catch (WebException e)
            {
                // Se algo der errado, serializa a exceção e retorna um JSON
                return Response.ToJson(e.Response);
            }
        }

        /// <summary>
        /// Faz um GET na API do PoçoWeb.
        /// </summary>
        /// <param name="requestOriginal">Objeto contendo o request inicial recebido pela API do SEST.</param>
        /// <param name="endpointDestino">String contendo o endpoint alvo (não deve conter barras no início nem no final)</param>
        /// <returns>Uma Task cujo resultado é um JToken, que é a resposta do request.</returns>
        public static async Task<JToken> Get(HttpRequest requestOriginal, string endpointDestino)
        {
            return await FazRequest(requestOriginal, endpointDestino, "GET");
        }

        /// <summary>
        /// Faz um POST na API do PoçoWeb.
        /// </summary>
        /// <param name="requestOriginal">Objeto contendo o request inicial recebido pela API do SEST.</param>
        /// <param name="endpointDestino">String contendo o endpoint alvo (não deve conter barras no início nem no final)</param>
        /// <returns>Uma Task cujo resultado é um JToken, que é a resposta do request.</returns>
        public static async Task<JToken> Post(HttpRequest requestOriginal, string endpointDestino)
        {
            return await FazRequest(requestOriginal, endpointDestino, "POST");
        }                        
    }
}
