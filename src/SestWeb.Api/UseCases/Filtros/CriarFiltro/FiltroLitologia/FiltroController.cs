using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.FiltrosUseCase.CriarFiltro.FiltroLitologia;
using SestWeb.Domain.Entities.Cálculos.Filtros;

namespace SestWeb.Api.UseCases.Filtros.CriarFiltro.FiltroLitologia
{
    [Route("api/criar-filtro-litologia")]
    [ApiController]
    public class FiltroController : ControllerBase
    {
        private readonly ICriarFiltroLitologiaUseCase _useCase;
        private readonly Presenter _presenter;

        public FiltroController(ICriarFiltroLitologiaUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarFiltro(FiltroLitologiaRequest request)
        {
            var input = PreencherInput(request);
            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private CriarFiltroLitologiaInput PreencherInput(FiltroLitologiaRequest request)
        {
            var input = new CriarFiltroLitologiaInput
            {
                LitologiasSelecionadas = request.LitologiasSelecionadas,
                LimiteInferior = request.LimiteInferior,
                LimiteSuperior = request.LimiteSuperior,
                IdPerfil = request.IdPerfil,
                TipoCorte = request.TipoCorte,
                TipoFiltro = TipoFiltroEnum.FiltroLitologia,
                Nome = request.Nome
            };

            return input;
        }
    }
}
