using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularNormalizaçãoPP;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu.CalcularNormalizaçãoPP
{
    [Route("api/calcular-normalizacao-pp")]
    [ApiController]

    public class CálculoController : ControllerBase
    {
        private readonly ICalcularNormalizaçãoPPUseCase _useCase;
        private readonly Presenter _presenter;

        public CálculoController(ICalcularNormalizaçãoPPUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CalcularNormalizaçãoPP(CalcularNormalizaçãoPPRequest request)
        {
            var input = PreencherInput(request);

            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private CalcularNormalizaçãoPPInput PreencherInput(CalcularNormalizaçãoPPRequest request)
        {
            var input = new CalcularNormalizaçãoPPInput();

            input.Coeficiente = request.Coeficiente;
            input.IdPoço = request.IdPoço;
            input.PerfilTensãoVerticalId = request.PerfilTensãoVerticalId;
            input.PerfilGPOROId = request.PerfilGPOROId;

            if (!request.Coeficiente.HasValue && request.ListaLot != null && request.ListaLot.Any())
            {
                foreach (var req in request.ListaLot)
                {
                    var lot = new LotInput
                    {
                        LDA = req.LDA,
                        LOT = req.LOT,
                        MR = req.MR,
                        PV = req.PV,
                        GradPressãoPoros = req.GradPressãoPoros
                    };

                    input.ListaLot.Add(lot);
                }
                
            }

            return input;
        }
    }
}
