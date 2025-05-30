using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Api.UseCases.Relatório;
using SestWeb.Application.UseCases.BaixarRelatórioUseCase;
using SestWeb.Application.UseCases.RelatórioUseCases;

namespace SestWeb.Api.UseCases.BaixarRelatório
{
    [Route("api/report/download")]
    [ApiController]
    public class BaixarRelatórioController : ControllerBase
    {
        private readonly IBaixarRelatórioUseCase _useCase;
        private readonly BaixarRelatórioPresenter _presenter;

        public BaixarRelatórioController(IBaixarRelatórioUseCase useCase, BaixarRelatórioPresenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> BaixarRelatório(BaixarRelatórioRequest request)
        {
            var input = PreencherInput(request);

            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            if (output.Arquivo != null)
            {
                return File(output.Arquivo, "image/jpeg");
            }

            return _presenter.ViewModel;
        }

        private BaixarRelatórioInput PreencherInput(BaixarRelatórioRequest request)
        {
            var estratigrafias = new List<RelatórioEntryInput>();
            var gráficos = new List<RelatórioEntryInput>();

            foreach (var estratigrafia in request.Estratigrafias)
            {
                estratigrafias.Add(ObterEntryRequestDeEntryInput(estratigrafia));
            }

            foreach (var gráfico in request.Graficos)
            {
                gráficos.Add(ObterEntryRequestDeEntryInput(gráfico));
            }

            var input = new BaixarRelatórioInput
            {
                AlturaMR = request.AlturaMR,
                IdPoço = request.IdPoço,
                Lda = request.Lda,
                Nome = request.Nome,
                ProfundidadeFinal = request.ProfundidadeFinal,
                Tipo = request.Tipo,
                Trajetória = ObterEntryRequestDeEntryInput(request.Trajetoria),
                Litologia = ObterEntryRequestDeEntryInput(request.Litologia),
                Estratigrafias = estratigrafias,
                Gráficos = gráficos
            };

            return input;
        }

        private RelatórioEntryInput ObterEntryRequestDeEntryInput(RelatórioEntryRequest entry)
        {
            return new RelatórioEntryInput
            {
                Título = entry.Titulo,
                Data = entry.Data,
                Registros = entry.Registros,
                Curvas = entry.Curvas
            };
        }
    }
}
