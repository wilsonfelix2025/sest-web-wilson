using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.EditarCálculo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPropriedadesMecânicas.EditarCálculo
{
    [Route("api/editar-calculo-propriedades-mecanicas")]
    [ApiController]
    public class CálculoController : ControllerBase
    {
        private readonly IEditarCálculoPropMecUseCase _useCase;
        private readonly Presenter _presenter;

        public CálculoController(IEditarCálculoPropMecUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditarCálculoPropMec(EditarCalculoPropMecRequest request)
        {
            var input = PreencherInput(request);

            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private EditarCalculoPropMecInput PreencherInput(EditarCalculoPropMecRequest request)
        {
            var input = new EditarCalculoPropMecInput();


            if (request.Trechos != null && request.Trechos.Any())
            {
                input.Trechos = new List<TrechoPropMecInput>();

                foreach (var trecho in request.Trechos)
                {
                    var trechoInput = new TrechoPropMecInput
                    {
                        PmBase = trecho.PmBase,
                        PmTopo = trecho.PmTopo,
                        ANGATCorrelação = trecho.ANGAT,
                        CoesãoCorrelação = trecho.COESA,
                        UCSCorrelação = trecho.UCS,
                        RESTRCorrelação = trecho.RESTR,
                        BIOTCorrelação = trecho.BIOT,
                        GrupoLitológico = trecho.GrupoLitológico
                    };

                    input.Trechos.Add(trechoInput);
                }
            }

            if (request.Regiões != null && request.Regiões.Any())
            {
                input.Regiões = new List<RegiõesInput>();

                foreach (var região in request.Regiões)
                {
                    var regiãoInput = new RegiõesInput
                    {
                        ANGATCorrelação = região.ANGAT,
                        CoesãoCorrelação = região.COESA,
                        UCSCorrelação = região.UCS,
                        RESTRCorrelação = região.RESTR,
                        GrupoLitológico = região.GrupoLitológico                        
                    };

                    input.Regiões.Add(regiãoInput);
                }
            }
                

            input.Nome = request.Nome;
            input.Correlações = request.Correlações;
            input.ListaIdPerfisEntrada = request.ListaIdPerfisEntrada;
            input.IdPoço = request.IdPoço;
            input.IdCálculo = request.IdCálculo;

            return input;
        }

        
        
    }
}
