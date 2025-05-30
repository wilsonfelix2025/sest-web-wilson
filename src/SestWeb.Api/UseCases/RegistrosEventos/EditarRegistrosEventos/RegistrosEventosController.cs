using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.RegistrosEventosUseCases.EditarRegistrosEventos;

namespace SestWeb.Api.UseCases.RegistrosEventos.EditarRegistrosEventos
{
    [Route("api/registrosEventos")]
    [ApiController]
    public class RegistrosEventosController : ControllerBase
    {
        private readonly IEditarRegistrosEventosUseCase _editarRegistrosEventosUseCase;
        private readonly Presenter _presenter;

        public RegistrosEventosController(IEditarRegistrosEventosUseCase editarRegistrosEventosUseCase, Presenter presenter)
        {
            _editarRegistrosEventosUseCase = editarRegistrosEventosUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Edita os registros e eventos de um poço.
        /// </summary>
        /// <param name="request">Corpo do request com as informações dos registros e eventos a ser editado.</param>
        /// <response code="200">Retorna os registros e eventos atualizados.</response>
        /// <response code="400">Não foi possível editar os registros e eventos.</response>
        /// <response code="404">Não foi encontrado poço com o id informado.</response>
        [HttpPut(Name = "EditarRegistrosEventos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditarRegistrosEventos(EditarRegistrosEventosRequest request)
        {
            var output = await _editarRegistrosEventosUseCase.Execute(
                request.IdPoço,
                request.Tipo,
                request.Trechos,
                request.Marcadores
            );
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}