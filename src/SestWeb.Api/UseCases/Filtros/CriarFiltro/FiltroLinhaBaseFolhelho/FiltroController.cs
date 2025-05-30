
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.FiltrosUseCase.CriarFiltro.FiltroLinhaBaseFolhelho;
using SestWeb.Domain.Entities.Cálculos.Filtros;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Filtros.CriarFiltro.FiltroLinhaBaseFolhelho
{
    [Route("api/criar-filtro-lbf")]
    [ApiController]
    public class FiltroController : ControllerBase
    {
        private readonly ICriarFiltroLinhaBaseFolhelhoUseCase _useCase;
        private readonly Presenter _presenter;

        public FiltroController(ICriarFiltroLinhaBaseFolhelhoUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarFiltro(FiltroLinhaBaseFolhelhoRequest request)
        {
            var input = PreencherInput(request);
            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private CriarFiltroLinhaBaseFolhelhoInput PreencherInput(FiltroLinhaBaseFolhelhoRequest request)
        {
            var input = new CriarFiltroLinhaBaseFolhelhoInput
            {
                IdLBF = request.IdLBF,
                LimiteInferior = request.LimiteInferior,
                LimiteSuperior = request.LimiteSuperior,
                IdPerfil = request.IdPerfil,
                TipoCorte = request.TipoCorte,
                TipoFiltro = TipoFiltroEnum.FiltroLinhaBaseFolhelho,
                Nome = request.Nome
            };

            return input;
        }
    }
}
