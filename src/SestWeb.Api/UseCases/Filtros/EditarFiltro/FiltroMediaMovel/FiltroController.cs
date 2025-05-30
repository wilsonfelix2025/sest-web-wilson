
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.FiltrosUseCase.EditarFiltro.FiltroMediaMovel;
using SestWeb.Domain.Entities.Cálculos.Filtros;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Filtros.EditarFiltro.FiltroMediaMovel
{
    [Route("api/editar-filtro-media-movel")]
    [ApiController]
    public class FiltroController : ControllerBase
    {
        private readonly IEditarFiltroMediaMovelUseCase _useCase;
        private readonly Presenter _presenter;

        public FiltroController(IEditarFiltroMediaMovelUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditarFiltro(FiltroMediaMovelRequest request)
        {
            var input = PreencherInput(request);
            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private EditarFiltroMediaMovelInput PreencherInput(FiltroMediaMovelRequest request)
        {
            var input = new EditarFiltroMediaMovelInput
            {
                NúmeroPontos = request.NúmeroPontos,
                LimiteInferior = request.LimiteInferior,
                LimiteSuperior = request.LimiteSuperior,
                IdPerfil = request.IdPerfil,
                TipoCorte = request.TipoCorte,
                TipoFiltro = TipoFiltroEnum.FiltroMédiaMóvel,
                Nome = request.Nome
            };

            return input;
        }
    }
}
