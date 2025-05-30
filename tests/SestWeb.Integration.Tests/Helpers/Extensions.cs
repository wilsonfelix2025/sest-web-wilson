using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SestWeb.Integration.Tests.Helpers
{
    public static class Extensions
    {
        public async static Task<JObject> ToJObject(this HttpResponseMessage response)
        {
            return (JObject)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

        }

        public async static Task<JArray> ToJObjects(this HttpResponseMessage response)
        {
            return JArray.Parse(await response.Content.ReadAsStringAsync());

        }
    }
}
