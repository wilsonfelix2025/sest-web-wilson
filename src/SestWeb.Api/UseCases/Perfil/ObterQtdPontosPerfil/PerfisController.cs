using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PerfilUseCases.ObterQtdPontosPerfil;
using SestWeb.Domain.Enums;

namespace SestWeb.Api.UseCases.Perfil.ObterQtdPontosPerfil
{
    [Route("api/perfis/obter-qtd-ponto")]
    [ApiController]
    public class PerfisController : ControllerBase
    {
        private readonly IObterQtdPontosPerfilUseCase _useCase;
        private readonly Presenter _presenter;

        public PerfisController(IObterQtdPontosPerfilUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém a quantidade de pontos do perfil, de acordo com os parametros passados.
        /// </summary>
        /// <returns>Coleção contendo os perfis do poço indicado que podem ser usados na funcionalidade.</returns>
        /// <response code="200">Retorna a quantidade de pontos do perfil, de acordo com os parametros passados.</response>
        /// <response code="400">Não foi possível obter a quantidade de pontos do perfil.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterQtdPontosPerfil(string perfilId, double pmLimite, string litologiasSelecionadas, TipoDeTrechoEnum tipoTrecho)
        {
            var input = PreencherInput(pmLimite, litologiasSelecionadas, tipoTrecho);
            var output = await _useCase.Execute(perfilId, input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private ObterQtdPontosPerfilInput PreencherInput(double pmLimite, string litologiasSelecionadas, TipoDeTrechoEnum tipoTrecho)
        {
            var input = new ObterQtdPontosPerfilInput
            {
                PmLimite = pmLimite,
                LitologiasSelecionadas = string.IsNullOrWhiteSpace(litologiasSelecionadas) ? null : litologiasSelecionadas.Split(',').ToList(),
                TipoTrecho = tipoTrecho
            };

            return input;
        }
    }
}
