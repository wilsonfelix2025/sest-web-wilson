using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Web;

namespace SestWeb.Api.Helpers.Web
{
    public class Form
    {
        /// <summary>
        /// Encoda um form enviado em um POST como uma URL de parâmetros.
        /// 
        /// Ex.: um form contendo {nome: "SEST", sobrenome: "WEB"} vira
        /// nome=SEST
        /// </summary>
        /// <param name="form">Um dict contendo os pares chave-valor que compõem o form.</param>
        /// <returns>O form encodado como uma URL.</returns>
        public static string EncodeAsUrl(IFormCollection form)
        {
            // Inicia uma lista vazia que receberá cada par^
            List<string> parametros = new List<string>();

            foreach (string key in form.Keys)
            {
                parametros.Add($"{key}={HttpUtility.UrlEncode(form[key])}");
            }

            return string.Join('&', parametros.ToArray());
        }
    }
}
