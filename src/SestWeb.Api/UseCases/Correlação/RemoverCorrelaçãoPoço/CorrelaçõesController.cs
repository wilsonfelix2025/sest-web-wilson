using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.RemoverCorrelaçãoPoço;

namespace SestWeb.Api.UseCases.Correlação.RemoverCorrelaçãoPoço
{
    /// <inheritdoc />
    [Route("api/correlacoes/delete-well-corr")]
    [ApiController]
    public class CorrelaçõesController : ControllerBase
    {
        private readonly IRemoverCorrelaçãoPoçoUseCase _removerCorrelaçãoPoçoUseCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public CorrelaçõesController(IRemoverCorrelaçãoPoçoUseCase removerCorrelaçãoPoçoUseCase, Presenter presenter)
        {
            _removerCorrelaçãoPoçoUseCase = removerCorrelaçãoPoçoUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Remove uma correlação exclusiva do caso de uso do poço.
        /// </summary>
        /// <param name="idPoço">Id do caso corrente do poço.</param>
        /// <param name="nome">Nome da correlação a ser removida.</param>
        /// <response code="204">Correlação removida com sucesso.</response>
        /// <response code="400">Não foi possível remover a correlação.</response>
        /// <response code="404">Correlação não encontrada.</response>
        [HttpDelete("{idPoço}/{nome}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteWellCorr(string idPoço, string nome)
        {
            var output = await _removerCorrelaçãoPoçoUseCase.Execute(idPoço, nome);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
