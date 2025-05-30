using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoGradientes;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoGradientes.EditarCálculo;
using SestWeb.Domain.Enums;
using System;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Cálculos.CálculoGradientes.EditarCálculo
{
    [Route("api/editar-calculo-gradientes")]
    [ApiController]
    public class CálculoController : ControllerBase
    {
        private readonly IEditarCálculoGradientesUseCase _useCase;
        private readonly Presenter _presenter;

        public CálculoController(IEditarCálculoGradientesUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarCálculo(EditarCálculoGradientesRequest request)
        {
            var input = PreencherInput(request);

            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private EditarCálculoGradientesInput PreencherInput(EditarCálculoGradientesRequest request)
        {
            var input = new EditarCálculoGradientesInput
            {
                IdPoço = request.IdPoço,
                IdCálculo = request.IdCálculo,
                NomeCálculo = request.NomeCálculo,
                AreaPlastificada = request.AreaPlastificada,
                CalcularFraturasColapsosSeparadamente = request.CalcularFraturasColapsosSeparadamente,
                FluidoPenetrante = request.FluidoPenetrante,
                HabilitarFiltroAutomatico = request.HabilitarFiltroAutomatico,
                IncluirEfeitosFísicosQuímicos = request.IncluirEfeitosFísicosQuímicos,
                IncluirEfeitosTérmicos = request.IncluirEfeitosTérmicos,
                MalhaNunDivAng = request.MalhaNunDivAng,
                MalhaNunDivRad = request.MalhaNunDivRad,
                MalhaRextRint = request.MalhaRextRint,
                MalhaRMaxRMin = request.MalhaRMaxRMin,
                PerfilANGATId = request.PerfilANGATId,
                PerfilAZTHminId = request.PerfilAZTHminId,
                PerfilBIOTId = request.PerfilBIOTId,
                PerfilCOESAId = request.PerfilCOESAId,
                PerfilDIAMBROCAId = request.PerfilDIAM_BROCAId,
                PerfilGPOROId = request.PerfilGPOROId,
                PerfilGSOBRId = request.PerfilGSOBRId,
                PerfilKSId = request.PerfilKSId,
                PerfilPERMId = request.PerfilPERMId,
                PerfilPOISSId = request.PerfilPOISSId,
                PerfilPOROId = request.PerfilPOROId,
                PerfilRETRId = request.PerfilRESTRId,
                PerfilTHORmaxId = request.PerfilTHORmaxId,
                PerfilTHORminId = request.PerfilTHORminId,
                PerfilUCSId = request.PerfilUCSId,
                PerfilYOUNGId = request.PerfilYOUNGId,
                Tempo = request.Tempo,
                TipoCritérioRuptura = PreencherCritério(request.TipoCritérioRuptura),
                TipoModelo = request.TipoModelo == "Elástico" ? ModeloAnáliseGradientesEnum.Elástico : ModeloAnáliseGradientesEnum.Poroelástico
            };

            var param = new ParâmetrosPoroElásticoInput
            {
                CoeficienteDifusãoSoluto = request.ParâmetrosPoroElástico.CoeficienteDifusãoSoluto,
                CoeficienteDissociaçãoSoluto = request.ParâmetrosPoroElástico.CoeficienteDissociaçãoSoluto,
                CoeficienteInchamento = request.ParâmetrosPoroElástico.CoeficienteInchamento,
                CoeficienteReflexão = request.ParâmetrosPoroElástico.CoeficienteReflexão,
                ConcentraçãoSolFluidoPerfuração = request.ParâmetrosPoroElástico.ConcentraçãoSolFluidoPerfuração,
                ConcentraçãoSolutoRocha = request.ParâmetrosPoroElástico.ConcentraçãoSolutoRocha,
                DensidadeFluidoFormação = request.ParâmetrosPoroElástico.DensidadeFluidoFormação,
                DifusidadeTérmica = request.ParâmetrosPoroElástico.DifusidadeTérmica,
                ExpansãoTérmicaRocha = request.ParâmetrosPoroElástico.ExpansãoTérmicaRocha,
                ExpansãoTérmicaVolumeFluidoPoros = request.ParâmetrosPoroElástico.ExpansãoTérmicaVolumeFluidoPoros,
                Kf = request.ParâmetrosPoroElástico.Kf,
                Litologias = request.ParâmetrosPoroElástico.Litologias,
                MassaMolarSoluto = request.ParâmetrosPoroElástico.MassaMolarSoluto,
                PropriedadeTérmicaTemperaturaFormação = request.ParâmetrosPoroElástico.PropriedadeTérmicaTemperaturaFormação,
                TemperaturaFormação = request.ParâmetrosPoroElástico.TemperaturaFormação,
                TemperaturaPoço = request.ParâmetrosPoroElástico.TemperaturaPoço,
                TipoSal = request.ParâmetrosPoroElástico.TipoSal == null ? string.Empty : request.ParâmetrosPoroElástico.TipoSal,
                Viscosidade = request.ParâmetrosPoroElástico.Viscosidade,
                TemperaturaFormaçãoFisicoQuimica = request.TemperaturaFormaçãoFisicoQuimica
            };

            input.ParâmetrosPoroElástico = param;
            return input;
        }

        private CritérioRupturaGradientesEnum PreencherCritério(string tipoCritérioRuptura)
        {

            switch (tipoCritérioRuptura)
            {
                case "Mohr-Coulomb":
                    return CritérioRupturaGradientesEnum.MohrCoulomb;
                case "Lade-Ewy":
                    return CritérioRupturaGradientesEnum.Lade;
                case "Drucker - Pragger Interno":
                    return CritérioRupturaGradientesEnum.DruckerPraggerInternal;
                case "Drucker - Pragger Central":
                    return CritérioRupturaGradientesEnum.DruckerPraggerMiddle;
                case "Drucker - Pragger Externo":
                    return CritérioRupturaGradientesEnum.DruckerPraggerExternal;
                default:
                    return CritérioRupturaGradientesEnum.MohrCoulomb;
            }
        }
    }
}
