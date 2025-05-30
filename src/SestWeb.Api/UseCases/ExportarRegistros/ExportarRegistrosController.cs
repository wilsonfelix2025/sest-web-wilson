using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.ExportarRegistros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.ExportarRegistros
{
    [Route("api/exportar-registros")]
    [ApiController]
    public class ExportarRegistrosController : ControllerBase
    {
        private readonly IExportarRegistrosUseCase _useCase;
        private readonly ExportarRegistrosPresenter _presenter;

        public ExportarRegistrosController(IExportarRegistrosUseCase useCase, ExportarRegistrosPresenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportarArquivo(ExportarRegistrosRequest request)
        {
            var input = PreencherInput(request);

            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            if (output.Arquivo != null)
            {
                return File(output.Arquivo, "text/plain", output.NomeArquivo);
            }

            return _presenter.ViewModel;
        }

        private ExportarRegistrosInput PreencherInput(ExportarRegistrosRequest request)
        {
            var input = new ExportarRegistrosInput
            {
                Registros = request.Registros,
                IdPoço = request.IdPoço
            };

            return input;
        }
    }
}
