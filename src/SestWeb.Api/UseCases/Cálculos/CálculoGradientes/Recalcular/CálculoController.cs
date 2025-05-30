using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoGradientes.Recalcular;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Cálculos.CálculoGradientes.Recalcular
{
    [Route("api/recalcular-calculo-gradientes")]
    [ApiController]
    public class CálculoController : ControllerBase
    {
        private readonly IRecalcularCálculoGradientesUseCase _useCase;
        private readonly Presenter _presenter;

        public CálculoController(IRecalcularCálculoGradientesUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Recalcular(RecalcularCálculoGradientesRequest request)
        {
            var input = PreencherInput(request);

            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private RecalcularCálculoGradientesInput PreencherInput(RecalcularCálculoGradientesRequest request)
        {
            var input = new RecalcularCálculoGradientesInput
            {
                IdPoço = request.IdPoço,
                IdCálculo = request.IdCálculo                
            };

            return input;
        }       
    }
}
