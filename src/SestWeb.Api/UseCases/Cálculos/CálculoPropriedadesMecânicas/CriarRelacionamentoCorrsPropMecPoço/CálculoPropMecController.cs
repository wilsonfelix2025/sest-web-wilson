using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.CriarRelacionamentoCorrsPropMecPoço;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.CriarRelacionamentoCorrsPropMecPoço
{
    /// <inheritdoc />
    [Route("api/propmec/create-well-relationship")]
    [ApiController]
    public class CálculoPropMecController : ControllerBase
    {
        private readonly ICriarRelacionamentoCorrsPropMecPoçoUseCase _criarRelacionamentoCorrsPropMecPoçoUseCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public CálculoPropMecController(ICriarRelacionamentoCorrsPropMecPoçoUseCase criarRelacionamentoCorrsPropMecPoçoUseCase, Presenter presenter)
        {
            _criarRelacionamentoCorrsPropMecPoçoUseCase = criarRelacionamentoCorrsPropMecPoçoUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Cria um novo relacionamento exclusivo para o caso do poço.
        /// </summary>
        /// <param name="createWellRelationshipRequest">Informações necessárias para a criação do relacionamento exclusivo do poço.</param>
        /// <returns>Um novo relacionamento exclusivo do poço criado.</returns>
        /// <response code="201">Retorna o novo relacionamento criado.</response>
        /// <response code="400">O relacionamento não foi criado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateWellRelationship(CreateWellRelationshipRequest createWellRelationshipRequest)
        {
            var output = await _criarRelacionamentoCorrsPropMecPoçoUseCase.Execute(createWellRelationshipRequest.IdPoço, createWellRelationshipRequest.GrupoLitológico, createWellRelationshipRequest.NomeAutor, createWellRelationshipRequest.ChaveAutor,
                createWellRelationshipRequest.CorrUcs, createWellRelationshipRequest.CorrCoesa, createWellRelationshipRequest.CorrAngat, createWellRelationshipRequest.CorrRestr);
            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
