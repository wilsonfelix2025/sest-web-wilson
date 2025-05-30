using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.AtualizarCorrelação;

namespace SestWeb.Api.UseCases.Correlação.AtualizarCorrelação
{
    [Route("api/correlacoes/{nome}/update")]
    [ApiController]
    public class CorrelaçõesController : ControllerBase
    {

        private readonly IAtualizarCorrelaçãoUseCase _atualizarCorrelaçãoUseCase;
        private readonly Presenter _presenter;

        public CorrelaçõesController(IAtualizarCorrelaçãoUseCase atualizarCorrelaçãoUseCase, Presenter presenter)
        {
            _atualizarCorrelaçãoUseCase = atualizarCorrelaçãoUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Edita uma correlação.
        /// </summary>
        /// <param name="nome">Nome da correlação.</param>
        /// <param name="input">Dados da correlação.</param>
        /// <response code="200">Correlação atualizada com sucesso.</response>
        /// <response code="400">Não foi possível atualizar a correlação.</response>
        /// <response code="404">A correlação não foi encontrada.</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCorr([FromRoute] string nome, [FromBody] AtualizarCorrelaçãoInput input)
        {
            var output = await _atualizarCorrelaçãoUseCase.Execute(nome, input.Descrição, input.Expressão);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
