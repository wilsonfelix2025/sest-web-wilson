using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoSobrecarga.EditarCálculo;
using Microsoft.AspNetCore.Http;

namespace SestWeb.Api.UseCases.Cálculos.CálculoSobrecarga.EditarCálculo
{
    [Route("api/editar-calculo-sobrecarga")]
    [ApiController]
    public class EditarCálculoSobrecargaController
    {
        private readonly IEditarCálculoSobrecargaUseCase _useCase;
        private readonly Presenter _presenter;

        public EditarCálculoSobrecargaController(IEditarCálculoSobrecargaUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarCálculoSobrecarga(EditarCálculoSobrecargaRequest request)
        {
            var input = PreencherInput(request);
            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private EditarCálculoSobrecargaInput PreencherInput(EditarCálculoSobrecargaRequest request)
        {
            return new EditarCálculoSobrecargaInput
            {
                IdPoço = request.IdPoço,
                IdRhob = request.IdRhob,
                Nome = request.Nome,
                IdCálculoAntigo = request.IdCálculoAntigo
            };
        }
    }
}
