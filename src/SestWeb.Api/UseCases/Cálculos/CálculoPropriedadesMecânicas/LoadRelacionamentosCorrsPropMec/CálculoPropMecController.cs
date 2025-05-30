using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.LoadRelacionamentosCorrsPropMec;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.LoadRelacionamentosCorrsPropMec
{
    [Route("api/propmec/load-system-relationships")]
    [ApiController]
    public class CálculoPropMecController : ControllerBase
    {
        private readonly ILoadRelacionamentosCorrsPropMecUseCase _loadRelacionamentosCorrsPropMecUseCase;
        private readonly Presenter _presenter;

        public CálculoPropMecController(ILoadRelacionamentosCorrsPropMecUseCase loadRelacionamentosCorrsPropMecUseCase, Presenter presenter)
        {
            _loadRelacionamentosCorrsPropMecUseCase = loadRelacionamentosCorrsPropMecUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Carrega ou atualiza os relacionamentos de propriedades mecânicas do sistema.
        /// </summary>
        /// <response code="200">Retorna todos relacionamentos carregados.</response>
        /// <response code="400">Não foi possível carregar os relacionamentos.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LoadSystemPropMecRelationships(string idPoço)
        {
            var output = await _loadRelacionamentosCorrsPropMecUseCase.Execute(idPoço);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
