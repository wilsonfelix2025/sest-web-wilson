using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.Helpers;
using SestWeb.Application.UseCases.CálculosUseCase.Pipeline;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Base;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Factory;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Reservatório;
using SestWeb.Domain.Entities.Correlações.ParâmetroCorrelação;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.EditarCálculo
{
    internal class EditarCálculoPressãoPorosUseCase : IEditarCálculoPressãoPorosUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly ICálculoPressãoPorosFactory _cálculoFactory;
        private readonly IPipelineUseCase _pipeUseCase;


        public EditarCálculoPressãoPorosUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository,
            IPoçoReadOnlyRepository poçoReadOnlyRepository, ICálculoPressãoPorosFactory cálculoFactory, IPipelineUseCase pipelineUseCase)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _cálculoFactory = cálculoFactory;
            _pipeUseCase = pipelineUseCase;
        }

        public async Task<EditarCálculoPressãoPorosOutput> Execute(EditarCálculoPressãoPorosInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);
                var cálculo = await _poçoReadOnlyRepository.ObterCálculo(input.IdPoço, input.IdCálculo);
                var perfisEntrada = new List<PerfilBase>();
                var perfisSaída = new List<PerfilBase>();
                var parâmetrosCorrelação = new List<ParâmetroCorrelação>();
                CorrelaçãoPressãoPoros tipoCálculo;

                foreach (var idPerfil in cálculo.PerfisSaída.IdPerfis)
                {
                    var perfil = await _perfilReadOnlyRepository.ObterPerfil(idPerfil);
                    perfisSaída.Add(perfil);
                }

                PressãoPorosHelper.ExtrairCorrelações(input, parâmetrosCorrelação);

                if (input.Tipo == "PP")
                {
                    var perfilFiltrado = await _perfilReadOnlyRepository.ObterPerfil(input.IdPerfilFiltrado);
                    if (perfilFiltrado == null)
                    {
                        return EditarCálculoPressãoPorosOutput.CálculoPressãoPorosNãoCriado("ID do perfil filtrado não foi encontrado.");
                    }

                    var perfilSobrecarga = await _perfilReadOnlyRepository.ObterPerfil(input.IdGradienteSobrecarga);
                    if (perfilSobrecarga == null)
                    {
                        return EditarCálculoPressãoPorosOutput.CálculoPressãoPorosNãoCriado("ID do gradiente de sobrecarga não foi encontrado.");
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
                        return EditarCálculoPressãoPorosOutput.CálculoPressãoPorosNãoCriado("método de cálculo inválido");
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
                            return EditarCálculoPressãoPorosOutput.CálculoPressãoPorosNãoCriado("ID do perfil filtrado não foi encontrado.");
                        }
                        perfisEntrada.Add(perfilEntrada);
                    }
                }
                else
                {
                    return EditarCálculoPressãoPorosOutput.CálculoPressãoPorosNãoCriado($"tipo de cálculo inválido - {input.Tipo}");
                }

                DadosReservatório reservatório = PressãoPorosHelper.PreencherDadosReservatório(input.Reservatório);

                var result = _cálculoFactory.CreateCálculoPressãoPoros(input.Nome, "PressãoPoros", perfisEntrada, perfisSaída,
                    tipoCálculo, parâmetrosCorrelação,
                    poço.Trajetória, poço.ObterLitologiaPadrão(), poço.DadosGerais, reservatório, out var calc);

                if (result.IsValid)
                {
                    calc.Execute();
                    calc.PerfisSaída.IdPerfis = calc.PerfisSaída.Perfis.Select(x => x.Id.ToString()).ToList();

                    await _poçoWriteOnlyRepository.EditarCálculo(poço, (Cálculo) calc, input.IdCálculo, "PressãoPoros");

                    var perfisAlterados = await _pipeUseCase.Execute(poço, calc, input.IdCálculo);

                    return EditarCálculoPressãoPorosOutput.CálculoPressãoPorosCriado(calc, perfisAlterados);
                }
                else
                {
                    return EditarCálculoPressãoPorosOutput.CálculoPressãoPorosNãoCriado(result.Errors.First().ErrorMessage);
                }
            }
            catch (Exception e)
            {
                return EditarCálculoPressãoPorosOutput.CálculoPressãoPorosNãoCriado(e.Message);
            }
        }
    }
}
