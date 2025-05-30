using Serilog;
using System;
using System.Threading.Tasks;
using SestWeb.Application.Services;

namespace SestWeb.Application.UseCases.PoçoWeb.GetFiles
{
    internal class GetFilesUseCase : IGetFilesUseCase
    {
        private readonly IPocoWebService _service;
        public GetFilesUseCase(IPocoWebService service)
        {
            _service = service;
        }

        public async Task<GetFilesOutput> Execute(string token, string tipoArquivo)
        {
            try
            {
                Log.Information("Bucando arquivos do poçoweb");

                if (string.IsNullOrWhiteSpace(tipoArquivo))
                    tipoArquivo = "drilling";

                //token = "XHJjWiwAAXse9vZwkkbt6OzlRHjA75";

                var listaArquivos = await _service.GetFilesByTypeFromWeb(token, tipoArquivo);
                
                if (listaArquivos == null)
                    return GetFilesOutput.BuscaSemSucesso("Arquivos do tipo " + tipoArquivo + " não encontrado");

                return GetFilesOutput.BuscaComSucesso(listaArquivos);
            }
            catch (Exception e)
            {
                Log.Error("Erro ao buscar arquivos do poçoweb " + e.Message);
                return GetFilesOutput.BuscaSemSucesso(e.Message);
            }
        }
    }
}
