using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.CálculosUseCase.Pipeline;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.UseCases.TrendUseCases.EditarTrend
{
    internal class EditarTrendUseCase : IEditarTrendUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPerfilWriteOnlyRepository _perfilWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPipelineUseCase _pipeUseCase;


        public EditarTrendUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPerfilWriteOnlyRepository perfilWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository, IPipelineUseCase pipelineUseCase)
        {
            _perfilWriteOnlyRepository = perfilWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _pipeUseCase = pipelineUseCase;
        }

        public async Task<EditarTrendOutput> Execute(List<EditarTrechosInput> input, string idPerfil, string nomeTrend)
        {
            try
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(idPerfil);
                if (perfil == null)
                    return EditarTrendOutput.TrendNãoEditado("Trend não pode ser editado. Perfil não encontrado");

                var poço = await _poçoReadOnlyRepository.ObterPoço(perfil.IdPoço);

                var dto = PreencherTrechos(input, nomeTrend);
                var factory = new TrendFactory();
                var result = factory.EditarTrend(perfil, dto, poço);

                if (result.result.IsValid == false)
                    return EditarTrendOutput.TrendNãoEditado(string.Join("\n", result.result.Errors));

                var perfilEditado = (PerfilBase) result.Entity;

                await _perfilWriteOnlyRepository.AtualizarTrendDoPerfil(perfilEditado);

                var perfisAlterados = await _pipeUseCase.Execute(poço, perfil.Trend);

                return EditarTrendOutput.TrendEditado(perfilEditado.Trend, perfisAlterados);
            }
            catch (Exception e)
            {
                return EditarTrendOutput.TrendNãoEditado(e.Message);
            }
        }

        private TrendDTO PreencherTrechos(List<EditarTrechosInput> input, string nomeTrend)
        {
            var trechos = new List<TrechoTrendDTO>();
            var dto = new TrendDTO();

            foreach (var inputTrecho in input)
            {
                var trecho = new TrechoTrendDTO
                {
                    ValorTopo = inputTrecho.ValorTopo,
                    ValorBase = inputTrecho.ValorBase,
                    PvTopo = inputTrecho.PvTopo,
                    PvBase = inputTrecho.PvBase,
                    Inclinação = inputTrecho.Inclinação
                };

                trechos.Add(trecho);
            }

            dto.Trechos = trechos;
            dto.NomeTrend = nomeTrend;

            return dto;
        }
    }
}
