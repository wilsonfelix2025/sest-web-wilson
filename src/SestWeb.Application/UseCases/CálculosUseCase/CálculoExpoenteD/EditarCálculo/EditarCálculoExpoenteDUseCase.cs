using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.CálculosUseCase.Pipeline;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.ExpoenteD.Factory;
using SestWeb.Domain.Entities.Cálculos.Tensões;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoExpoenteD.EditarCálculo
{

    public class EditarCálculoExpoenteDUseCase : IEditarCálculoExpoenteDUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly ICálculoExpoenteDFactory _cálculoFactory;
        private readonly IPipelineUseCase _pipeUseCase;


        public EditarCálculoExpoenteDUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository, ICálculoExpoenteDFactory cálculoFactory, IPipelineUseCase pipelineUseCase)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _cálculoFactory = cálculoFactory;
            _cálculoFactory = cálculoFactory;
            _pipeUseCase = pipelineUseCase;
        }

        public async Task<EditarCálculoExpoenteDOutput> Execute(EditarCálculoExpoenteDInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);
                if (poço == null)
                    return EditarCálculoExpoenteDOutput.CálculoNãoEditado("Poço não encontrado");

                var cálculo = (Domain.Entities.Cálculos.ExpoenteD.CálculoExpoenteD)poço.Cálculos.First(c => c.Id.ToString() == input.IdCálculo);

                //preparação para as entradas para a factory
                var listaPerfisEntrada = await PreencherPerfisEntrada(input);
                var listaPerfisSaída = await PreencherListaPerfisSaída(cálculo.PerfisSaída.IdPerfis);

                //retorna se a entidade calculo criada está valida ou não
                var result = _cálculoFactory.CreateCálculoExpoenteD(input.Nome, "ExpoenteD"
                    , listaPerfisEntrada.ToList(), listaPerfisSaída, poço.Trajetória, poço.ObterLitologiaPadrão(), input.Correlação, out var calc);

                //se for valida, executa o calculo
                if (result.IsValid)
                    calc.Execute();
                else
                    return EditarCálculoExpoenteDOutput.CálculoNãoEditado(string.Join(";\n", result.Errors));

                //validação dos irmãos
                var validator = new Validadores.CálculoValidator(_poçoReadOnlyRepository, poço);
                var resultUseCase = validator.Validate((Cálculo)calc);

                if (resultUseCase.IsValid)
                {
                    await _poçoWriteOnlyRepository.EditarCálculo(poço, (Cálculo)calc, input.IdCálculo, "ExpoenteD");

                    var perfisAlterados = await _pipeUseCase.Execute(poço, calc, input.IdCálculo);

                    return EditarCálculoExpoenteDOutput.CálculoEditado(calc, perfisAlterados);
                }

                return EditarCálculoExpoenteDOutput.CálculoNãoEditado(string.Join(";\n", result.Errors));
            }
            catch (Exception e)
            {
                return EditarCálculoExpoenteDOutput.CálculoNãoEditado(e.Message);
            }
        }

        private async Task<IReadOnlyCollection<PerfilBase>> PreencherPerfisEntrada(EditarCálculoExpoenteDInput input)
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

        private async Task<IList<PerfilBase>> PreencherListaPerfisSaída(List<string> perfis)
        {
            var listaPerfil = new List<PerfilBase>();

            foreach (var perfil in perfis)
            {
                var perfilSaida = await _perfilReadOnlyRepository.ObterPerfil(perfil);
                listaPerfil.Add(perfilSaida);
            }

            return listaPerfil;
        }
    }
}
