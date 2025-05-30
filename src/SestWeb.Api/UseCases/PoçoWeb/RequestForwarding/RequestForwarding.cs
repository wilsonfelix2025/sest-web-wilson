using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using SestWeb.Api.Helpers.IO;
using SestWeb.Domain.Entities.PoçoWeb.OilField;
using SestWeb.Domain.Entities.PoçoWeb.OpUnit;
using SestWeb.Domain.Entities.PoçoWeb.ParentWell;
using SestWeb.Domain.Entities.PoçoWeb.ResponseItem;

namespace SestWeb.Api.UseCases.PoçoWeb.RequestForwarding
{
    [Route("")]
    [ApiController]
    public class RequestForwardingController : ControllerBase
    {
        private readonly string _caminhoBaseDeArquivosJson = $"UseCases/PoçoWeb/RequestForwarding/Data";

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("api/public/opunits")]
        public JToken ListarUnidadesOperacionais()
        {
            return Json.FromFile($"{_caminhoBaseDeArquivosJson}/opunits.json");
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("api/public/opunit/{id}")]
        public JToken ObterUnidadeOperacional(string id)
        {
            return PesquisarPorID<OpUnit>("opunits", id);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("api/public/oilfield/{id}")]
        public JToken ObterCampo(string id)
        {
            return PesquisarPorID<OilField>("oilfields", id);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("api/public/well/{id}")]
        public JToken ObterPoço(string id)
        {
            return PesquisarPorID<ParentWell>("fullWells", id);
        }

        /// <summary>
        /// Endpoint que encaminha requests recebidos para o PoçoWeb.
        /// </summary>        
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("api/public/filestree")]
        public async Task<JToken> ListarArquivosEmÁrvore(string endpoint, string queryParams)
        {
            return await PoçoWeb.Get(Request, "api/public/files/?format=tree");
        }

        /// <summary>
        /// Encaminha GET requests recebidos pela API local para o PoçoWeb.
        /// </summary>        
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("o/{*endpoint}")]
        public async Task<JToken> EncaminharGetRequest(string endpoint, string queryParams)
        {
            return await PoçoWeb.Get(Request, $"o/{endpoint}");
        }

        /// <summary>
        /// Encaminha POST requests recebidos pela API local para o PoçoWeb.
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("o/{*endpoint}")]
        public async Task<JToken> EncaminharPostRequest(string endpoint, string queryParams)
        {
            return await PoçoWeb.Post(Request, $"o/{endpoint}");
        }

        private JToken PesquisarPorID<T>(string nomeArquivo, string id) where T : IPoçoWebResponseItem
        {
            JToken jsonDoArquivo = Json.FromFile($"{_caminhoBaseDeArquivosJson}/{nomeArquivo}.json");
            List<T> list = jsonDoArquivo.ToObject<List<T>>();
            T element = list.Find(x => x.Url.EndsWith($"/{id}/"));
            JToken objetoRetorno = JToken.FromObject(element);
            Json.KeysToLowercase(objetoRetorno);
            return objetoRetorno;
        }
    }
}
