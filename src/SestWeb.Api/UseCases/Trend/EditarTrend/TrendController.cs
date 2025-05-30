using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.TrendUseCases.EditarTrend;

namespace SestWeb.Api.UseCases.Trend.EditarTrend
{
    [Route("api/editar-trend")]
    [ApiController]
    public class TrendController : ControllerBase
    {
        private readonly IEditarTrendUseCase _useCase;
        private readonly Presenter _presenter;

        public TrendController(IEditarTrendUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Edita o trend.
        /// </summary>
        /// <param name="request">Informações necessárias para a edição dos trechos do trend.</param>
        /// <returns>Trend editado.</returns>
        /// <response code="200">Retorna o trend editado.</response>
        /// <response code="400">O trend não foi editado.</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditarTrend(EditarTrendRequest request)
        {
            var input = PreencherInput(request);
            var output = await _useCase.Execute(input, request.IdPerfil, request.NomeTrend);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private List<EditarTrechosInput> PreencherInput(EditarTrendRequest request)
        {
            var listInput = new List<EditarTrechosInput>();

            foreach (var trecho in request.Trechos)
            {
                var input = new EditarTrechosInput
                {
                    ValorTopo = trecho.ValorTopo,
                    ValorBase = trecho.ValorBase,
                    Inclinação = trecho.Inclinação,
                    PvBase = trecho.PvBase,
                    PvTopo = trecho.PvTopo
                };

                listInput.Add(input);
            }

            return listInput;

        }
    }
}
