using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Serilog;
using SestWeb.Application.Repositories;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase.ImportarLitologiaUseCase
{
    internal class ImportarDadosLitologiaUseCase : ImportarDadosUseCase<LitologiaInput>, IImportarDadosLitologiaUseCase
    {

        public ImportarDadosLitologiaUseCase(IPoçoWriteOnlyRepository poçoWriteOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository
                                            ,IPerfilWriteOnlyRepository perfilWriteOnlyRepository)
            : base(poçoWriteOnlyRepository, poçoReadOnlyRepository, perfilWriteOnlyRepository)
        {
        }

        public async Task<ImportarDadosOutput> Execute(string idPoço, LitologiaInput litologia)
        {
            try
            {
                Log.Information("Recebendo dados da planilha para importação de litologia do poço " + idPoço);
                var resultadoValidação = await ValidaçõesIniciais(idPoço, litologia);

                if (!string.IsNullOrWhiteSpace(resultadoValidação))
                {
                    return ImportarDadosOutput.ImportaçãoComFalhasDeValidação(resultadoValidação);
                }

                var litologiasParaImportar = PreencherLitologiasParaImportar(litologia);
                var poçoDto = PreencherDadosDTO(litologia, Poço.Trajetória);

                var dadosSelecionados = new List<DadosSelecionadosEnum>();

                var resultFactory =
                    PoçoFactory.EditarPoço(poçoDto, Poço, null, litologiasParaImportar, dadosSelecionados, null);

                if (resultFactory.result.IsValid == false)
                {
                    return ImportarDadosOutput.ImportaçãoComFalhasDeValidação(string.Join("\n", resultFactory.result.Errors));
                }

                var poçoEntity = (Poço)resultFactory.Entity;

                if (litologia.CorreçãoMesaRotativa != null && litologia.CorreçãoMesaRotativa != "" && litologia.TipoProfundidade != TipoProfundidade.Cota)
                {
                    double.TryParse(litologia.CorreçãoMesaRotativa, NumberStyles.Any, CultureInfo.InvariantCulture, out var doubleCorreçãoMR);
                    
                    foreach (var lito in poçoEntity.Litologias)
                    {
                        if (lito.Classificação.Nome == litologia.TipoLitologia)
                        {
                            lito.Shift(doubleCorreçãoMR);
                        }
                    }
                }

                var result = await _poçoWriteOnlyRepository.AtualizarLitologias(poçoEntity.Id, poçoEntity);

                if (result)
                {
                    return ImportarDadosOutput.ImportadoComSucesso();
                }

                return ImportarDadosOutput.ImportaçãoComFalha();
            }
            catch (Exception e)
            {
                Log.Error("Erro ao importar dados de planilha-litologia dados do poço " + idPoço + ". Erro: " + e.Message);
                return ImportarDadosOutput.ImportaçãoComFalha(e.Message);
            }
        }

        private PoçoDTO PreencherDadosDTO(LitologiaInput litologia, Trajetória trajetória)
        {
            List<LitologiaDTO> listaLitologia = new List<LitologiaDTO>();
            LitologiaDTO litologiaDto = new LitologiaDTO();

            foreach (var ponto in litologia.PontosLitologia)
            {
                var valorPm = ponto.PM;

                if (litologia.TipoProfundidade == TipoProfundidade.Cota)
                {
                    if (valorPm < 0)
                        valorPm = valorPm * -1;

                }

                var pontoDTO = new PontoLitologiaDTO
                {
                    Pm = valorPm.ToString(),
                    TipoRocha = ponto.TipoRocha
                };

                litologiaDto.Nome = litologia.Nome;
                litologiaDto.Classificação = litologia.TipoLitologia;
                litologiaDto.TipoProfundidade = litologia.TipoProfundidade.ToString();

                litologiaDto.Pontos.Add(pontoDTO);
            }

            listaLitologia.Add(litologiaDto);

            PoçoDTO poçoDTO = PoçoDTOFactory.CriarPoçoDTO(null, listaLitologia, null, null, null, null, null, null, null);

            return poçoDTO;
        }

        private List<LitologiaParaImportarDTO> PreencherLitologiasParaImportar(LitologiaInput litologia)
        {
            var list = new List<LitologiaParaImportarDTO>();

            var litologiaParaImportar = new LitologiaParaImportarDTO
            {
                ValorTopo = litologia.ValorTopo,
                ValorBase = litologia.ValorBase,
                NovoNome = litologia.NovoNome,
                Nome = litologia.Nome,
                Ação = litologia.Ação
            };

            list.Add(litologiaParaImportar);

            return list;
        }
    }
}
