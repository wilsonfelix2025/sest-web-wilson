using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.CálculosUseCase.Pipeline;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Sobrecarga.Factory;
using SestWeb.Domain.Entities.Perfis.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoSobrecarga.EditarCálculo
{
    internal class EditarCálculoSobrecargaUseCase : IEditarCálculoSobrecargaUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly ICálculoSobrecargaFactory _cálculoFactory;
        private readonly IPipelineUseCase _pipeUseCase;


        public EditarCálculoSobrecargaUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository,
            IPoçoReadOnlyRepository poçoReadOnlyRepository, ICálculoSobrecargaFactory cálculoFactory, IPipelineUseCase pipelineUseCase)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _cálculoFactory = cálculoFactory;
            _pipeUseCase = pipelineUseCase;
        }

        public async Task<EditarCálculoSobrecargaOutput> Execute(EditarCálculoSobrecargaInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoço(input.IdPoço);

                if (poço == null)
                {
                    return EditarCálculoSobrecargaOutput.CálculoSobrecargaNãoEditado("poço não encontrado.");
                }

                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.IdRhob);

                if (perfil == null)
                {
                    return EditarCálculoSobrecargaOutput.CálculoSobrecargaNãoEditado("perfil não encontrado.");
                }

                var cálculo = await _poçoReadOnlyRepository.ObterCálculo(input.IdPoço, input.IdCálculoAntigo);

                if (cálculo == null)
                {
                    return EditarCálculoSobrecargaOutput.CálculoSobrecargaNãoEditado("cálculo não encontrado.");
                }


                var perfisEntrada = new List<PerfilBase> { perfil };
                var sobrecargaPerfisSaida = new List<PerfilBase>();

                foreach (var idPerfil in cálculo.PerfisSaída.IdPerfis)
                {
                    var perfilSaída = await _perfilReadOnlyRepository.ObterPerfil(idPerfil);
                    sobrecargaPerfisSaida.Add(perfilSaída);
                }

                var result = _cálculoFactory.CreateCálculoSobrecarga(input.Nome, "Sobrecarga", perfisEntrada, sobrecargaPerfisSaida, poço.Trajetória,
                    poço.Litologias.FirstOrDefault(), poço.DadosGerais, out var calc);
                calc.Execute();

                await _poçoWriteOnlyRepository.EditarCálculo(poço, (Cálculo) calc, input.IdCálculoAntigo, "Sobrecarga");

                var perfisAlterados = await _pipeUseCase.Execute(poço, calc, input.IdCálculoAntigo);

                return EditarCálculoSobrecargaOutput.CálculoSobrecargaEditado(calc, perfisAlterados);
            }
            catch (Exception e)
            {
                return EditarCálculoSobrecargaOutput.CálculoSobrecargaNãoEditado(e.Message);
            }
        }
    }
}
