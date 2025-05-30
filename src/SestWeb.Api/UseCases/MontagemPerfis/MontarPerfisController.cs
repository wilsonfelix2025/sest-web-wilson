using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SestWeb.Application.UseCases.MontagemPerfis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.MontagemPerfis
{
    [Route("api/montar-perfis")]
    [ApiController]
    public class MontarPerfisController : ControllerBase
    {
        private readonly IMontarPerfisUseCase _useCase;
        private readonly Presenter _presenter;

        public MontarPerfisController(IMontarPerfisUseCase useCase, Presenter presenter)
        {
            _presenter = presenter;
            _useCase = useCase;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InserirTrecho(Request request)
        {
            Log.Information("Recebendo dados para montar perfis: " + request);

            var input = PreencherInput(request);

            var output = await _useCase.Execute(input, request.IdPoço);
            Log.Information("Finalizando montar perfis: " + request + " Output:" + output);
            _presenter.Populate(output);
            return _presenter.ViewModel;
        }

        private MontarPerfisInput PreencherInput(Request request)
        {
            var inputMontagem = new MontarPerfisInput
            {
                NomeDTC = request.NomeDTC,
                NomeDTS = request.NomeDTS,
                NomeGRay = request.NomeGRay,
                NomeNPhi = request.NomeNPhi,
                NomeResist = request.NomeResist,
                NomeRhob = request.NomeRhob,
                RemoverTendência = request.RemoverTendência
            };

            foreach (var req in request.ListaTrechos)
            {
                if (string.IsNullOrWhiteSpace(req.IdNPhiApoio) && string.IsNullOrWhiteSpace(req.IdDTCApoio) && string.IsNullOrWhiteSpace(req.IdDTSApoio)
                && string.IsNullOrWhiteSpace(req.IdGRayApoio) && string.IsNullOrWhiteSpace(req.IdResistApoio) && string.IsNullOrWhiteSpace(req.IdRhobApoio))
                    continue;

                var input = new MontagemPerfisListaInput()
                {
                    PvBase = req.PvBase,
                    PvTopo = req.PvTopo,
                    PvBaseApoio = req.PvBaseApoio,
                    PvTopoApoio = req.PvTopoApoio,
                    IdPoçoApoio = req.IdPoçoApoio,
                    IdResistApoio = req.IdResistApoio,
                    IdDTCApoio = req.IdDTCApoio,
                    IdDTSApoio = req.IdDTSApoio,
                    IdGRayApoio = req.IdGRayApoio,
                    IdNPhiApoio = req.IdNPhiApoio,
                    IdRhobApoio = req.IdRhobApoio,
                };

                inputMontagem.Lista.Add(input);
            }

            return inputMontagem;
        }
    }
}
