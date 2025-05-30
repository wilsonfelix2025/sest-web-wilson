using System.Collections.Generic;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.PoçoWeb.File;

namespace SestWeb.Application.UseCases.PoçoWeb.GetFiles
{
    public class GetFilesOutput : UseCaseOutput<GetFilesStatus>
    {
        public List<FileDTO> ListaArquivos { get; set; }

        public GetFilesOutput()
        {
            
        }

        public static GetFilesOutput BuscaComSucesso(List<FileDTO> listaArquivos)
        {
            return new GetFilesOutput
            {
                Status = GetFilesStatus.BuscaComSucesso,
                Mensagem = "Busca dos arquivos poçoweb feita com sucesso",
                ListaArquivos = listaArquivos
            };
        }

        public static GetFilesOutput BuscaSemSucesso(string msg = "")
        {
            return new GetFilesOutput
            {
                Status = GetFilesStatus.BuscaSemSucesso,
                Mensagem = "Não foi possível buscar arquivos do poçoweb. " + msg
            };
        }
    }
}
