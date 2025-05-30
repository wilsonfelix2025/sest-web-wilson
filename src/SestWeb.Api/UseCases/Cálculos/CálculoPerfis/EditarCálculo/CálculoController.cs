using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPerfis.EditarCálculo;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPerfis.EditarCálculo
{
    [Route("api/editar-calculo-perfis")]
    [ApiController]
    public class CálculoController : ControllerBase
    {
        private readonly IEditarCálculoPerfisUseCase _useCase;
        private readonly Presenter _presenter;

        public CálculoController(IEditarCálculoPerfisUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditarCálculoPerfis(EditarCalculoPerfisRequest request)
        {
            var input = PreencherInput(request);

            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private EditarCalculoPerfisInput PreencherInput(EditarCalculoPerfisRequest request)
        {
            var input = new EditarCalculoPerfisInput();


            if (request.Trechos != null && request.Trechos.Any())
            {
                input.Trechos = new List<TrechoInput>();

                foreach (var trecho in request.Trechos)
                {
                    var trechoInput = new TrechoInput
                    {
                        TipoPerfil = trecho.TipoPerfil,
                        Correlação = trecho.Correlação,
                        PmBase = trecho.PmBase,
                        PmTopo = trecho.PmTopo
                    };

                    if (trecho.ListaParametros != null && trecho.ListaParametros.Any())
                        trechoInput.ListaParametros = PreencherInputListaParametros(trecho.ListaParametros);

                    input.Trechos.Add(trechoInput);
                }
            }

            if (request.Parâmetros != null && request.Parâmetros.Any())
                input.Parâmetros = PreencherInputListaParametros(request.Parâmetros);

            input.Nome = request.Nome;
            input.Correlações = request.Correlações;
            input.ListaIdPerfisEntrada = request.ListaIdPerfisEntrada;
            input.IdPoço = request.IdPoço;
            input.IdCálculo = request.IdCálculo;

            return input;
        }

        private List<ParametroInput> PreencherInputListaParametros(List<ParametrosRequest> listaParam)
        {
            var listaParametrosInput = new List<ParametroInput>();

            foreach (var param in listaParam)
            {
                var parametroInput = new ParametroInput
                {
                    Correlação = param.Correlação,
                    Parâmetro = param.Parâmetro,
                    Valor = param.Valor
                };

                listaParametrosInput.Add(parametroInput);
            }

            return listaParametrosInput;
        }
    }
}
