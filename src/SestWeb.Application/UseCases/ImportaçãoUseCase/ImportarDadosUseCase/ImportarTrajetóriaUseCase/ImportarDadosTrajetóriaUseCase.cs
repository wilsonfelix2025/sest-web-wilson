using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using SestWeb.Application.Repositories;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase.ImportarTrajetóriaUseCase
{
    internal class ImportarDadosTrajetóriaUseCase : ImportarDadosUseCase<TrajetóriaInput>, IImportarDadosTrajetóriaUseCase
    {

        public ImportarDadosTrajetóriaUseCase(IPoçoWriteOnlyRepository poçoWriteOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository
                                             ,IPerfilWriteOnlyRepository perfilWriteOnlyRepository)
        :base(poçoWriteOnlyRepository, poçoReadOnlyRepository, perfilWriteOnlyRepository)
        {
                
        }

        public async Task<ImportarDadosOutput> Execute(string idPoço, TrajetóriaInput input)
        {
            try
            {
                Log.Information("Recebendo dados da planilha para importação de trajetória do poço " + idPoço);
                var resultadoValidação = await ValidaçõesIniciais(idPoço, input);

                if (!string.IsNullOrWhiteSpace(resultadoValidação))
                {
                    return ImportarDadosOutput.ImportaçãoComFalhasDeValidação(resultadoValidação);
                }

                var poçoDto = PreencherDadosDTO(input);

                var dadosSelecionados = new List<DadosSelecionadosEnum>();
                dadosSelecionados.Add(DadosSelecionadosEnum.Trajetória);

              
                var resultFactory =
                    PoçoFactory.EditarPoço(poçoDto, Poço, null, null, dadosSelecionados, null);

                if (resultFactory.result.IsValid == false)
                {
                    return ImportarDadosOutput.ImportaçãoComFalhasDeValidação(string.Join("\n", resultFactory.result.Errors));
                }

                var poçoEntity = (Poço)resultFactory.Entity;
                var result = await _poçoWriteOnlyRepository.AtualizarTrajetória(poçoEntity.Id, poçoEntity.Trajetória);

                if (result)
                {
                    return ImportarDadosOutput.ImportadoComSucesso();
                }

                return ImportarDadosOutput.ImportaçãoComFalha();
            }
            catch (Exception e)
            {
                Log.Error("Erro ao importar dados de planilha-trajetória do poço " + idPoço + ". Erro: " + e.Message);
                return ImportarDadosOutput.ImportaçãoComFalha(e.Message);
            }
        }

        private PoçoDTO PreencherDadosDTO(TrajetóriaInput trajetória)
        {
            var trajetóriaDTO = new TrajetóriaDTO();

            trajetóriaDTO.TipoDeAção = trajetória.Ação;
            trajetóriaDTO.ValorBase = trajetória.ValorBase;
            trajetóriaDTO.ValorTopo = trajetória.ValorTopo;

            foreach (var ponto in trajetória.PontoTrajetória)
            {
                var pontoDTO = new PontoTrajetóriaDTO
                {
                    Pm = ponto.PM,
                    Inclinação = ponto.Inclinação,
                    Azimute = ponto.Azimute
                };

                trajetóriaDTO.Pontos.Add(pontoDTO);
            }

            PoçoDTO poçoDTO = PoçoDTOFactory.CriarPoçoDTO(trajetóriaDTO, null, null, null, null, null, null, null, null);

            return poçoDTO;
        }
    }
}
