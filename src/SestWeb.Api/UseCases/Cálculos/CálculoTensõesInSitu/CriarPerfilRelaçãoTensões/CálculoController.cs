
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CriarPerfilRelaçãoTensões;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu.CriarPerfilRelaçãoTensões
{
    [Route("api/criar-perfil-relacao-tensoes")]
    [ApiController]
    public class CálculoController : ControllerBase
    {
        private readonly ICriarPerfilRelaçãoTensõesUseCase _useCase;
        private readonly Presenter _presenter;

        public CálculoController(ICriarPerfilRelaçãoTensõesUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarPerfilRelaçãoTensões(Request request)
        {
            var input = PreencherInput(request);

            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private CriarPerfilRelaçãoInput PreencherInput(Request request)
        {
            var input = new CriarPerfilRelaçãoInput();


            if (request.Valores != null && request.Valores.Any())
            {
                input.Valores = new List<ValoresInput>();

                foreach (var valor in request.Valores)
                {
                    var valorInput = new ValoresInput
                    {
                        PMTopo = valor.PMTopo,
                        PMBase = valor.PMBase,
                        Valor = valor.Valor
                    };

                    input.Valores.Add(valorInput);
                }
            }

            input.IdPoço = request.IdPoço;
            input.NomePerfil = request.NomePerfil;

            return input;
        }


    }
}
