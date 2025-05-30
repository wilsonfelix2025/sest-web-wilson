using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.CriarCorrelação;

namespace SestWeb.Api.UseCases.Correlação.CriarCorrelação
{
    /// <inheritdoc />
    [Route("api/correlacoes/create")]
    [ApiController]
    public class CorrelaçõesController : ControllerBase
    {
        private readonly ICriarCorrelaçãoUseCase _criarCorrelaçãoUseCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public CorrelaçõesController(ICriarCorrelaçãoUseCase criarCorrelaçãoUseCase, Presenter presenter)
        {
            _criarCorrelaçãoUseCase = criarCorrelaçãoUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Cria uma nova correlação.
        /// </summary>
        /// <param name="createCorrRequest">Informações necessárias para a criação da correlação.</param>
        /// <returns>Uma nova correlação criada.</returns>
        /// <response code="201">Retorna a nova correlação criada.</response>
        /// <response code="400">A correlação não foi criada.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCorr(CreateCorrRequest createCorrRequest)
        {
            var output = await _criarCorrelaçãoUseCase.Execute(createCorrRequest.IdPoço, createCorrRequest.Nome, createCorrRequest.NomeAutor, createCorrRequest.ChaveAutor, createCorrRequest.Descrição, createCorrRequest.Expressão);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
