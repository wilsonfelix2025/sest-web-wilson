using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularK0Acompanhamento;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu.CalcularK0Acompanhamento
{
    [Route("api/calcular-k0-acompanhamento")]
    [ApiController]
    public class CálculoController : ControllerBase
    {
        private readonly ICalcularK0AcompanhamentoUseCase _useCase;
        private readonly Presenter _presenter;

        public CálculoController(ICalcularK0AcompanhamentoUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CalcularK0Acompanhamento(CalcularK0AcompanhamentoRequest request)
        {
            var input = PreencherInput(request);

            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private CalcularK0AcompanhamentoInput PreencherInput(CalcularK0AcompanhamentoRequest request)
        {
            var input = new CalcularK0AcompanhamentoInput();

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
