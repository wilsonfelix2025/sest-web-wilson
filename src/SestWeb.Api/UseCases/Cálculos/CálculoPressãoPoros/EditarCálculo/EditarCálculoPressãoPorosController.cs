using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.EditarCálculo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPressãoPoros.EditarCálculo
{
    [Route("api/editar-calculo-pressao-poros")]
    [ApiController]
    public class EditarCálculoPressãoPorosController : ControllerBase
    {
        private readonly IEditarCálculoPressãoPorosUseCase _useCase;
        private readonly Presenter _presenter;

        public EditarCálculoPressãoPorosController(IEditarCálculoPressãoPorosUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditarCálculoPressãoPoros(EditarCálculoPressãoPorosRequest request)
        {
            var input = PreencherInput(request);
            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private EditarCálculoPressãoPorosInput PreencherInput(EditarCálculoPressãoPorosRequest request)
        {
            return new EditarCálculoPressãoPorosInput
            {
                Nome = request.Nome,
                IdPoço = request.IdPoço,
                Tipo = request.Tipo,
                Gn = request.Gn,
                Exp = request.Exp,
                IdPerfilFiltrado = request.IdPerfilFiltrado,
                IdGradienteSobrecarga = request.IdGradienteSobrecarga,
                CalculoReservatorio = request.CalculoReservatorio,
                Gpph = request.Gpph,
                IdPph = request.IdPph,
                IdCálculo = request.Id
            };
        }
    }
}
