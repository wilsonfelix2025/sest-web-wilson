using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using SestWeb.Application.Repositories;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase.ImportarPerfilUseCase
{
    internal class ImportarDadosPerfilUseCase : ImportarDadosUseCase<List<PerfilInput>>, IImportarDadosPerfilUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;

        public ImportarDadosPerfilUseCase(IPoçoWriteOnlyRepository poçoWriteOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository
                                         ,IPerfilWriteOnlyRepository perfilWriteOnlyRepository, IPerfilReadOnlyRepository perfilReadOnlyRepository)
            : base(poçoWriteOnlyRepository, poçoReadOnlyRepository, perfilWriteOnlyRepository)
        {
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
        }

        public async Task<ImportarDadosOutput> Execute(string idPoço, List<PerfilInput> listaPerfil)
        {
            try
            {
                Log.Information("Recebendo dados da planilha para importação de perfil do poço " + idPoço);
                var resultadoValidação = await ValidaçõesIniciais(idPoço, listaPerfil);

                if (!string.IsNullOrWhiteSpace(resultadoValidação))
                {
                    return ImportarDadosOutput.ImportaçãoComFalhasDeValidação(resultadoValidação);
                }

                var poçoDto = PreencherDadosDTO(listaPerfil, Poço.Trajetória);
                var perfisParaImportar = PreencherPefilParaImportar(listaPerfil);

                var dadosSelecionados = new List<DadosSelecionadosEnum>();

                var resultFactory =
                    PoçoFactory.EditarPoço(poçoDto, Poço, perfisParaImportar, null, dadosSelecionados, null);

                if (resultFactory.result.IsValid == false)
                {
                    return ImportarDadosOutput.ImportaçãoComFalhasDeValidação(string.Join("\n", resultFactory.result.Errors));
                }

                var poçoEntity = (Poço)resultFactory.Entity;
                await _perfilWriteOnlyRepository.AtualizarPerfis(poçoEntity.Id, Poço);

                List<PerfilBase> perfisImportados = await GetPerfisImportados(perfisParaImportar, idPoço);

                return ImportarDadosOutput.ImportadoComSucesso(perfisImportados);
            }
            catch (Exception e)
            {
                Log.Error("Erro ao importar dados de planilha-litologia dados do poço " + idPoço + ". Erro: " + e.Message);
                return ImportarDadosOutput.ImportaçãoComFalha(e.Message);
            }
        }

        private async Task<List<PerfilBase>> GetPerfisImportados(List<PerfilParaImportarDTO> perfisParaImportar, string idPoço)
        {
            var perfiSImportados = new List<PerfilBase>(perfisParaImportar.Count);

            foreach (var perfil in perfisParaImportar)
            {
                var perfisDoPoço = await _perfilReadOnlyRepository.ObterPerfisDeUmPoço(idPoço);
                var perfilImportado = perfisDoPoço.First(p => p.Nome == perfil.Nome);
                perfiSImportados.Add(perfilImportado);
            }

            return perfiSImportados;
        }

        private PoçoDTO PreencherDadosDTO(List<PerfilInput> listaPerfil, Trajetória trajetória)
        {
            List<PerfilDTO> listaPerfilDto = new List<PerfilDTO>();
            
            foreach (var perfil in listaPerfil)
            {
                PerfilDTO perfilDto = new PerfilDTO
                {
                    Nome = perfil.Nome,
                    Mnemonico = perfil.TipoPerfil,
                    TipoProfundidade = perfil.TipoProfundidade.ToString(),
                    Unidade = perfil.Unidade
                };

                foreach (var ponto in perfil.PontosPerfil)
                {
                    var valorPm = 0.0; 
                    var pmÉDouble = double.TryParse(ponto.PM,NumberStyles.Any,CultureInfo.InvariantCulture, out valorPm);

                    if (perfil.TipoProfundidade == TipoProfundidade.Cota && pmÉDouble)
                    {
                        if (valorPm < 0)
                            valorPm = valorPm * -1;
                    }

                    var pontoDTO = new PontoDTO
                    {
                        Pm = pmÉDouble ? valorPm.ToString() : ponto.PM,
                        Origem = OrigemPonto.Importado,
                        Valor = ponto.Valor
                    };

                    perfilDto.PontosDTO.Add(pontoDTO);
                }

                listaPerfilDto.Add(perfilDto);
            }            

            PoçoDTO poçoDTO = PoçoDTOFactory.CriarPoçoDTO(null, null, null, listaPerfilDto, null, null, null, null, null);

            return poçoDTO;
        }

        private List<PerfilParaImportarDTO> PreencherPefilParaImportar(List<PerfilInput> perfis)
        {
            var list = new List<PerfilParaImportarDTO>();

            foreach (var perfil in perfis)
            {
                var perfilParaImportar = new PerfilParaImportarDTO
                {
                    Tipo = perfil.TipoPerfil,
                    Unidade = perfil.Unidade,
                    ValorTopo = perfil.ValorTopo,
                    ValorBase = perfil.ValorBase,
                    Nome = perfil.Nome,
                    NovoNome = perfil.NovoNome,
                    Ação = perfil.Ação
                };
                list.Add(perfilParaImportar);
            }

            return list;
        }
    }
}
