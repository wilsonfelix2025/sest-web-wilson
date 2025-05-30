using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace SestWeb.Api.Helpers.IO
{
    public class Json
    {
        /// <summary>
        /// Abre um arquivo JSON e transfere o conteúdo para um objeto JSON.
        /// </summary>
        /// <param name="path">Caminho do arquivo a ser aberto.</param>
        /// <returns>O objeto JSON criado a partir do conteúdo do arquivo.</returns>
        public static JToken FromFile(string path)
        {
            // Extrai o texto do arquivo no caminho passado
            string content = File.ReadAllText($"{path}", Encoding.UTF8);
            // Monta o objeto JSON e retorna
            return JToken.Parse(content);
        }

        public static void KeysToLowercase(JToken token)
        {
            if (token is JObject)
            {
                // Extrai a lista de propriedades do objeto JSON
                JObject jsonObject = (JObject)token;
                List<JProperty> listaDePropriedades = new List<JProperty>(jsonObject.Properties());
                foreach (var propriedade in listaDePropriedades)
                {
                    // Se o value dessa key for um objeto, chama a função para essa entrada
                    if (propriedade.Value.Type == JTokenType.Object)
                    {
                        KeysToLowercase((JObject)propriedade.Value);
                    }

                    // Se o value dessa key for um array
                    if (propriedade.Value.Type == JTokenType.Array)
                    {
                        // Obtém o array
                        var arr = JArray.Parse(propriedade.Value.ToString());
                        // Itera sobre ele chamando a função para cada entrada
                        foreach (var pr in arr)
                        {
                            KeysToLowercase((JObject)pr);
                        }

                        // Atualiza o valor com o novo array
                        propriedade.Value = arr;
                    }

                    // Caso seja apenas uma string, faz a substituição diretamente
                    propriedade.Replace(new JProperty(propriedade.Name.ToLower(CultureInfo.InvariantCulture), propriedade.Value));
                }

                return;
            }

            if (token is JArray)
            {
                foreach (var elem in token)
                {
                    KeysToLowercase(elem);
                }
            }
        }
    }
}
