using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.ObterCorrelaçõesPossíveisConformeAsSelecionadas;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.ObterCorrelaçõesPossíveisConformeAsSelecionadas
{
    [Route("api/propmec/get-all-possible-correlations")]
    [ApiController]
    public class CálculoPropMecController : ControllerBase
    {
        private readonly IObterCorrelaçõesPossíveisConformeAsSelecionadasUseCase _obterCorrelaçõesPossíveisConformeAsSelecionadasUseCase;
        private readonly Presenter _presenter;

        public CálculoPropMecController(IObterCorrelaçõesPossíveisConformeAsSelecionadasUseCase obterCorrelaçõesPossíveisConformeAsSelecionadasUseCase, Presenter presenter)
        {
            _obterCorrelaçõesPossíveisConformeAsSelecionadasUseCase = obterCorrelaçõesPossíveisConformeAsSelecionadasUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Obtém as correlações de (ucs, coesa, angat e restr) considerando que o usuário já fez 1, 2 ou 3 seleções (informadas no request).
        /// </summary>
        /// <param name="obterCorrsPossíveisRequest">Informações necessárias para a obtenção das correlações possíveis, dado as correlações já selecionadas.</param>
        /// <returns>Correlações possíveis encontradas.</returns>
        /// <response code="201">Retorna as correlações possíveis.</response>
        /// <response code="400">Não retorna as correlações possíveis.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllPossibleCorrelations(ObterCorrsPossíveisRequest obterCorrsPossíveisRequest)
        {
            var output = await _obterCorrelaçõesPossíveisConformeAsSelecionadasUseCase.Execute(obterCorrsPossíveisRequest.IdPoço, obterCorrsPossíveisRequest.GrupoLitológicoSelecionado, obterCorrsPossíveisRequest.CorrUcsSelecionada, obterCorrsPossíveisRequest.CorrCoesaSelecionada, obterCorrsPossíveisRequest.CorrAngatSelecionada, obterCorrsPossíveisRequest.CorrRestrSelecionada);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}
