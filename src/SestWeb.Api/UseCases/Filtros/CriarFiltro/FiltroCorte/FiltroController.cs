using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.FiltrosUseCase.CriarFiltro.FiltroCorte;
using SestWeb.Domain.Entities.Cálculos.Filtros;

namespace SestWeb.Api.UseCases.Filtros.CriarFiltro.FiltroCorte
{
    [Route("api/criar-filtro-corte")]
    [ApiController]
    public class FiltroController : ControllerBase
    {
        private readonly ICriarFiltroCorteUseCase _useCase;
        private readonly Presenter _presenter;

        public FiltroController(ICriarFiltroCorteUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarFiltro(FiltroCorteRequest request)
        {
            var input = PreencherInput(request);
            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private CriarFiltroCorteInput PreencherInput(FiltroCorteRequest request)
        {
            var input = new CriarFiltroCorteInput
            {
                LimiteInferior = request.LimiteInferior,
                LimiteSuperior = request.LimiteSuperior,
                IdPerfil = request.IdPerfil,
                TipoCorte = request.TipoCorte,
                TipoFiltro = TipoFiltroEnum.FiltroCorte,
                Nome = request.Nome
            };

            return input;
        }

    }
}
