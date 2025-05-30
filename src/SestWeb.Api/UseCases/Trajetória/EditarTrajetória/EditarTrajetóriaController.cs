using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Api.UseCases.Perfil.EditarTrajetória;
using SestWeb.Application.UseCases.TrajetóriaUseCases.EditarTrajetória;

namespace SestWeb.Api.UseCases.Trajetória.EditarTrajetória
{
    [Route("api/pocos")]
    [ApiController]
    public class EditarTrajetóriaController : ControllerBase
    {
        private readonly IEditarTrajetóriaUseCase _editarTrajetóriaUseCase;
        private readonly Presenter _presenter;

        public EditarTrajetóriaController(IEditarTrajetóriaUseCase editarTrajetóriaUseCase, Presenter presenter)
        {
            _editarTrajetóriaUseCase = editarTrajetóriaUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Edita a trajetória do poço com o ID correspondente.
        /// </summary>
        /// <param name="id">Id do poço.</param>
        /// <param name="request">Corpo do request com os pontos da trajetória a ser editada.</param>
        /// <response code="200">Retorna a nova trajetória do poço.</response>
        /// <response code="400">Não foi possível editar a trajetória.</response>
        /// <response code="404">Não foi encontrado poço com o id informado.</response>
        [HttpPost("{id}/editar-trajetoria", Name = "EditarTrajetória")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterTrajetória(string id, EditarTrajetóriaRequest request)
        {
            var input = PreencherInput(request.Pontos);
            var output = await _editarTrajetóriaUseCase.Execute(id, input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private PontoTrajetóriaInput[] PreencherInput(PontoTrajetóriaRequest[] pontos)
        {
            PontoTrajetóriaInput[] pontosProcessados = new PontoTrajetóriaInput[pontos.Length];
            for (var i = 0; i < pontos.Length; i++)
            {
                pontosProcessados[i] = new PontoTrajetóriaInput
                {
                    Pm = pontos[i].Pm,
                    Inclinação = pontos[i].Inclinação,
                    Azimute = pontos[i].Azimute
                };
            }

            return pontosProcessados;
        }
    }
}