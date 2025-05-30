using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.FiltrosUseCase.EditarFiltro.FiltroLitologia;
using SestWeb.Domain.Entities.Cálculos.Filtros;

namespace SestWeb.Api.UseCases.Filtros.EditarFiltro.FiltroLitologia
{
    [Route("api/editar-filtro-litologia")]
    [ApiController]
    public class FiltroController : ControllerBase
    {
        private readonly IEditarFiltroLitologiaUseCase _useCase;
        private readonly Presenter _presenter;

        public FiltroController(IEditarFiltroLitologiaUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditarFiltro(FiltroLitologiaRequest request)
        {
            var input = PreencherInput(request);
            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private EditarFiltroLitologiaInput PreencherInput(FiltroLitologiaRequest request)
        {
            var input = new EditarFiltroLitologiaInput
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
