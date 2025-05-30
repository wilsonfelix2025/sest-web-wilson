using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SestWeb.Application.UseCases.InserirTrechoUseCase;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.InserirTrecho
{
    [Route("api/inserir-trecho-inicial")]
    [ApiController]
    public class InserirTrechoController : ControllerBase
    {
        private readonly IInserirTrechoUseCase _useCase;
        private readonly Presenter _presenter;

        public InserirTrechoController(IInserirTrechoUseCase useCase, Presenter presenter)
        {
            _presenter = presenter;
            _useCase = useCase;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InserirTrecho(InserirTrechoRequest request)
        {
            Log.Information("Recebendo dados para inserir trecho: " + request);

            var trechoInicialInput = PreencherInput(request);

            var output = await _useCase.Execute(request.PerfilId, trechoInicialInput);
            Log.Information("Finalizando trecho: " + request + " Output:" + output);
            _presenter.Populate(output);
            return _presenter.ViewModel; 
        }

        private InserirTrechoInput PreencherInput(InserirTrechoRequest request)
        {
            var input = new InserirTrechoInput
            {
                TipoTratamento = request.TipoTratamento,
                LitologiasSelecionadas = request.LitologiasSelecionadas,
                PMLimite = request.PMLimite,
                TipoDeTrecho = request.TipoDeTrecho,
                NomeNovoPerfil = request.NomeNovoPerfil
            };

            return input;
        }
    }
}
