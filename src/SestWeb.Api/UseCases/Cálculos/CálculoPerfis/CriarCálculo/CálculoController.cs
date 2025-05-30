using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPerfis.CriarCálculo;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPerfis.CriarCálculo
{
    [Route("api/criar-calculo-perfis")]
    [ApiController]
    public class CálculoController : ControllerBase
    {
        private readonly ICriarCálculoPerfisUseCase _useCase;
        private readonly Presenter _presenter;

        public CálculoController(ICriarCálculoPerfisUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarCálculoPerfis(CriarCalculoPerfisRequest request)
        {
            var input = PreencherInput(request);

            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private CriarCalculoPerfisInput PreencherInput(CriarCalculoPerfisRequest request)
        {
            var input = new CriarCalculoPerfisInput();
            

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

            return input;
        }

        private List<ParametroInput> PreencherInputListaParametros(List<ParametrosRequest> listaParam)
        {
            var listaParametrosInput = new List<ParametroInput>();

            foreach (var param in listaParam)
            {
                var parametroInput = new ParametroInput
                {
                    Correlação = param.Correlação, Parâmetro = param.Parâmetro, Valor = param.Valor
                };

                listaParametrosInput.Add(parametroInput);
            }

            return listaParametrosInput;
        }
    }
}
