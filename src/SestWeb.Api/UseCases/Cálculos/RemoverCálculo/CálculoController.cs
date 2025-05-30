using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.RemoverCálculo;

namespace SestWeb.Api.UseCases.Cálculos.RemoverCálculo
{
    [Route("api/remover-calculo")]
    [ApiController]
    public class CálculoController : ControllerBase
    {
        private readonly IRemoverCálculoUseCase _useCase;
        private readonly Presenter _presenter;

        public CálculoController(IRemoverCálculoUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoverCálculo(string idPoço, string idCálculo)
        {
            var output = await _useCase.Execute(idPoço, idCálculo);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

    }
}
