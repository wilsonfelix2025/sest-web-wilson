using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using SestWeb.Application.UseCases.ExportarArquivoUseCase;

namespace SestWeb.Api.UseCases.ExportarArquivo
{
    [Route("api/exportar-arquivo")]
    [ApiController]
    public class ExportarArquivoController : ControllerBase
    {
        private readonly IExportarArquivoUseCase _useCase;
        private readonly ExportarArquivoPresenter _presenter;

        public ExportarArquivoController(IExportarArquivoUseCase useCase, ExportarArquivoPresenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportarArquivo(ExportarArquivoRequest request)
        {
            var input = PreencherInput(request);

            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            if (output.Arquivo != null) { 
                return File(output.Arquivo, "text/plain");
            }

            return _presenter.ViewModel;
        }

        private ExportarArquivoInput PreencherInput(ExportarArquivoRequest request) 
        {
            var input = new ExportarArquivoInput 
            {
                Perfis = request.Perfis,
                Topo = request.PmTopo,
                Base = request.PmBase,
                Intervalo = request.Intervalo,
                Trajetoria = request.Trajetoria,
                Litologia = request.Litologia,
                Pv = request.Pv,
                Cota = request.Cota,
                Arquivo = request.Arquivo,
                IdPoço = request.IdPoço
            };

            return input;
        }
    }
}
