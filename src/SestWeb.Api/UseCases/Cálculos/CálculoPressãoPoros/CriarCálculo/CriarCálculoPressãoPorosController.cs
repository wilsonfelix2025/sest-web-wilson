using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.CriarCálculo;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.ReservatórioInput;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Cálculos.CálculoPressãoPoros.CriarCálculo
{
    [Route("api/criar-calculo-pressao-poros")]
    [ApiController]
    public class CriarCálculoPressãoPorosController : ControllerBase
    {
        private readonly ICriarCálculoPressãoPorosUseCase _useCase;
        private readonly Presenter _presenter;

        public CriarCálculoPressãoPorosController(ICriarCálculoPressãoPorosUseCase useCase, Presenter presenter)
        {
            _useCase = useCase;
            _presenter = presenter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarCálculoPressãoPoros(CriarCálculoPressãoPorosRequest request)
        {
            var input = PreencherInput(request);
            var output = await _useCase.Execute(input);
            _presenter.Populate(output);

            return _presenter.ViewModel;
        }

        private CriarCálculoPressãoPorosInput PreencherInput(CriarCálculoPressãoPorosRequest request)
        {
            DadosReservatórioInput dadosReservatorio = null;

            if (request.Reservatorio != null)
            {
                dadosReservatorio = new DadosReservatórioInput
                {
                    Nome = request.Reservatorio.Nome,
                    Interesse = new InteresseReservatórioInput
                    {
                        Base = request.Reservatorio.Interesse.Base,
                        Topo = request.Reservatorio.Interesse.Topo
                    },
                    Referencia = new ReferênciaReservatórioInput
                    {
                        Poco = request.Reservatorio.Referencia.Poco,
                        Pp = request.Reservatorio.Referencia.Pp,
                        Cota = request.Reservatorio.Referencia.Cota,
                        Contatos = new ContatosReservatórioInput
                        {
                            GasOleo = request.Reservatorio.Referencia.Contatos.GasOleo,
                            OleoAgua = request.Reservatorio.Referencia.Contatos.OleoAgua
                        },
                        Gradiente = new GradienteReservatórioInput
                        {
                            Agua = request.Reservatorio.Referencia.Gradiente.Agua,
                            Gas = request.Reservatorio.Referencia.Gradiente.Gas,
                            Oleo = request.Reservatorio.Referencia.Gradiente.Oleo
                        }
                    }
                };
            }

            return new CriarCálculoPressãoPorosInput
            {
                Nome = request.Nome,
                IdPoço = request.IdPoço,
                Tipo = request.Tipo,
                Gn = request.Gn,
                Exp = request.Exp,
                IdPerfilFiltrado = request.IdPerfilFiltrado,
                IdGradienteSobrecarga = request.IdGradienteSobrecarga,
                CalculoReservatorio = request.CalculoReservatorio,
                Gpph = request.Gpph,
                IdPph = request.IdPph,
                Reservatório = dadosReservatorio
            };
        }
    }
}
