using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.PerfilUseCases.EditarPerfil;
using SestWeb.Domain.EstilosVisuais;

namespace SestWeb.Api.UseCases.Perfil.EditarPerfil
{
    [Route("api/perfis")]
    [ApiController]
    public class EditarPerfilController : ControllerBase
    {
        private readonly IEditarPerfilUseCase _editarPerfilUseCase;
        private readonly Presenter _presenter;

        public EditarPerfilController(IEditarPerfilUseCase editarPerfilUseCase, Presenter presenter)
        {
            _editarPerfilUseCase = editarPerfilUseCase;
            _presenter = presenter;
        }

        /// <summary>
        /// Edita o perfil com o ID correspondente..
        /// </summary>
        /// <param name="id">Id do perfil.</param>
        /// <param name="request">Corpo do request com as informações do perfil a ser editado.</param>
        /// <response code="200">Retorna o perfil desejado.</response>
        /// <response code="400">Não foi possível obter o perfil desejado.</response>
        /// <response code="404">Não foi encontrado perfil com o id informado.</response>
        [HttpPut("{id}", Name = "EditarPerfil")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPerfil(string id, EditarPerfilRequest request)
        {
            EstiloLinha estiloLinhaRequest ;
            double espessuraRequest = double.Parse(request.EstiloVisual["espessura"]);

            if (string.IsNullOrWhiteSpace(request.EstiloVisual["estiloLinha"]))
            {
                estiloLinhaRequest = EstiloLinha.Solid;
                espessuraRequest = 0;
            }
            else
            {
                estiloLinhaRequest = (EstiloLinha) Enum.Parse(typeof(EstiloLinha), request.EstiloVisual["estiloLinha"]);
            }


            var estiloVisual = new EstiloVisual(
                request.EstiloVisual["corDaLinha"],
                espessuraRequest,
                estiloLinhaRequest,
                (TipoMarcador) Enum.Parse(typeof(TipoMarcador), request.EstiloVisual["marcador"]),
                request.EstiloVisual["corDoMarcador"]
            );

            var output = await _editarPerfilUseCase.Execute(
                id,
                request.Nome,
                request.Descrição,
                estiloVisual,
                request.PontosDePerfil,
                request.EmPv
            );
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }
    }
}