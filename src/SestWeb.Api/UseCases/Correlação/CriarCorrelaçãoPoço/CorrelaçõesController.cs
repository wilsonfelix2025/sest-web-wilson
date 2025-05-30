using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.CriarCorrelaçãoPoço;

namespace SestWeb.Api.UseCases.Correlação.CriarCorrelaçãoPoço
{
    /// <inheritdoc />
    [Route("api/correlacoes/create-well-corr")]
    [ApiController]
    public class CorrelaçõesController : ControllerBase
    {
        private readonly ICriarCorrelaçãoPoçoUseCase _criarCorrelaçãoPoçoUseCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public CorrelaçõesController(ICriarCorrelaçãoPoçoUseCase criarCorrelaçãoPoçoUseCase, Presenter presenter)
        {
            _criarCorrelaçãoPoçoUseCase = criarCorrelaçãoPoçoUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Cria uma nova correlação exclusiva para o caso do poço.
        /// </summary>
        /// <param name="createWellCorrRequest">Informações necessárias para a criação da correlação exclusiva do poço.</param>
        /// <returns>Uma nova correlação exclusiva do poço criada.</returns>
        /// <response code="201">Retorna a nova correlação criada.</response>
        /// <response code="400">A correlação não foi criada.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateWellCorr(CreateWellCorrRequest createWellCorrRequest)
        {
            var output = await _criarCorrelaçãoPoçoUseCase.Execute(createWellCorrRequest.idPoço, createWellCorrRequest.Nome, createWellCorrRequest.NomeAutor, createWellCorrRequest.ChaveAutor, createWellCorrRequest.Descrição, createWellCorrRequest.Expressão);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
