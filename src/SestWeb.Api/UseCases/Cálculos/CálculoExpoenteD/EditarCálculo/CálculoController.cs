using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoExpoenteD.EditarCálculo;

namespace SestWeb.Api.UseCases.Cálculos.CálculoExpoenteD.EditarCálculo
{
    [Route("api/editar-calculo-expoented")]
    [ApiController]
    public class CálculoController : ControllerBase
    {
        private readonly IEditarCálculoExpoenteDUseCase _useCase;
        private readonly Presenter _presenter;

        public CálculoController(IEditarCálculoExpoenteDUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditarCálculo(EditarCálculoExpoenteDRequest request)
        {
            var input = PreencherInput(request);

            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private EditarCálculoExpoenteDInput PreencherInput(EditarCálculoExpoenteDRequest request)
        {
            var input = new EditarCálculoExpoenteDInput();
            input.Correlação = request.Correlação;
            input.IdPoço = request.IdPoço;
            input.Nome = request.Nome;
            input.PerfilROPId = request.PerfilROPId;
            input.PerfilRPMId = request.PerfilRPMId;
            input.PerfilWOBId = request.PerfilWOBId;
            input.PerfilDIAM_BROCA = request.PerfilDIAM_BROCA;
            input.PerfilECDId = request.PerfilECDId;
            input.IdCálculo = request.IdCálculo;

            return input;
        }
    }
}
