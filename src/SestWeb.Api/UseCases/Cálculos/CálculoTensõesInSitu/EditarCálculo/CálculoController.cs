
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.EditarCálculo;
using SestWeb.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu.EditarCálculo
{
    [Route("api/editar-calculo-tensoesinsitu")]
    [ApiController]
    public class CálculoController : ControllerBase
    {
        private readonly IEditarCálculoTensõesInSituUseCase _useCase;
        private readonly Presenter _presenter;

        public CálculoController(IEditarCálculoTensõesInSituUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditarCálculo(EditarCálculoTensõesInSituRequest request)
        {
            var input = PreencherInput(request);

            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private EditarCálculoTensõesInSituInput PreencherInput(EditarCálculoTensõesInSituRequest request)
        {
            var input = new EditarCálculoTensõesInSituInput();
            input.Coeficiente = request.Coeficiente;
            input.IdPoço = request.IdPoço;
            input.IdCálculo = request.IdCálculo;
            input.NomeCálculo = request.NomeCálculo;
            input.PerfilGPOROId = request.PerfilGPOROId;
            input.PerfilPoissonId = request.PerfilPoissonId;
            input.PerfilTensãoVerticalId = request.PerfilTensãoVerticalId;
            input.PerfilTHORminId = request.PerfilTHORminId;
            switch (request.TensãoHorizontalMenorMetodologiaCálculo)
            {
                case "K0":
                    input.TensãoHorizontalMenorMetodoligiaCálculo = MetodologiaCálculoTensãoHorizontalMenorEnum.K0;
                    break;
                case "K0Acompanhamento":
                    input.TensãoHorizontalMenorMetodoligiaCálculo = MetodologiaCálculoTensãoHorizontalMenorEnum.K0Acompanhamento;
                    break;
                case "ModeloElástico":
                    input.TensãoHorizontalMenorMetodoligiaCálculo = MetodologiaCálculoTensãoHorizontalMenorEnum.ModeloElástico;
                    break;
                case "NormalizaçãoLDA":
                    input.TensãoHorizontalMenorMetodoligiaCálculo = MetodologiaCálculoTensãoHorizontalMenorEnum.NormalizaçãoLDA;
                    break;
                case "NormalizaçãoPP":
                    input.TensãoHorizontalMenorMetodoligiaCálculo = MetodologiaCálculoTensãoHorizontalMenorEnum.NormalizaçãoPP;
                    break;
                default:
                    input.TensãoHorizontalMenorMetodoligiaCálculo = MetodologiaCálculoTensãoHorizontalMenorEnum.NãoEspecificado;
                    break;
            }

            switch (request.TensãoHorizontalMaiorMetodologiaCálculo)
            {
                case "BreakoutTrechosVerticais":
                    input.TensãoHorizontalMaiorMetodologiaCálculo = MetodologiaCálculoTensãoHorizontalMaiorEnum.BreakoutTrechosVerticais;
                    break;
                case "FraturasTrechosVerticais":
                    input.TensãoHorizontalMaiorMetodologiaCálculo = MetodologiaCálculoTensãoHorizontalMaiorEnum.FraturasTrechosVerticais;
                    break;
                case "RelaçãoEntreTensões":
                    input.TensãoHorizontalMaiorMetodologiaCálculo = MetodologiaCálculoTensãoHorizontalMaiorEnum.RelaçãoEntreTensões;
                    break;
                default:
                    input.TensãoHorizontalMaiorMetodologiaCálculo = MetodologiaCálculoTensãoHorizontalMaiorEnum.NãoEspecificado;
                    break;
            }

            

            if (request.ListaLot != null && request.ListaLot.Any())
            {
                input.ListaLot = new List<LotInput>();

                foreach (var req in request.ListaLot)
                {
                    var lot = new LotInput
                    {
                        GradPressãoPoros = req.GradPressãoPoros,
                        LDA = req.LDA,
                        LOT = req.LOT,
                        MR = req.MR,
                        PV = req.PV
                    };

                    input.ListaLot.Add(lot);
                }
            }

            if (request.Depleção != null)
                input.Depleção = PreencherDepleção(request);


            if (request.Breakout != null && request.Breakout.TrechosBreakout.Any())
                input.Breakout = PreencherBreakout(request);

            if (request.FraturasTrechosVerticais != null && request.FraturasTrechosVerticais.TrechosFratura.Any())
                input.FraturasTrechosVerticais = PreencherFraturaTrechosVerticais(request);

            if (request.RelaçãoTensão != null)
                input.RelaçãoTensão = PreencheRelaçãoTensão(request);

            return input;
        }

        private RelaçãoTensãoInput PreencheRelaçãoTensão(EditarCálculoTensõesInSituRequest request)
        {
            var input = new RelaçãoTensãoInput
            {
                AZTHMenor = request.RelaçãoTensão.AZTHMenor,
                PerfilGPOROId = request.RelaçãoTensão.PerfilGPOROId,
                PerfilRelaçãoTensãoId = request.RelaçãoTensão.PerfilRelaçãoTensãoId,
                PerfilTVERTId = request.RelaçãoTensão.PerfilTVERTId,
                TipoRelação = request.RelaçãoTensão.TipoRelação
            };

            return input;
        }

        private FraturasTrechosVerticaisInput PreencherFraturaTrechosVerticais(EditarCálculoTensõesInSituRequest request)
        {
            var input = new FraturasTrechosVerticaisInput
            {
                PerfilGPOROId = request.FraturasTrechosVerticais.PerfilGPOROId,
                PerfilRESTRId = request.FraturasTrechosVerticais.PerfilRESTRId,
                Azimuth = request.FraturasTrechosVerticais.Azimuth,
                TrechosFratura = new List<FraturaTrechosVerticaisValoresInput>()
            };

            foreach (var req in request.FraturasTrechosVerticais.TrechosFratura)
            {
                var valor = new FraturaTrechosVerticaisValoresInput
                {
                    PesoFluido = req.PesoFluido,
                    PM = req.PM
                };
                input.TrechosFratura.Add(valor);
            }

            return input;
        }

        private DepleçãoInput PreencherDepleção(EditarCálculoTensõesInSituRequest request)
        {
            var input = new DepleçãoInput
            {
                BiotId = request.Depleção.BiotId,
                GPORODepletadaId = request.Depleção.GPORODepletadaId,
                GPOROOriginalId = request.Depleção.GPOROOriginalId,
                PoissonId = request.Depleção.PoissonId
            };

            return input;
        }

        private BreakoutInput PreencherBreakout(EditarCálculoTensõesInSituRequest request)
        {
            var input = new BreakoutInput
            {
                PerfilANGATId = request.Breakout.PerfilANGATId,
                PerfilGPOROId = request.Breakout.PerfilGPOROId,
                PerfilUCSId = request.Breakout.PerfilUCSId,
                TrechosBreakout = new List<BreakoutValoresInput>(),
                Azimuth = request.Breakout.Azimuth
            };

            foreach (var req in request.Breakout.TrechosBreakout)
            {
                var valor = new BreakoutValoresInput
                {
                    Azimute = req.Azimute,
                    Largura = req.Largura,
                    PesoFluido = req.PesoFluido,
                    PM = req.PM
                };

                input.TrechosBreakout.Add(valor);
            }
            return input;
        }
    }
}
