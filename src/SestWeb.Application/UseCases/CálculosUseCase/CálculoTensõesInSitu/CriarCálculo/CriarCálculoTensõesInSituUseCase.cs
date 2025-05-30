
using SestWeb.Application.Repositories;
using SestWeb.Application.Validadores;
using SestWeb.Domain.DTOs.Cálculo.TensõesInSitu;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Tensões.Factory;
using SestWeb.Domain.Entities.Correlações.Base.Factory;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CriarCálculo
{
    public class CriarCálculoTensõesInSituUseCase : ICriarCálculoTensõesInSituUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly ICálculoTensõesFactory _cálculoFactory;
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoFactory _correlaçãoFactory;

        public CriarCálculoTensõesInSituUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository, ICálculoTensõesFactory cálculoFactory
            , ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, ICorrelaçãoFactory correlaçãoFactory)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _cálculoFactory = cálculoFactory;
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoFactory = correlaçãoFactory;
        }

        public async Task<CriarCálculoTensõesInSituOutput> Execute(CriarCálculoTensõesInSituInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);
                if (poço == null)
                    return CriarCálculoTensõesInSituOutput.CálculoNãoCriado("Poço não encontrado");

                ////preparação para as entradas para a factory
                var listaPerfisEntrada = await PreencherPerfisEntrada(input);
                var parâmetrosLOT = PreencherParâmetrosLOT(input.ListaLot);
                var depleção = await PreencherDepleção(input.Depleção);
                var breakout = await PreencherBreakout(input.Breakout);
                var listaPerfisSaída = PreencherListaPerfisSaída(input.NomeCálculo, poço, input.TensãoHorizontalMenorMetodoligiaCálculo);
                var relaçãoTensão = await PreencherRelaçãoTensão(input.RelaçãoTensão);
                var fraturasTrechosVerticais = await PreencherFraturasTrechosVerticais(input.FraturasTrechosVerticais);

                if (depleção != null)
                {
                    listaPerfisEntrada.AddRange(new List<PerfilBase>() { depleção.Biot });
                }

                ////retorna se a entidade calculo criada está valida ou não
                var result = _cálculoFactory.CreateCálculoTensões(input.NomeCálculo, input.TensãoHorizontalMenorMetodoligiaCálculo, "Tensões"
                    , listaPerfisEntrada.ToList(), listaPerfisSaída, poço.Trajetória, poço.ObterLitologiaPadrão(), parâmetrosLOT, depleção
                    , input.Coeficiente, breakout, relaçãoTensão, fraturasTrechosVerticais, poço.DadosGerais.Geometria, poço.DadosGerais, input.TensãoHorizontalMaiorMetodologiaCálculo, out var calc);

                ////se for valida, executa o calculo
                if (result.IsValid)
                    calc.Execute();
                else
                    return CriarCálculoTensõesInSituOutput.CálculoNãoCriado(string.Join(";\n", result.Errors));

                //validção dos irmãos
                var validator = new CálculoValidator(_poçoReadOnlyRepository, poço);
                var resultUseCase = validator.Validate((Cálculo)calc);

                if (resultUseCase.IsValid)
                {
                    await _poçoWriteOnlyRepository.CriarCálculo(poço, (Cálculo)calc, "Tensões");
                    return CriarCálculoTensõesInSituOutput.CálculoCriado(calc);
                }

                return CriarCálculoTensõesInSituOutput.CálculoNãoCriado(string.Join(";\n", result.Errors));
            }
            catch (Exception e)
            {
                return CriarCálculoTensõesInSituOutput.CálculoNãoCriado(e.Message);
            }
        }

        private async Task<FraturasTrechosVerticaisDTO> PreencherFraturasTrechosVerticais(FraturasTrechosVerticaisInput fraturasTrechosVerticais)
        {
            if (fraturasTrechosVerticais == null)
                return null;

            var dto = new FraturasTrechosVerticaisDTO();
            dto.PerfilGPORO = await _perfilReadOnlyRepository.ObterPerfil(fraturasTrechosVerticais.PerfilGPOROId);
            dto.PerfilGPOROId = fraturasTrechosVerticais.PerfilGPOROId;
            dto.PerfilRESTR = await _perfilReadOnlyRepository.ObterPerfil(fraturasTrechosVerticais.PerfilRESTRId);
            dto.PerfilRESTRId = fraturasTrechosVerticais.PerfilRESTRId;
            dto.Azimuth = fraturasTrechosVerticais.Azimuth;
            dto.TrechosFratura = new List<FraturaTrechosVerticaisValoresDTO>();

            foreach (var item in fraturasTrechosVerticais.TrechosFratura)
            {
                var trecho = new FraturaTrechosVerticaisValoresDTO
                {
                    PesoFluido = item.PesoFluido,
                    PM = item.PM
                };

                dto.TrechosFratura.Add(trecho);
            }

            return dto;
        }

        private async Task<RelaçãoTensãoDTO> PreencherRelaçãoTensão(RelaçãoTensãoInput relaçãoTensão)
        {
            if (relaçãoTensão == null)
                return null;

            var dto = new RelaçãoTensãoDTO();
            dto.AZTHMenor = relaçãoTensão.AZTHMenor;

            if (!string.IsNullOrEmpty(relaçãoTensão.PerfilGPOROId))
            {
                dto.PerfilGPORO = await _perfilReadOnlyRepository.ObterPerfil(relaçãoTensão.PerfilGPOROId);
                dto.PerfilGPOROId = relaçãoTensão.PerfilGPOROId;
            }

            if (!string.IsNullOrEmpty(relaçãoTensão.PerfilRelaçãoTensãoId))
            {
                dto.PerfilRelaçãoTensão = await _perfilReadOnlyRepository.ObterPerfil(relaçãoTensão.PerfilRelaçãoTensãoId);
                dto.PerfilRelaçãoTensãoId = relaçãoTensão.PerfilRelaçãoTensãoId;
            }

            if (!string.IsNullOrEmpty(relaçãoTensão.PerfilTVERTId))
            {
                dto.PerfilTVERT = await _perfilReadOnlyRepository.ObterPerfil(relaçãoTensão.PerfilTVERTId);
                dto.PerfilTVERTId = relaçãoTensão.PerfilTVERTId;
            }

            switch (relaçãoTensão.TipoRelação)
            {
                case "THORm`/THORM`":
                    dto.TipoRelação = TipoRelaçãoEntreTensãoEnum.THORmLinhaXTHORMLinha;
                    break;
                case "THORM`/TVERT`":
                    dto.TipoRelação = TipoRelaçãoEntreTensãoEnum.THORMLinhaXTVertLinha;
                    break;
                case "THORm/THORM":
                    dto.TipoRelação = TipoRelaçãoEntreTensãoEnum.THORmXTHORM;
                    break;
                case "THORM/TVERT":
                    dto.TipoRelação = TipoRelaçãoEntreTensãoEnum.THORMXTVert;
                    break;
                default:
                    dto.TipoRelação = TipoRelaçãoEntreTensãoEnum.NãoEspecificado;
                    break;
            }

            return dto;
        }

        private async Task<BreakoutDTO> PreencherBreakout(BreakoutInput breakout)
        {
            if (breakout == null)
                return null;

            var dto = new BreakoutDTO();
            dto.Azimuth = breakout.Azimuth;
            dto.PerfilANGAT = await _perfilReadOnlyRepository.ObterPerfil(breakout.PerfilANGATId);
            dto.PerfilANGATId = breakout.PerfilANGATId;
            dto.PerfilGPORO = await _perfilReadOnlyRepository.ObterPerfil(breakout.PerfilGPOROId);
            dto.PerfilGPOROId = breakout.PerfilGPOROId;
            dto.PerfilUCS = await _perfilReadOnlyRepository.ObterPerfil(breakout.PerfilUCSId);
            dto.PerfilUCSId = breakout.PerfilUCSId;
            dto.TrechosBreakout = new List<BreakoutValoresDTO>();

            foreach (var item in breakout.TrechosBreakout)
            {
                var trecho = new BreakoutValoresDTO
                {
                    Azimute = item.Azimute,
                    Largura = item.Largura,
                    PesoFluido = item.PesoFluido,
                    PM = item.PM
                };

                dto.TrechosBreakout.Add(trecho);
            }

            return dto;

        }

        private async Task<DepleçãoDTO> PreencherDepleção(DepleçãoInput depleção)
        {
            if (depleção == null)
                return null;

            var dto = new DepleçãoDTO();
            dto.GPOROOriginal = await _perfilReadOnlyRepository.ObterPerfil(depleção.GPOROOriginalId);
            dto.GPOROOriginalId = depleção.GPOROOriginalId;
            dto.Biot = await _perfilReadOnlyRepository.ObterPerfil(depleção.BiotId);
            dto.BiotId = depleção.BiotId;
            dto.GPORODepletada = await _perfilReadOnlyRepository.ObterPerfil(depleção.GPORODepletadaId);
            dto.GPORODepletadaId = depleção.GPORODepletadaId;
            dto.Poisson = await _perfilReadOnlyRepository.ObterPerfil(depleção.PoissonId);
            dto.PoissonId = depleção.PoissonId;

            return dto;
        }

        private List<ParâmetrosLotDTO> PreencherParâmetrosLOT(List<LotInput> listaLot)
        {
            if (listaLot == null || !listaLot.Any())
                return null;

            var dto = new List<ParâmetrosLotDTO>();

            foreach (var input in listaLot)
            {
                var lot = new ParâmetrosLotDTO
                {
                    GradPressãoPoros = input.GradPressãoPoros,
                    Lda = input.LDA,
                    Lot = input.LOT,
                    MesaRotativa = input.MR,
                    ProfundidadeVertical = input.PV,
                    Tvert = input.TVert
                };

                dto.Add(lot);
            }

            return dto;
        }

        private async Task<List<PerfilBase>> PreencherPerfisEntrada(CriarCálculoTensõesInSituInput input)
        {
            var perfisEntrada = new List<PerfilBase>();

            if (!string.IsNullOrEmpty(input.PerfilGPOROId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilGPOROId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilPoissonId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilPoissonId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilTensãoVerticalId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilTensãoVerticalId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilTHORminId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilTHORminId);
                perfisEntrada.Add(perfil);
            }

            if (input.TensãoHorizontalMaiorMetodologiaCálculo == MetodologiaCálculoTensãoHorizontalMaiorEnum.RelaçãoEntreTensões)
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.RelaçãoTensão.PerfilRelaçãoTensãoId);
                perfisEntrada.Add(perfil);
            }

            return perfisEntrada;
        }

        private List<PerfilBase> PreencherListaPerfisSaída(string nomeCálculo, Poço poço, MetodologiaCálculoTensãoHorizontalMenorEnum metodologiaCálculo)
        {
            var listaPerfil = new List<PerfilBase>();

            var perfilThorMin = PerfisFactory.Create("THORmin", "THORmin_" + nomeCálculo, poço.Trajetória, poço.ObterLitologiaPadrão());
            var perfilThorMax = PerfisFactory.Create("THORmax", "THORmax_" + nomeCálculo, poço.Trajetória, poço.ObterLitologiaPadrão());
            var perfilGFrat = PerfisFactory.Create("GFRAT_σh", "GFRAT(σh)_" + nomeCálculo, poço.Trajetória, poço.ObterLitologiaPadrão());
            var perfilGTHORMax = PerfisFactory.Create("GTHORmax", "GTHORmax_" + nomeCálculo, poço.Trajetória, poço.ObterLitologiaPadrão());
            var perfilAZTHmin = PerfisFactory.Create("AZTHmin", "AZTHmin_" + nomeCálculo, poço.Trajetória, poço.ObterLitologiaPadrão());

            listaPerfil.Add(perfilThorMin);
            listaPerfil.Add(perfilThorMax);
            listaPerfil.Add(perfilGFrat);
            listaPerfil.Add(perfilGTHORMax);
            listaPerfil.Add(perfilAZTHmin);

            if (metodologiaCálculo == MetodologiaCálculoTensãoHorizontalMenorEnum.K0Acompanhamento || metodologiaCálculo == MetodologiaCálculoTensãoHorizontalMenorEnum.K0)
            {
                var k0 = PerfisFactory.Create("K0", "K0_" + nomeCálculo, poço.Trajetória, poço.ObterLitologiaPadrão());
                listaPerfil.Add(k0);
            }

                return listaPerfil;
        }
    }
}
