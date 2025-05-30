
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.FiltrosUseCase.CriarFiltro.FiltroMediaMovel;
using SestWeb.Domain.Entities.Cálculos.Filtros;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Filtros.CriarFiltro.FiltroMediaMovel
{
    [Route("api/criar-filtro-media-movel")]
    [ApiController]
    public class FiltroController : ControllerBase
    {
        private readonly ICriarFiltroMediaMovelUseCase _useCase;
        private readonly Presenter _presenter;

        public FiltroController(ICriarFiltroMediaMovelUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarFiltro(FiltroMediaMovelRequest request)
        {
            var input = PreencherInput(request);
            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private CriarFiltroMediaMovelInput PreencherInput(FiltroMediaMovelRequest request)
        {
            var input = new CriarFiltroMediaMovelInput
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
