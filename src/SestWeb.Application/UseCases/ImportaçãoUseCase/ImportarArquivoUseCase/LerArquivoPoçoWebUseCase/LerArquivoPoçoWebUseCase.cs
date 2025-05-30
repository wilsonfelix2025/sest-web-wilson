using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;
using SestWeb.Application.Services;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Importadores.Shallow;
using SestWeb.Domain.Importadores.Shallow.Sest5;

namespace SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarArquivoUseCase.LerArquivoPoçoWebUseCase
{
    public class LerArquivoPoçoWebUseCase : ILerArquivoPoçoWebUseCase
    {
        private readonly IPocoWebService _service;
        private readonly ILeitorShallowArquivos _leitorShallowArquivos;

        public LerArquivoPoçoWebUseCase(IPocoWebService service, ILeitorShallowArquivos leitorShallowArquivos)
        {
            _service = service;
            _leitorShallowArquivos = leitorShallowArquivos;
        }

        public async Task<LerArquivoPoçoWebOutput> Execute(string urlArquivo, string token)
        {
            try
            {
                if (urlArquivo.Contains("?"))
                {
                    urlArquivo += "&render=json";
                }
                else
                {
                    urlArquivo += "?render=json";
                }

                
                var json = await _service.GetPoçoWebDTOFromWebByFile(urlArquivo, token);

                var poçoWeb = JsonConvert.DeserializeObject<PoçoWebDto>(json);
                
                if (poçoWeb == null)
                    return LerArquivoPoçoWebOutput.LeituraSemSucesso("Não foi possível encontrar o arquivo: " + urlArquivo);

                RetornoDTO retorno = _leitorShallowArquivos.LerArquivo(TipoArquivo.PoçoWeb, "", poçoWeb);

                return LerArquivoPoçoWebOutput.LeituraComSucesso(retorno, json);
            }
            catch (Exception e)
            {
                Log.Error("Erro ao importar do poço web. Arquivo: " + urlArquivo + ". Erro: " + e.Message);
                return LerArquivoPoçoWebOutput.LeituraSemSucesso(e.Message);
            }
            
        }
    }
}
