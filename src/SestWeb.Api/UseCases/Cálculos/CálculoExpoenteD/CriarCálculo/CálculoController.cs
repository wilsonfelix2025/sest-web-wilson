using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoExpoenteD.CriarCálculo;

namespace SestWeb.Api.UseCases.Cálculos.CálculoExpoenteD.CriarCálculo
{
    [Route("api/criar-calculo-expoented")]
    [ApiController]
    public class CálculoController : ControllerBase
    {
        private readonly ICriarCálculoExpoenteDUseCase _useCase;
        private readonly Presenter _presenter;

        public CálculoController(ICriarCálculoExpoenteDUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarCálculo(CriarCálculoExpoenteDRequest request)
        {
            var input = PreencherInput(request);

            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private CriarCálculoExpoenteDInput PreencherInput(CriarCálculoExpoenteDRequest request)
        {
            var input = new CriarCálculoExpoenteDInput();
            input.Correlação = request.Correlação;
            input.IdPoço = request.IdPoço;
            input.Nome = request.Nome;
            input.PerfilROPId = request.PerfilROPId;
            input.PerfilRPMId = request.PerfilRPMId;
            input.PerfilWOBId = request.PerfilWOBId;
            input.PerfilDIAM_BROCA = request.PerfilDIAM_BROCA;
            input.PerfilECDId = request.PerfilECDId;

            return input;
        }
    }
}
