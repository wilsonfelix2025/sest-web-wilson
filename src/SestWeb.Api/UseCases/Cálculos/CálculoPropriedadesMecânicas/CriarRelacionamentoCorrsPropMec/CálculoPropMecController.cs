using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.CriarRelacionamentoCorrsPropMec;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.CriarRelacionamentoCorrsPropMec
{
    /// <inheritdoc />
    [Route("api/propmec/create-user-relationship")]
    [ApiController]
    public class CálculoPropMecController : ControllerBase
    {
        private readonly ICriarRelacionamentoCorrsPropMecUseCase _criarRelacionamentoCorrsPropMecUseCase;
        private readonly Presenter _presenter;

        /// <inheritdoc />
        public CálculoPropMecController(ICriarRelacionamentoCorrsPropMecUseCase criarRelacionamentoCorrsPropMecUseCase, Presenter presenter)
        {
            _criarRelacionamentoCorrsPropMecUseCase = criarRelacionamentoCorrsPropMecUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Cria um novo relacionamento do usuário, disponibilizado para todos.
        /// </summary>
        /// <param name="createUserRelationshipRequest">Informações necessárias para a criação do relacionamento do usuário.</param>
        /// <returns>Um novo relacionamento exclusivo do poço criado.</returns>
        /// <response code="201">Retorna o novo relacionamento criado.</response>
        /// <response code="400">O relacionamento não foi criado.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateWellRelationship(CreateUserRelationshipRequest createUserRelationshipRequest)
        {
            var output = await _criarRelacionamentoCorrsPropMecUseCase.Execute(createUserRelationshipRequest.IdPoço, createUserRelationshipRequest.GrupoLitológico, createUserRelationshipRequest.NomeAutor, createUserRelationshipRequest.ChaveAutor,
                createUserRelationshipRequest.CorrUcs, createUserRelationshipRequest.CorrCoesa, createUserRelationshipRequest.CorrAngat, createUserRelationshipRequest.CorrRestr);
            _presenter.Populate(output);
            return _presenter.ViewModel;
        }
    }
}
