using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoSobrecarga.CriarCálculo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Cálculos.CálculoSobrecarga.CriarCálculo
{
    [Route("api/criar-calculo-sobrecarga")]
    [ApiController]
    public class CriarCálculoSobrecargaController : ControllerBase
    {
        private readonly ICriarCálculoSobrecargaUseCase _useCase;
        private readonly Presenter _presenter;

        public CriarCálculoSobrecargaController(ICriarCálculoSobrecargaUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarCálculoSobrecarga(CriarCálculoSobrecargaRequest request)
        {
            var input = PreencherInput(request);
            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private CriarCálculoSobrecargaInput PreencherInput(CriarCálculoSobrecargaRequest request)
        {
            return new CriarCálculoSobrecargaInput
            {
                IdPoço = request.IdPoço,
                IdRhob = request.IdRhob,
                Nome = request.Nome
            };
        }
    }
}
