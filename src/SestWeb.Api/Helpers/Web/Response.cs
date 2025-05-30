using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;

namespace SestWeb.Api.Helpers.Web
{
    public class Response
    {
        /// <summary>
        /// Serializa um objeto WebResponse como um JSON.
        /// </summary>
        /// <param name="response">A resposta recebida de um endpoint.</param>
        /// <returns>Um JToken contendo a resposta</returns>
        public static JToken ToJson(WebResponse response)
        {
            string responseComoString = "";

            // Lê os dados da response
            using (var streamReader = new StreamReader(response.GetResponseStream()))
                responseComoString += streamReader.ReadToEnd();

            // Se a string não for um bloco HTML
            if (!responseComoString.StartsWith("<"))
            {
                // Faz um parse JSON e retorna o objeto resultante
                return JToken.Parse(responseComoString);
            }

            // Do contrário, cria um novo objeto JSON
            JObject json = new JObject
            {
                // Adiciona a resposta ao campo "response" do JSON
                { "response", responseComoString }
            };

            return json;
        }
    }
}
