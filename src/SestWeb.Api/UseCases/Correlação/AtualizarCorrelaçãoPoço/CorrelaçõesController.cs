using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.AtualizarCorrelaçãoPoço;

namespace SestWeb.Api.UseCases.Correlação.AtualizarCorrelaçãoPoço
{
    [Route("api/correlacoes/{idPoço}/{nome}/update-well-corr")]
    [ApiController]
    public class CorrelaçõesController : ControllerBase
    {

        private readonly IAtualizarCorrelaçãoPoçoUseCase _atualizarCorrelaçãoPoçoUseCase;
        private readonly Presenter _presenter;

        public CorrelaçõesController(IAtualizarCorrelaçãoPoçoUseCase atualizarCorrelaçãoPoçoUseCase, Presenter presenter)
        {
            _atualizarCorrelaçãoPoçoUseCase = atualizarCorrelaçãoPoçoUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Edita uma correlação exclusiva do caso do poço.
        /// </summary>
        /// <param name="idPoço">Id do caso corrente do poço.</param>
        /// <param name="nome">Nome da correlação.</param>
        /// <param name="input">Dados da correlação.</param>
        /// <response code="200">Correlação atualizada com sucesso.</response>
        /// <response code="400">Não foi possível atualizar a correlação.</response>
        /// <response code="404">A correlação não foi encontrada.</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateWellCorr([FromRoute] string idPoço, [FromRoute] string nome, [FromBody] AtualizarCorrelaçãoPoçoInput input)
        {
            var output = await _atualizarCorrelaçãoPoçoUseCase.Execute(idPoço, nome, input.Descrição, input.Expressão);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
