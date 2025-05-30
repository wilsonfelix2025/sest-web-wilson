using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.FiltrosUseCase.CriarFiltro.FiltroSimples;
using SestWeb.Domain.Entities.Cálculos.Filtros;

namespace SestWeb.Api.UseCases.Filtros.CriarFiltro.FiltroSimples
{
    [Route("api/criar-filtro-simples")]
    [ApiController]
    public class FiltroController : ControllerBase
    {
        private readonly ICriarFiltroSimplesUseCase _useCase;
        private readonly Presenter _presenter;

        public FiltroController(ICriarFiltroSimplesUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarFiltro(FiltroSimplesRequest request)
        {
            var input = PreencherInput(request);
            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private CriarFiltroSimplesInput PreencherInput(FiltroSimplesRequest request)
        {
            var input = new CriarFiltroSimplesInput
            {
                DesvioMáximo = request.DesvioMáximo,
                LimiteInferior = request.LimiteInferior,
                LimiteSuperior = request.LimiteSuperior,
                IdPerfil = request.IdPerfil,
                TipoCorte = request.TipoCorte,
                TipoFiltro = TipoFiltroEnum.FiltroSimples,
                Nome = request.Nome
            };

            return input;
        }

    }
}
