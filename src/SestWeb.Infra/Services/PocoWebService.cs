using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SestWeb.Application.Services;
using SestWeb.Domain.Entities.PoçoWeb.File;
using SestWeb.Domain.Entities.PoçoWeb.FormatUrl;
using SestWeb.Domain.Entities.PoçoWeb.OilField;
using SestWeb.Domain.Entities.PoçoWeb.OpUnit;
using SestWeb.Domain.Entities.PoçoWeb.ResponseItem;
using SestWeb.Domain.Entities.PoçoWeb.Well;
using Well = SestWeb.Domain.Entities.PoçoWeb.Well.Well;

namespace SestWeb.Infra.Services
{
    public class PocoWebService : IPocoWebService
    {
        private string urlDestino = $"https://pocoweb.petro.intelie.net/api/public/";
        private bool debug = false;

        public PocoWebService()
        {

        }

        public PocoWebService(string poçoWebUrl)
        {
            this.urlDestino = poçoWebUrl;
        }

        private async Task<dynamic> getResponseJson(string url, string authorization, string method)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = method;

            request.Headers.Add("Authorization", authorization);
            // Faz o request HTTP e retorna a resposta
            // Console.WriteLine("\nURL: {0}", url);
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)await request.GetResponseAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nErro ao contatar banco:\n " + e);
                return null;
            }
            string responseComoString = "";

            // Lê os dados da response
            using (var streamReader = new System.IO.StreamReader(response.GetResponseStream()))
            {
                responseComoString += streamReader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject(responseComoString);
        }

        private async Task<string> getResponseString(string url, string authorization, string method)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = method;

            request.Headers.Add("Authorization", authorization);
            // Faz o request HTTP e retorna a resposta
            // Console.WriteLine("\nURL: {0}", url);
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)await request.GetResponseAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nErro ao contatar banco:\n " + e);
                return null;
            }
            string responseComoString = "";

            // Lê os dados da response
            using (var streamReader = new System.IO.StreamReader(response.GetResponseStream()))
            {
                responseComoString += streamReader.ReadToEnd();
            }

            return responseComoString;
        }

        private async Task<dynamic> getResponseJson(string url, string authorization, object obj)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.Headers.Add("Authorization", authorization);

            using (var streamWriter = new System.IO.StreamWriter(request.GetRequestStream()))
            {
                var json = JsonConvert.SerializeObject(obj);
                streamWriter.Write(json);
            }

            // Faz o request HTTP e retorna a resposta
            // Console.WriteLine("\nURL: {0}", url);
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)await request.GetResponseAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro ao contatar banco: " + e);
                return null;
            }
            string responseComoString = "";

            // Lê os dados da response
            using (var streamReader = new System.IO.StreamReader(response.GetResponseStream()))
            {
                responseComoString += streamReader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject(responseComoString);
        }

        public async Task<Well> createWell(WellRequest well, string authorization)
        {
            dynamic jsonObj = await getResponseJson(urlDestino + "wells/", authorization, well);
            if (jsonObj == null)
            {
                return null;
            }

            return new Well(jsonObj.url.Value, jsonObj.name.Value, jsonObj.oilfield.url);
        }

        public async Task<File> createFile(FileRequest file, string authorization)
        {
            dynamic jsonObj = await getResponseJson(urlDestino + "files/", authorization, file);
            if (jsonObj == null)
            {
                return null;
            }

            return new File(jsonObj.url.Value, jsonObj.name.Value, jsonObj.well.url, jsonObj.file_type);
        }

        public async Task<bool> deleteFile(string id, string authorization)
        {
            dynamic jsonObj = await getResponseJson(urlDestino + "files/" + id + '/', authorization, "DELETE");
            if (jsonObj == null)
            {
                return false;
            }
            return true;
        }

        private void debugMessage(string message, params object[] arg)
        {
            if (debug)
            {
                Console.WriteLine(message, arg);
            }
        }

        private async Task<List<IPoçoWebResponseItem>> GetFromWeb<T>(string url, string authorization)
        {
            List<IPoçoWebResponseItem> list = new List<IPoçoWebResponseItem>();

            dynamic jsonObj = await getResponseJson(url, authorization, "GET");
            string aux = "";
            IPoçoWebResponseItem obj = null;
            foreach (var item in jsonObj)
            {
                if (typeof(T) == typeof(OpUnit))
                {
                    obj = new OpUnit(item.url.Value, item.name.Value);
                }
                else if (typeof(T) == typeof(OilField))
                {
                    aux = FormatUrl.Execute(item.opunit.url.Value)[0];
                    obj = new OilField(item.url.Value, item.name.Value, aux);
                }
                else if (typeof(T) == typeof(Well))
                {
                    aux = FormatUrl.Execute(item.oilfield.url.Value)[0];
                    obj = new Well(item.url.Value, item.name.Value, aux);
                }
                debugMessage("| URL: {0} | | Nome: {1} | | Pai: {2} |", item.url.Value, item.name.Value, aux);
                list.Add(obj);
            }

            return list;
        }

        public async Task<List<OpUnit>> GetOpUnitsFromWeb(string authorization)
        {
            debugMessage("\nOpUnits\n");
            List<OpUnit> opUnits = (await GetFromWeb<OpUnit>(urlDestino + "opunits/", authorization)).ConvertAll(o => (OpUnit)o);
            debugMessage("END\n");

            return opUnits;
        }

        public async Task<List<OilField>> GetOilFieldsFromWeb(string authorization, List<OpUnit> opUnits)
        {
            debugMessage("\nOilField\n");
            List<OilField> oilFields = (await GetFromWeb<OilField>(urlDestino + "oilfields/", authorization)).ConvertAll(o => (OilField)o);

            foreach (var item in oilFields)
            {
                opUnits.Find(x => x.Id == item.OpUnitId).OilFields.Add(item.Id);
            }
            debugMessage("END\n");

            return oilFields;
        }

        public async Task<List<Well>> GetWellsFromWeb(string authorization, List<OilField> oilFields)
        {
            debugMessage("\nWells\n");
            List<Well> wells = (await GetFromWeb<Well>(urlDestino + "wells/", authorization)).ConvertAll(o => (Well)o);

            foreach (var item in wells)
            {
                oilFields.Find(x => x.Id == item.OilFieldId).Wells.Add(item.Id);
            }
            debugMessage("END\n");

            return wells;
        }

        public async Task<List<File>> GetFilesFromWeb(string authorization, List<Well> wells)
        {
            debugMessage("\nFiles\n");
            List<File> files = new List<File>();
            dynamic jsonObj;

            File file;
            foreach (Well w in wells)
            {
                jsonObj = await getResponseJson(urlDestino + "well/" + w.Id + "/", authorization, "GET");
                if (jsonObj == null)
                {
                    debugMessage("| --- ERRO: id {0} --- |\n", w.Id);
                    continue;
                }
                foreach (var item in jsonObj.files)
                {
                    debugMessage("| URL: {0} | | Well: {1} | | Name: {2}", item.url.Value, w.Id, item.name.Value);
                    if (item.file_type.Value == "sesttr.project" || item.file_type.Value == "sesttr.monitoring" || item.file_type.Value == "sesttr.retroanalysis")
                    {
                        file = new File(item.url.Value, item.name.Value, w.Url, item.file_type.Value);
                        files.Add(file);

                        w.Files.Add(file.Id);
                    }
                }
            }
            debugMessage("END\n");

            return files;
        }

        public async Task<List<FileDTO>> GetFilesByTypeFromWeb(string authorization, string tipoArquivo)
        {
            var json = await getResponseString(urlDestino + "files/?render=json&file_type=dadosbasicos." + tipoArquivo, authorization, "GET");

            if (json == null)
                return null;

            //foreach (var item in jsonObj)
            //{
            //    file = new File((item as JObject)["url"].Value<string>(), (item as JObject)["name"].Value<string>(), (item as JObject)["url"].Value<string>(), (item as JObject)["file_type"].Value<string>());
            //    files.Add(file);
            //}

            //foreach (var item in jsonObj)
            //{
            //    file = new FileDTO((item as JObject)["name"].Value<string>(), (item as JObject)["url"].Value<string>());
            //    files.Add(file);
            //}

            var files = JsonConvert.DeserializeObject<List<FileDTO>>(json);

            return files;
        }

        public async Task<string> GetFileRevisions(string urlRevisions, string authorization)
        {
            WebRequest request = WebRequest.Create(urlRevisions);
            request.Method = "GET";
            request.Headers.Add("Authorization", authorization);
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)await request.GetResponseAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nErro ao contatar banco:\n " + e);
                return null;
            }
            string responseComoString = "";

            // Lê os dados da response
            using (var streamReader = new System.IO.StreamReader(response.GetResponseStream()))
            {
                responseComoString += streamReader.ReadToEnd();
            }

            var jsonObj = responseComoString;

            return jsonObj;

        }

        public async Task<string> GetPoçoWebDTOFromWebByFile(string urlArquivo, string authorization)
        {

            WebRequest request = WebRequest.Create(urlArquivo);
            request.Method = "GET";
            request.Headers.Add("Authorization", authorization);
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)await request.GetResponseAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nErro ao contatar banco:\n " + e);
                return null;
            }
            string responseComoString = "";

            // Lê os dados da response
            using (var streamReader = new System.IO.StreamReader(response.GetResponseStream()))
            {
                responseComoString += streamReader.ReadToEnd();
            }

            try
            {
                //var jsonObj = JsonConvert.DeserializeObject<PoçoWebDto>(responseComoString);
                var jsonObj = responseComoString;
                if (jsonObj == null)
                    return null;
                return jsonObj;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }
    }

}