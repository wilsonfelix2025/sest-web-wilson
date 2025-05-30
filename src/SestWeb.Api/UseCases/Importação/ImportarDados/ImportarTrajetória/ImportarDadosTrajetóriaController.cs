using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase.ImportarTrajetóriaUseCase;

namespace SestWeb.Api.UseCases.Importação.ImportarDados
{
    [Route("api/importar-dados/trajetoria")]
    [ApiController]
    public class ImportarDadosTrajetóriaController : ControllerBase
    {
        private readonly IImportarDadosTrajetóriaUseCase _importarDadosUseCase;
        private readonly ImportarDadosPresenter _presenter;

        public ImportarDadosTrajetóriaController(IImportarDadosTrajetóriaUseCase importarDadosUseCase, ImportarDadosPresenter presenter)
        {
            _importarDadosUseCase = importarDadosUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Faz a importação de dados de trajetória da planilha
        /// </summary>
        /// <param name="request">Informações necessárias para a importação dos dados de trajetória da planilha.</param>
        /// <returns>O resultado da importação.</returns>
        /// <response code="200">Arquivo importado com sucesso.</response>
        /// <response code="400">O arquivo não foi importado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportarDadosTrajetoria(ImportarDadosTrajetóriaRequest request)
        {
            Log.Information("Recebendo importação de dados de trajetória: " + request);

            var trajetória = PreencherInput(request);

            var output = await _importarDadosUseCase.Execute(request.PoçoId, trajetória);
            Log.Information("Finalizando importação de dados de trajetória: " + request + " Output:" + output);
            _presenter.Populate(output);
            return _presenter.ViewModel;
        }

        private TrajetóriaInput PreencherInput(ImportarDadosTrajetóriaRequest request)
        {
            var listaReqDistinct = request.PontosTrajetória.Distinct();
            var input = new TrajetóriaInput();

            input.Ação = request.Ação;
            input.ValorBase = request.ValorBase;
            input.ValorTopo = request.ValorTopo;

            foreach (var ponto in listaReqDistinct)
            {
                if (string.IsNullOrWhiteSpace(ponto.Inclinacao) 
                    && string.IsNullOrWhiteSpace(ponto.Azimute)
                    && string.IsNullOrWhiteSpace(ponto.PM))
                    break;

                var pontoInput = new PontoTrajetóriaInput
                {
                    Azimute = ponto.Azimute,
                    Inclinação = ponto.Inclinacao,
                    PM = ponto.PM
                };

                input.PontoTrajetória.Add(pontoInput);
            }

            return input;
        }
    }
}