using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarArquivoUseCase;

namespace SestWeb.Api.UseCases.Importação.ImportarArquivo
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportarArquivoController : ControllerBase
    {
        private readonly IImportarArquivoUseCase _importarArquivoUseCase;
        private readonly ImportarArquivoPresenter _presenter;

        public ImportarArquivoController(IImportarArquivoUseCase importarArquivoUseCase,
            ImportarArquivoPresenter presenter)
        {
            _importarArquivoUseCase = importarArquivoUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Faz a importação de um arquivo, a partir dos dados selecionados previamente.
        /// </summary>
        /// <param name="request">Informações necessárias para a importação do arquivo.</param>
        /// <returns>O caminho do arquivo e o tamanho em bytes.</returns>
        /// <response code="200">Arquivo importado com sucesso.</response>
        /// <response code="400">O arquivo não foi importado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportarArquivo(DadoParaImportarRequest request)
        {
            Log.Information("Recebendo importação de arquivo: " + request);
            var listaPerfis = PreencherPerfilModel(request);
            var listaLitologias = PreencherLitologiaModel(request);
            var token = Request != null ? Request.Headers["Authorization"].ToString() : string.Empty;

            if (request.Extras.ContainsKey("poçoWeb"))
                request.Extras.Add("token",token);

            var output = await _importarArquivoUseCase.Execute(request.DadosSelecionados, request.CaminhoDoArquivo,
                listaPerfis, listaLitologias, request.PoçoId, request.CorreçãoMesaRotativa, request.Extras);
            Log.Information("Finalizando importação de arquivo: " + request + " Output:" + output);
            _presenter.Populate(output);
            return _presenter.ViewModel;
        }

        private static List<LitologiaModel> PreencherLitologiaModel(DadoParaImportarRequest request)
        {
            var list = new List<LitologiaModel>();

            foreach (var reqLit in request.ListaLitologias)
            {
                var lit = new LitologiaModel
                {
                    Ação = reqLit.Ação,
                    ValorBase = reqLit.ValorBase,
                    ValorTopo = reqLit.ValorTopo,
                    Nome = reqLit.Nome,
                    NovoNome = reqLit.NovoNome,
                    Tipo = reqLit.Tipo
                };

                list.Add(lit);
            }

            return list;
        }

        private static List<PerfilModel> PreencherPerfilModel(DadoParaImportarRequest request)
        {
            var list = new List<PerfilModel>();

            foreach (var reqProfile in request.ListaPerfis)
            {
                var profile = new PerfilModel
                {
                    Tipo = reqProfile.Tipo,
                    Unidade = reqProfile.Unidade,
                    Ação = reqProfile.Ação,
                    ValorBase = reqProfile.ValorBase,
                    ValorTopo = reqProfile.ValorTopo,
                    Nome = reqProfile.Nome,
                    NovoNome = reqProfile.NovoNome
                };

                list.Add(profile);
            }

            return list;
        }
    }
}