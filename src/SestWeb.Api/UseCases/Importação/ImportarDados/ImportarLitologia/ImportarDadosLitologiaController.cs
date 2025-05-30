using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase.ImportarLitologiaUseCase;

namespace SestWeb.Api.UseCases.Importação.ImportarDados.ImportarLitologia
{
    [Route("api/importar-dados/litologia")]
    [ApiController]
    public class ImportarDadosLitologiaController : ControllerBase
    {
        private readonly ImportarDadosPresenter _presenter;
        private readonly IImportarDadosLitologiaUseCase _importarDadosUseCase;


        public ImportarDadosLitologiaController(IImportarDadosLitologiaUseCase importarDadosUseCase, ImportarDadosPresenter presenter)
        {
            _importarDadosUseCase = importarDadosUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Faz a importação de dados da planilha, a partir do tipo selecionado previamente.
        /// </summary>
        /// <param name="request">Informações necessárias para a importação dos dados da planilha.</param>
        /// <returns>O resultado da importação.</returns>
        /// <response code="200">Arquivo importado com sucesso.</response>
        /// <response code="400">O arquivo não foi importado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportarDadosLitologia(ImportarDadosLitologiaRequest request)
        {
            Log.Information("Recebendo importação de dados de litologia: " + request);

            var litologiaInput = PreencherInput(request);

            var output = await _importarDadosUseCase.Execute(request.PoçoId, litologiaInput);
            Log.Information("Finalizando importação de dados de litologia: " + request + " Output:" + output);
            _presenter.Populate(output);
            return _presenter.ViewModel;
        }

        private LitologiaInput PreencherInput(ImportarDadosLitologiaRequest request)
        {
            var litologia = new LitologiaInput
            {
                TipoProfundidade = request.Litologia.TipoProfundidade,
                TipoLitologia = request.Litologia.Tipo,
                Ação = request.Litologia.Ação,
                Nome = request.Litologia.Nome,
                NovoNome = request.Litologia.NovoNome,
                ValorBase = request.Litologia.ValorBase,
                ValorTopo = request.Litologia.ValorTopo,
                CorreçãoMesaRotativa = request.Litologia.CorreçãoMesaRotativa
            };

            foreach (var ponto in request.Litologia.PontosLitologia)
            {
                if (string.IsNullOrWhiteSpace(ponto.TipoRocha)
                    && ponto.Pm == null)
                    break;

                var pontoInput = new PontoLitologiaInput
                {
                    PM = ponto.Pm.Value,
                    TipoRocha = ponto.TipoRocha
                };

                litologia.PontosLitologia.Add(pontoInput);
            }

            return litologia;
        }

    }
}