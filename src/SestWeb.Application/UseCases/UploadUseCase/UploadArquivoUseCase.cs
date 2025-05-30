using System;
using System.Threading.Tasks;
using SestWeb.Application.Services;
using SestWeb.Domain.Importadores.Shallow;
using SestWeb.Domain.Importadores.Shallow.Sest5;

namespace SestWeb.Application.UseCases.UploadUseCase
{
    internal class UploadArquivoUseCase : IUploadArquivoUseCase
    {
        private readonly IFileService _fileService;
        private readonly ILeitorShallowArquivos _leitorShallowArquivos;

        public UploadArquivoUseCase(IFileService fileService, ILeitorShallowArquivos leitorShallowArquivos)
        {
            _fileService = fileService;
            _leitorShallowArquivos = leitorShallowArquivos;
        }

        public async Task<UploadArquivoOutput> Execute(string extensão, byte[] stream)
        {
            try
            {
                var caminho = await _fileService.SalvarArquivo(extensão, stream);
                RetornoDTO retorno = null;
                if (extensão == ".xml")
                {
                    retorno = _leitorShallowArquivos.LerArquivo(TipoArquivo.Sest5, caminho);
                }
                else if (extensão == ".txt")
                {
                    retorno = _leitorShallowArquivos.LerArquivo(TipoArquivo.Sigeo, caminho);
                }
                else if (extensão == ".las")
                {
                    retorno = _leitorShallowArquivos.LerArquivo(TipoArquivo.Las, caminho);
                }
                else if (extensão == ".xsrt")
                {
                    retorno = _leitorShallowArquivos.LerArquivo(TipoArquivo.SestTr1, caminho);
                } 
                else if (extensão == ".xsrt2")
                {
                    retorno = _leitorShallowArquivos.LerArquivo(TipoArquivo.SestTr2, caminho);
                }

                return UploadArquivoOutput.UploadRealizado(caminho, retorno);
            }
            catch (Exception e)
            {
                return UploadArquivoOutput.UploadNãoRealizado(e.Message);
            }
        }
    }
}
