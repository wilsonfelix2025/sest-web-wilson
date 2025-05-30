using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularNormalizaçãoLDA;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu.CalcularNormalizaçãoLDA
{
    [Route("api/calcular-normalizacao-lda")]
    [ApiController]
    public class CálculoController : ControllerBase
    {
        private readonly ICalcularNormalizaçãoLDAUseCase _useCase;
        private readonly Presenter _presenter;

        public CálculoController(ICalcularNormalizaçãoLDAUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CalcularNormalizaçãoLDA(CalcularNormalizaçãoLDARequest request)
        {
            var input = PreencherInput(request);

            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private CalcularNormalizaçãoLDAInput PreencherInput(CalcularNormalizaçãoLDARequest request)
        {
            var input = new CalcularNormalizaçãoLDAInput();

            input.Coeficiente = request.Coeficiente;
            input.IdPoço = request.IdPoço;
            input.PerfilTensãoVerticalId = request.PerfilTensãoVerticalId;

            if (!request.Coeficiente.HasValue && request.ListaLot != null && request.ListaLot.Any())
            {
                foreach (var req in request.ListaLot)
                {
                    var lot = new LotInput
                    {
                        LDA = req.LDA,
                        LOT = req.LOT,
                        MR = req.MR,
                        PV = req.PV
                    };

                    input.ListaLot.Add(lot);
                }
                
            }

            return input;
        }
    }
}
