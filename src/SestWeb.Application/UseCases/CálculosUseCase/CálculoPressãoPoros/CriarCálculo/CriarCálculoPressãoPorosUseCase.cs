using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.Helpers;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Base;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Factory;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Reservatório;
using SestWeb.Domain.Entities.Correlações.ParâmetroCorrelação;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using SestWeb.Domain.Entities.Poço;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.CriarCálculo
{
    internal class CriarCálculoPressãoPorosUseCase : ICriarCálculoPressãoPorosUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly ICálculoPressãoPorosFactory _cálculoFactory;

        public CriarCálculoPressãoPorosUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository,
            IPoçoReadOnlyRepository poçoReadOnlyRepository, ICálculoPressãoPorosFactory cálculoFactory)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _cálculoFactory = cálculoFactory;
        }

        public async Task<CriarCálculoPressãoPorosOutput> Execute(CriarCálculoPressãoPorosInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);
                var pressaoPorosPerfisSaida = new List<PerfilBase>();
                var parâmetrosCorrelação = new List<ParâmetroCorrelação>();
                var perfisEntrada = new List<PerfilBase>();
                CorrelaçãoPressãoPoros tipoCálculo;

                if (poço == null)
                {
                    return CriarCálculoPressãoPorosOutput.CálculoPressãoPorosNãoCriado("poço não encontrado.");
                }

                PressãoPorosHelper.ExtrairCorrelações(input, parâmetrosCorrelação);

                if (input.Tipo == "PP")
                {
                    var perfilFiltrado = await _perfilReadOnlyRepository.ObterPerfil(input.IdPerfilFiltrado);
                    if (perfilFiltrado == null)
                    {
                        return CriarCálculoPressãoPorosOutput.CálculoPressãoPorosNãoCriado("ID do perfil filtrado não foi encontrado.");
                    }

                    var perfilSobrecarga = await _perfilReadOnlyRepository.ObterPerfil(input.IdGradienteSobrecarga);
                    if (perfilSobrecarga == null)
                    {
                        return CriarCálculoPressãoPorosOutput.CálculoPressãoPorosNãoCriado("ID do gradiente de sobrecarga não foi encontrado.");
                    }

                    if (perfilFiltrado.Mnemonico == TiposPerfil.GeTipoPerfil<DTC>().Mnemônico)
                    {
                        tipoCálculo = CorrelaçãoPressãoPoros.EatonDTC;
                    }
                    else if (perfilFiltrado.Mnemonico == TiposPerfil.GeTipoPerfil<ExpoenteD>().Mnemônico)
                    {
                        tipoCálculo = CorrelaçãoPressãoPoros.EatonExpoenteD;
                    }
                    else if (perfilFiltrado.Mnemonico == TiposPerfil.GeTipoPerfil<RESIST>().Mnemônico)
                    {
                        tipoCálculo = CorrelaçãoPressãoPoros.EatonResistividade;
                    }
                    else
                    {
                        return CriarCálculoPressãoPorosOutput.CálculoPressãoPorosNãoCriado("método de cálculo inválido");
                    }

                    perfisEntrada.Add(perfilFiltrado);
                    perfisEntrada.Add(perfilSobrecarga);
                } 
                else if (input.Tipo == "PPh")
                {
                    tipoCálculo = CorrelaçãoPressãoPoros.Hidrostática;
                } 
                else if (input.Tipo == "GPP")
                {
                    tipoCálculo = CorrelaçãoPressãoPoros.Gradiente;

                    if (input.IdPph != "" && input.IdPph != null)
                    {
                        var perfilEntrada = await _perfilReadOnlyRepository.ObterPerfil(input.IdPph);
                        if (perfilEntrada == null)
                        {
                            return CriarCálculoPressãoPorosOutput.CálculoPressãoPorosNãoCriado("ID do perfil filtrado não foi encontrado.");
                        }
                        perfisEntrada.Add(perfilEntrada);
                    }
                }
                else
                {
                    return CriarCálculoPressãoPorosOutput.CálculoPressãoPorosNãoCriado($"tipo de cálculo inválido - {input.Tipo}");
                }

                DadosReservatório reservatório = PressãoPorosHelper.PreencherDadosReservatório(input.Reservatório);

                var result = _cálculoFactory.CreateCálculoPressãoPoros(input.Nome, "PressãoPoros", perfisEntrada, pressaoPorosPerfisSaida,
                    tipoCálculo, parâmetrosCorrelação, 
                    poço.Trajetória, poço.ObterLitologiaPadrão(), poço.DadosGerais, reservatório, out var calc);

                if (result.IsValid)
                {
                    calc.Execute();
                    calc.PerfisSaída.IdPerfis = calc.PerfisSaída.Perfis.Select(x => x.Id.ToString()).ToList();

                    await _poçoWriteOnlyRepository.CriarCálculo(poço, (Cálculo)calc, "PressãoPoros");

                    return CriarCálculoPressãoPorosOutput.CálculoPressãoPorosCriado(calc);
                }
                else
                {
                    return CriarCálculoPressãoPorosOutput.CálculoPressãoPorosNãoCriado(result.Errors.First().ErrorMessage);
                }
            }
            catch (Exception e)
            {
                return CriarCálculoPressãoPorosOutput.CálculoPressãoPorosNãoCriado(e.Message);
            }
        }
    }
}
