using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using SestWeb.Api.UseCases.Relatório;
using SestWeb.Application.UseCases.GerarRelatórioUseCase;
using SestWeb.Application.UseCases.RelatórioUseCases;

namespace SestWeb.Api.UseCases.GerarRelatório
{
    [Route("api/report/preview")]
    [ApiController]
    public class GerarRelatórioController : ControllerBase
    {
        private readonly IGerarRelatórioUseCase _useCase;
        private readonly GerarRelatórioPresenter _presenter;

        public GerarRelatórioController(IGerarRelatórioUseCase useCase, GerarRelatórioPresenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GerarRelatório(GerarRelatórioRequest request)
        {
            var input = PreencherInput(request);

            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private GerarRelatórioInput PreencherInput(GerarRelatórioRequest request) 
        {
            var estratigrafias = new List<RelatórioEntryInput>();
            var gráficos = new List<RelatórioEntryInput>();

            foreach (var estratigrafia in request.Estratigrafias)
            {
                estratigrafias.Add(ObterEntryRequestDeEntryInput(estratigrafia));
            }

            foreach(var gráfico in request.Graficos)
            {
                gráficos.Add(ObterEntryRequestDeEntryInput(gráfico));
            }

            var input = new GerarRelatórioInput 
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
