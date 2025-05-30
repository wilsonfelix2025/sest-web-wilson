using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SestWeb.Application.UseCases.ComposiçãoPerfil;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.ComposiçãoPerfil
{
    [Route("api/composição-perfil")]
    [ApiController]
    public class ComporPerfilController : ControllerBase
    {
        private readonly IComporPerfilUseCase _useCase;
        private readonly Presenter _presenter;

        public ComporPerfilController(IComporPerfilUseCase useCase, Presenter presenter)
        {
            _presenter = presenter;
            _useCase = useCase;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ComporPerfil(Request request)
        {
            Log.Information("Recebendo dados para compor perfil: " + request);

            var input = PreencherInput(request);

            var output = await _useCase.Execute(input, request.IdPoço);
            Log.Information("Finalizando compor perfil: " + request + " Output:" + output);
            _presenter.Populate(output);
            return _presenter.ViewModel;
        }

        private ComporPerfilInput PreencherInput(Request request)
        {
            var inputComposição = new ComporPerfilInput
            {
                NomePerfil = request.NomePerfil,
                TipoPerfil = request.TipoPerfil,
            };

            foreach (var req in request.ListaTrechos)
            {
                if (string.IsNullOrWhiteSpace(req.IdPerfil))
                    continue;

                var input = new ComposiçãoPerfilListaInput()
                {
                    PmBase = req.PmBase,
                    PmTopo = req.PmTopo,
                    IdPerfil = req.IdPerfil,
                };

                inputComposição.Lista.Add(input);
            }

            return inputComposição;
        }
    }
}
