using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Application.Validadores;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.ExpoenteD.Factory;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Poço;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoExpoenteD.CriarCálculo
{
    public class CriarCálculoExpoenteDUseCase : ICriarCálculoExpoenteDUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly ICálculoExpoenteDFactory _cálculoFactory;

        public CriarCálculoExpoenteDUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository, ICálculoExpoenteDFactory cálculoFactory)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _cálculoFactory = cálculoFactory;
        }

        public async Task<CriarCálculoExpoenteDOutput> Execute(CriarCálculoExpoenteDInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);
                if (poço == null)
                    return CriarCálculoExpoenteDOutput.CálculoExpoenteDNãoCriado("Poço não encontrado");

                ////preparação para as entradas para a factory
                var listaPerfisEntrada = await PreencherPerfisEntrada(input);
                var listaPerfisSaída = PreencherListaPerfisSaída(input.Nome, poço);

                ////retorna se a entidade calculo criada está valida ou não
                var result = _cálculoFactory.CreateCálculoExpoenteD(input.Nome, "ExpoenteD"
                    , listaPerfisEntrada.ToList(), listaPerfisSaída, poço.Trajetória, poço.ObterLitologiaPadrão(), input.Correlação, out var calc);

                ////se for valida, executa o calculo
                if (result.IsValid)
                    calc.Execute();
                else
                    return CriarCálculoExpoenteDOutput.CálculoExpoenteDNãoCriado(string.Join(";\n", result.Errors));

                //validação dos irmãos
                var validator = new CálculoValidator(_poçoReadOnlyRepository, poço);
                var resultUseCase = validator.Validate((Cálculo)calc);

                if (resultUseCase.IsValid)
                {
                    await _poçoWriteOnlyRepository.CriarCálculo(poço, (Cálculo)calc, "ExpoenteD", true);
                    return CriarCálculoExpoenteDOutput.CálculoExpoenteDCriado(calc);
                }

                return CriarCálculoExpoenteDOutput.CálculoExpoenteDNãoCriado(string.Join(";\n", result.Errors));
            }
            catch (Exception e)
            {
                return CriarCálculoExpoenteDOutput.CálculoExpoenteDNãoCriado(e.Message);
            }
        }

        private async Task<IReadOnlyCollection<PerfilBase>> PreencherPerfisEntrada(CriarCálculoExpoenteDInput input)
        {
            var perfisEntrada = new List<PerfilBase>();

            if (!string.IsNullOrEmpty(input.PerfilROPId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilROPId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilRPMId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilRPMId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilWOBId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilWOBId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilDIAM_BROCA))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilDIAM_BROCA);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilECDId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilECDId);
                perfisEntrada.Add(perfil);
            }

            return perfisEntrada;
        }

        private List<PerfilBase> PreencherListaPerfisSaída(string nomeCálculo, Poço poço)
        {
            var listaPerfil = new List<PerfilBase>();

            var perfilExpD = PerfisFactory.Create("ExpoenteD", "ExpoenteD_" + nomeCálculo, poço.Trajetória, poço.ObterLitologiaPadrão());

            listaPerfil.Add(perfilExpD);

            return listaPerfil;
        }
    }
}
