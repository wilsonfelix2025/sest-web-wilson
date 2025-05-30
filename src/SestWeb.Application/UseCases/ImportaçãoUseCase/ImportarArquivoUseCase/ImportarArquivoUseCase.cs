using SestWeb.Domain.Importadores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using SestWeb.Application.Repositories;
using SestWeb.Application.Services;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Enums;
using SestWeb.Application.Validadores;
using SestWeb.Domain.Importadores.Shallow.Utils;
using System.Globalization;
using SestWeb.Domain.Entities.RegistrosEventos;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;

namespace SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarArquivoUseCase
{
    internal class ImportarArquivoUseCase : IImportarArquivoUseCase
    {
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPocoWebService _poçoWebService;
        private readonly IRegistrosEventosReadOnlyRepository _registrosEventosReadOnlyRepository;
        private readonly IRegistrosEventosWriteOnlyRepository _registrosEventosWriteOnlyRepository;

        public ImportarArquivoUseCase(IPoçoWriteOnlyRepository poçoWriteOnlyRepository
                                    , IPoçoReadOnlyRepository poçoReadOnlyRepository
                                    , IPocoWebService poçoWebService
                                    , IRegistrosEventosReadOnlyRepository registrosEventosReadOnlyRepository
                                    , IRegistrosEventosWriteOnlyRepository registrosEventosWriteOnlyRepository)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWebService = poçoWebService;
            _registrosEventosReadOnlyRepository = registrosEventosReadOnlyRepository;
            _registrosEventosWriteOnlyRepository = registrosEventosWriteOnlyRepository;
        }

        public async Task<ImportarArquivoOutput> Execute(List<DadosSelecionadosEnum> dadosSelecionados, string caminhoOuDadoDoArquivo, List<PerfilModel> perfisSelecionados, List<LitologiaModel> litologiasSelecionadas, string idPoço, string correçãoMesaRotativa, Dictionary<string, object> dadosExtras)
        {
            try
            {
                foreach (var perfil in perfisSelecionados)
                {
                    if (!StringUtils.HasContent(perfil.Nome) || !StringUtils.HasContent(perfil.Tipo) || !StringUtils.HasContent(perfil.Unidade))
                    {
                        var nomePerfil = perfil.NovoNome ?? perfil.Nome;
                        return ImportarArquivoOutput.ImportaçãoCanceladaPerfisIncompletos(perfil.Nome);
                    }
                }

                var profilesToImport = PreencherPefilParaImportarDTO(perfisSelecionados);
                var lithologiesToImport = PreencherLitologiasParaImportarDTO(litologiasSelecionadas);

                Log.Information("Recebendo dados para importação do poço " + idPoço + ". Arquivo: " + caminhoOuDadoDoArquivo);

                var poçoAtual = await _poçoReadOnlyRepository.ObterPoço(idPoço);
                if (poçoAtual == null)
                    return ImportarArquivoOutput.ImportaçãoComFalha("Erro ao importar os dados. Poço não encontrado.");

                var lithoValidationResult = ValidarLitologias(poçoAtual.TipoPoço, litologiasSelecionadas, out var message);

                if (!lithoValidationResult)
                {
                    return ImportarArquivoOutput.ImportaçãoComFalhasDeValidação(message);
                }

                IReadOnlyCollection<RegistroEvento> registros = null; 
                if (dadosSelecionados.Contains(DadosSelecionadosEnum.Registros) || dadosSelecionados.Contains(DadosSelecionadosEnum.Eventos))
                    registros = await _registrosEventosReadOnlyRepository.ObterRegistrosEventosDeUmPoço(idPoço);

                if (dadosExtras.ContainsKey("poçoWeb"))
                {
                    var poçoWebJson = await BuscarArquivoPoçoWeb(dadosExtras["token"].ToString(), caminhoOuDadoDoArquivo);
                    dadosExtras.Add("poçoWebJson",poçoWebJson);
                    dadosExtras.Add("poço", poçoAtual);
                }

                //Ler os dados do arquivo
                var importadorArquivo = new ImportadorArquivo(dadosSelecionados, caminhoOuDadoDoArquivo, profilesToImport, lithologiesToImport, poçoAtual, dadosExtras, registros);
                var validationResult = importadorArquivo.ImportarArquivo();

                if (validationResult.result.IsValid == false)
                    return ImportarArquivoOutput.ImportaçãoComFalhasDeValidação("Não foi possível importar os dados do arquivo: " + string.Join("\n",validationResult.result.Errors));

                if (validationResult.result.IsValid && validationResult.Entity == null)
                    return ImportarArquivoOutput.ImportaçãoComFalha("Erro ao importar os dados. Verifique os logs");

                PoçoValidator validador = new PoçoValidator(_poçoReadOnlyRepository);
                var poçoValido = validador.Validate((Poço)validationResult.Entity);

                if (poçoValido.IsValid == false)
                {
                    return ImportarArquivoOutput.ImportaçãoComFalhasDeValidação("Não foi possível importar os dados do arquivo: " + string.Join("\n", poçoValido.Errors));
                }

                var poçoEntity = (Poço)validationResult.Entity;
                
                if (correçãoMesaRotativa != "")
                {
                    double.TryParse(correçãoMesaRotativa, NumberStyles.Any, CultureInfo.InvariantCulture, out var doubleCorreçãoMR);
                    poçoEntity.Shift(poçoEntity.DadosGerais.Geometria.MesaRotativa - doubleCorreçãoMR);
                }

                await _registrosEventosWriteOnlyRepository.AtualizarRegistrosEventos(poçoEntity, poçoEntity.RegistrosEventos);
                await _poçoWriteOnlyRepository.AtualizarPoço(poçoEntity);
                return ImportarArquivoOutput.ImportadoComSucesso();
            }
            catch (Exception e)
            {
                Log.Error("Erro ao importar os dados do poço " + idPoço + " . Arquivo: " + caminhoOuDadoDoArquivo + ". Erro: " + e.Message);
                return ImportarArquivoOutput.ImportaçãoComFalha(e.Message);
            }
        }

        private List<LitologiaParaImportarDTO> PreencherLitologiasParaImportarDTO(List<LitologiaModel> litologiasSelecionadas)
        {
            var list = new List<LitologiaParaImportarDTO>();

            foreach (var litologia in litologiasSelecionadas)
            {
                var litologiaParaImportar = new LitologiaParaImportarDTO
                {
                    ValorTopo = litologia.ValorTopo,
                    ValorBase = litologia.ValorBase,
                    NovoNome = litologia.NovoNome,
                    Nome = litologia.Nome,
                    Ação = litologia.Ação,
                    Tipo = litologia.Tipo
                };

                list.Add(litologiaParaImportar);
            }

            return list;
        }

        private List<PerfilParaImportarDTO> PreencherPefilParaImportarDTO(List<PerfilModel> perfisSelecionados)
        {
            var list = new List<PerfilParaImportarDTO>();

            foreach (var perfil in perfisSelecionados)
            {
                var perfilParaImportar = new PerfilParaImportarDTO
                {
                    Tipo = perfil.Tipo,
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

        private async Task<string> BuscarArquivoPoçoWeb(string token, string url)
        {
            if (url.Contains('?'))
                url += "&render=json";
            else
                url += "?render=json";

            var poçoWebString = await _poçoWebService.GetPoçoWebDTOFromWebByFile(url, token);

            return poçoWebString;
        }

        private bool ValidarLitologias(TipoPoço tipoPoço, List<LitologiaModel> litologias, out string message)
        {
            var qtdMaximaLitologias = tipoPoço == TipoPoço.Retroanalise ? 1 : 2;

            if (litologias.Count > qtdMaximaLitologias)
            {
                message = $"Este poço suporta a importação de no máximo {qtdMaximaLitologias} litologia{(qtdMaximaLitologias > 1 ? "s" : "")}.";
                return false;
            }

            if (qtdMaximaLitologias == 2 && litologias.Count > 1)
            {
                if (litologias[0].Tipo == litologias[1].Tipo)
                {
                    message = $"Não é possível importar duas litologias de tipo '{litologias[0].Tipo}'.";
                    return false;
                }
            }

            message = "";
            return true;
        }
    }
}
