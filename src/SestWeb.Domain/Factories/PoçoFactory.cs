using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.Poço.EstratigrafiaDoPoço;
using SestWeb.Domain.Entities.RegistrosEventos;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Validadores;
using SestWeb.Domain.Validadores.DTO;
using SestWeb.Domain.Entities.Poço.Objetivos;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Importadores.Shallow.Utils;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using static System.String;
using SestWeb.Domain.Helpers;
using SestWeb.Domain.SistemasUnidades.Grupos.Base;
using SestWeb.Domain.SistemasUnidades.Base;

namespace SestWeb.Domain.Factories
{
    /// <summary>
    ///     Factory para criação de poço
    /// </summary>
    public class PoçoFactory
    {
        /// <summary>
        /// Cria um poço.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nome">Nome do poço.</param>
        /// <param name="tipo">Tipo do poço.</param>
        /// <returns>Um poço.</returns>
        /// <exception cref="ArgumentException">Lançado quando <paramref name="nome" /> é inválido.</exception>
        public static Poço CriarPoço(string id, string nome, TipoPoço tipo)
        {
            if (IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome do poço inválido.");

            var poço = (Poço)Activator.CreateInstance(typeof(Poço),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
                new object[] { id, nome, tipo }, null);

            //Trajetória default de 10000m
            poço.Trajetória.GerarTrajetóriaPadrão();

            poço.RegistrosEventos = TiposPadrão.GetLista(poço.Id);

            //Inicializando listas de litologias vazias de acordo com o tipo do poço
            poço.Litologias.Clear();
            if (tipo == TipoPoço.Projeto)
            {
                poço.Litologias.Add(new Litologia(TipoLitologia.Prevista, poço.Trajetória));
                poço.Litologias.Add(new Litologia(TipoLitologia.Adaptada, poço.Trajetória));
            }
            else if (tipo == TipoPoço.Monitoramento)
            {
                poço.Litologias.Add(new Litologia(TipoLitologia.Prevista, poço.Trajetória));
                poço.Litologias.Add(new Litologia(TipoLitologia.Interpretada, poço.Trajetória));
            }
            else if (tipo == TipoPoço.Retroanalise)
            {
                poço.Litologias.Add(new Litologia(TipoLitologia.Interpretada, poço.Trajetória));
            }

            return poço;
        }

        public static Result EditarPoço(PoçoDTO dto, Poço entity, List<PerfilParaImportarDTO> profiles, List<LitologiaParaImportarDTO> lithologies, List<DadosSelecionadosEnum> dadosSelecionados, IReadOnlyCollection<RegistroEvento> registrosDoPoço)
        {
            var result = new Result();
            var existeLitologiaParaImportar = lithologies?.Count > 0;
            var existePerfisParaImportar = profiles?.Count > 0;

            var nomesPerfisExistentes = entity.Perfis.Select(p => p.Nome).ToList();

            if (existePerfisParaImportar)
            {
                // alteração devido à validação de nome de perfil existente dentro do validador.
                // o novo nome vem do front, mas o perfil vem do leitorDeep com nome antigo.
                foreach (var profile in profiles)
                {
                    if (profile.NovoNome != null)
                    {
                        var perfilDto = dto.Perfis.Find(p => p.Nome.Equals(profile.Nome));
                        if (perfilDto != null)
                        {
                            perfilDto.Nome = profile.NovoNome;
                        }
                    }
                }
            }

            var validadorDTO = new PoçoDTOValidator(nomesPerfisExistentes, dadosSelecionados, existeLitologiaParaImportar, existePerfisParaImportar);

            if (dto.Litologias != null)
                dto.Litologias = SubstituirLitologiasInvalidas(dto.Litologias);

            var validationResult = validadorDTO.Validate(dto);

            result.result = validationResult;

            if (validationResult.IsValid == false)
            {
                return result;
            }

            var mesaRotativaAntiga = entity.DadosGerais.Geometria.MesaRotativa;
            entity = PreencherPoçoEntidade(dto, entity, profiles, lithologies, dadosSelecionados, registrosDoPoço);

            if (dadosSelecionados.Contains(DadosSelecionadosEnum.MesaRotativa))
            {
                double.TryParse(dto.DadosGerais.Geometria.MesaRotativa, NumberStyles.Any, CultureInfo.InvariantCulture, out var mesaRotativaNova);
                var delta = mesaRotativaNova - mesaRotativaAntiga;

                if (!FastMath.SafeEquals(delta, 0, 0.01))
                {
                    entity.Shift(delta);
                }
            }

            if (entity.DadosGerais.Geometria.OffShore != null
                && entity.DadosGerais.Geometria.OffShore.LaminaDagua > 0)
            {
                var baseSedimentos = entity.DadosGerais.Geometria.OffShore.LaminaDagua + entity.DadosGerais.Geometria.MesaRotativa;
                entity.Estratigrafia.AtualizarEstratigrafiaComBaseSedimentos(entity.Trajetória, baseSedimentos);
                AtualizarLitologiaComLaminaDAgua(entity);
            }
            else if (entity.DadosGerais.Geometria.OnShore != null)
            {
                var baseSedimentos = entity.DadosGerais.Geometria.OnShore.AlturaDeAntePoço + entity.DadosGerais.Geometria.MesaRotativa;
                entity.Estratigrafia.AtualizarEstratigrafiaComBaseSedimentos(entity.Trajetória, baseSedimentos);
            }

            //Tratamento especial para o primeiro ponto do perfil DIAM_BROCA
            AtualizaPrimeiroPontoDoPerfilDiametroDeBroca(entity);

            var validatorEntity = new PoçoValidator();
            var validationResultEntity = validatorEntity.Validate(entity);
            result.result = validationResultEntity;

            if (validationResultEntity.IsValid == false)
            {
                return result;
            }

            //Tratamento de overlapping de estratigrafia quando há alguma mudança em estratigrafia
            if (dto.Estratigrafia != null)
                entity.Estratigrafia.TratarOverLappingDoMesmoTipo();

            result.Entity = entity;
            return result;
        }

        private static List<LitologiaDTO> SubstituirLitologiasInvalidas(List<LitologiaDTO> listLito)
        {
            foreach (var lito in listLito)
            {
                if (lito != null)
                {
                    var tiposDeRocha = lito.Pontos.Select(s => s.TipoRocha).ToHashSet();
                    foreach (var rocha in tiposDeRocha)
                    {
                        var tipoRochaEntity = TipoRocha.FromMnemonico(rocha);

                        if (tipoRochaEntity == null)
                            tipoRochaEntity = TipoRocha.FromNome(rocha);

                        if (tipoRochaEntity == null && int.TryParse(rocha, out _))
                            tipoRochaEntity = TipoRocha.FromNumero(int.Parse(rocha));

                        if (tipoRochaEntity == null)
                            lito.Pontos.Where(r => r.TipoRocha == rocha).ToList().ForEach(s => s.TipoRochaMnemonico = "OUT");
                        else
                            lito.Pontos.Where(r => r.TipoRocha == rocha).ToList().ForEach(s => s.TipoRochaMnemonico = tipoRochaEntity.Mnemonico);
                    }
                }
            }

            return listLito;
        }

        private static Poço PreencherPoçoEntidade(PoçoDTO dto, Poço entity, List<PerfilParaImportarDTO> perfisImportados = null, List<LitologiaParaImportarDTO> litologiasImportadas = null, List<DadosSelecionadosEnum> dadosSelecionados = null, IReadOnlyCollection<RegistroEvento> registrosDoPoço = null)
        {
            PreencherDadosGerais(dto.DadosGerais, entity.DadosGerais, dadosSelecionados);
            PreencherTrajetória(dto.Trajetória, entity);

            if (litologiasImportadas != null)
                PreencherLitologia(dto.Litologias, entity, litologiasImportadas);
            else if (dto.Litologias != null && dto.Litologias.Any())
                SubstituirLitologiasDoPoço(dto.Litologias, entity);

            PreencherEstratigrafias(entity.Trajetória, dto.Estratigrafia, entity.Estratigrafia);

            if (perfisImportados != null)
                PreencherPerfis(dto.Perfis, entity, perfisImportados);
            else if (dto.Perfis != null && dto.Perfis.Any())
                SubstituirPerfisDoPoço(dto.Perfis, entity);

            PreencherSapatas(dto.Sapatas, entity);
            PreencherObjetivos(dto.Objetivos, entity);
            PreencherRegistrosDePerfuração(dto.Registros, entity, registrosDoPoço);
            PreencherEventos(dto.Eventos, entity, registrosDoPoço);

            return entity;
        }

        private static void PreencherDadosGerais(DadosGeraisDTO dadosGeraisDto, DadosGerais dadosGerais, List<DadosSelecionadosEnum> dadosSelecionados)
        {
            if (!dadosSelecionados.Contains(DadosSelecionadosEnum.DadosGerais))
                return;

            #region Identificação

            if (dadosGeraisDto.Identificação != null)
            {
                dadosGerais.Identificação.NomePoço = CarregarNomeDoPoço(dadosGerais.Identificação.NomePoço, dadosGerais.Identificação.NomePoçoLocalImportação, dadosGeraisDto.Identificação.NomePoço,dadosGeraisDto.Identificação.NomePoçoLocalImportação);
                dadosGerais.Identificação.Bacia = dadosGeraisDto.Identificação.Bacia;
                dadosGerais.Identificação.Campo = dadosGeraisDto.Identificação.Campo;
                dadosGerais.Identificação.Companhia = dadosGeraisDto.Identificação.Companhia;
                dadosGerais.Identificação.Sonda = dadosGeraisDto.Identificação.Sonda;
                dadosGerais.Identificação.Finalidade = dadosGeraisDto.Identificação.Finalidade;
                dadosGerais.Identificação.Analista = dadosGeraisDto.Identificação.Analista;
                dadosGerais.Identificação.NívelProteção = dadosGeraisDto.Identificação.NívelProteção;
                dadosGerais.Identificação.ClassificaçãoPoço = dadosGeraisDto.Identificação.ClassificaçãoPoço;
                dadosGerais.Identificação.TipoCompletação = dadosGeraisDto.Identificação.TipoCompletação;
                dadosGerais.Identificação.PoçoWebUrl = dadosGeraisDto.Identificação.PoçoWebUrl;
                dadosGerais.Identificação.PoçoWebDtÚltimaAtualização = dadosGeraisDto.Identificação.PoçoWebDtÚltimaAtualização;
                dadosGerais.Identificação.PoçoWebUrlRevisões = dadosGeraisDto.Identificação.PoçoWebUrlRevisões;
                dadosGerais.Identificação.PoçoWebRevisãoUrl = dadosGeraisDto.Identificação.PoçoWebRevisãoUrl;
                dadosGerais.Identificação.CriticidadePoço = dadosGeraisDto.Identificação.CriticidadePoço;
                dadosGerais.Identificação.IntervençãoWorkover = dadosGeraisDto.Identificação.IntervençãoWorkover;
                dadosGerais.Identificação.NomePoçoWeb = dadosGeraisDto.Identificação.NomePoçoWeb;
                dadosGerais.Identificação.NomePoçoLocalImportação = CarregarLocalImportação(dadosGerais.Identificação.NomePoçoLocalImportação, dadosGeraisDto.Identificação.NomePoçoLocalImportação);

                if (double.TryParse(dadosGeraisDto.Identificação.ComplexidadePoço, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out _))
                    dadosGerais.Identificação.ComplexidadePoço =
                        StringUtils.ToDoubleInvariantCulture(dadosGeraisDto.Identificação.ComplexidadePoço, 2);

                if (double.TryParse(dadosGeraisDto.Identificação.VidaÚtilPrevista, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out _))
                    dadosGerais.Identificação.VidaÚtilPrevista =
                        StringUtils.ToDoubleInvariantCulture(dadosGeraisDto.Identificação.VidaÚtilPrevista, 2);

                if (dadosGeraisDto.Identificação.TipoPoço != null)
                    dadosGerais.Identificação.TipoPoço =
                        (TipoPoço)Enum.Parse(typeof(TipoPoço), dadosGeraisDto.Identificação.TipoPoço);
            }

            #endregion

            #region Area

            if (dadosGeraisDto.Area != null)
            {
                if (double.TryParse(dadosGeraisDto.Area.DensidadeAguaMar, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out _))
                    dadosGerais.Area.DensidadeAguaMar =
                        StringUtils.ToDoubleInvariantCulture(dadosGeraisDto.Area.DensidadeAguaMar, 2);

                if (double.TryParse(dadosGeraisDto.Area.DensidadeSuperficie, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out _))
                    dadosGerais.Area.DensidadeSuperficie =
                        StringUtils.ToDoubleInvariantCulture(dadosGeraisDto.Area.DensidadeSuperficie, 2);

                if (double.TryParse(dadosGeraisDto.Area.SonicoSuperficie, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out _))
                    dadosGerais.Area.SonicoSuperficie =
                        StringUtils.ToDoubleInvariantCulture(dadosGeraisDto.Area.SonicoSuperficie, 2);

                if (double.TryParse(dadosGeraisDto.Area.DTSSuperficie, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out _))
                    dadosGerais.Area.DTSSuperficie =
                        StringUtils.ToDoubleInvariantCulture(dadosGeraisDto.Area.DTSSuperficie, 2);
            }

            #endregion

            #region Geometria

            if (dadosGeraisDto.Geometria != null)
            {
                if (double.TryParse(dadosGeraisDto.Geometria.Coordenadas.UtMx, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out _))
                    dadosGerais.Geometria.Coordenadas.UtMx =
                        StringUtils.ToDoubleInvariantCulture(dadosGeraisDto.Geometria.Coordenadas.UtMx, 2);

                if (double.TryParse(dadosGeraisDto.Geometria.Coordenadas.UtMy, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out _))
                    dadosGerais.Geometria.Coordenadas.UtMy =
                        StringUtils.ToDoubleInvariantCulture(dadosGeraisDto.Geometria.Coordenadas.UtMy, 2);

                if (double.TryParse(dadosGeraisDto.Geometria.MesaRotativa, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out _))
                    dadosGerais.Geometria.MesaRotativa =
                        StringUtils.ToDoubleInvariantCulture(dadosGeraisDto.Geometria.MesaRotativa, 2);

                if (double.TryParse(dadosGeraisDto.Geometria.OffShore.LaminaDagua, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out _))
                    dadosGerais.Geometria.OffShore.LaminaDagua =
                        StringUtils.ToDoubleInvariantCulture(dadosGeraisDto.Geometria.OffShore.LaminaDagua, 2);

                if (double.TryParse(dadosGeraisDto.Geometria.OnShore.AlturaDeAntePoço, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out _))
                    dadosGerais.Geometria.OnShore.AlturaDeAntePoço =
                        StringUtils.ToDoubleInvariantCulture(dadosGeraisDto.Geometria.OnShore.AlturaDeAntePoço, 2);

                if (double.TryParse(dadosGeraisDto.Geometria.OnShore.Elevação, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out _))
                    dadosGerais.Geometria.OnShore.Elevação =
                        StringUtils.ToDoubleInvariantCulture(dadosGeraisDto.Geometria.OnShore.Elevação, 2);

                if (double.TryParse(dadosGeraisDto.Geometria.OnShore.LençolFreático, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out _))
                    dadosGerais.Geometria.OnShore.LençolFreático =
                        StringUtils.ToDoubleInvariantCulture(dadosGeraisDto.Geometria.OnShore.LençolFreático, 2);

                dadosGerais.Geometria.AjusteDeMesaRotativaComElevação(dadosGeraisDto.Geometria
                    .AtualizaMesaRotativaComElevação);
            }

            #endregion

        }

        private static string CarregarLocalImportação(string nomePoçoLocalImportaçãoAtual, string nomePoçoLocalImportaçãoDTO)
        {
            var listaPrioridade = new List<string> { "SIGEO", "LAS", "SEST" };

            if (IsNullOrEmpty(nomePoçoLocalImportaçãoDTO))
                return nomePoçoLocalImportaçãoAtual;

            var indexAtual = listaPrioridade.IndexOf(nomePoçoLocalImportaçãoAtual);
            var indexDTO = listaPrioridade.IndexOf(nomePoçoLocalImportaçãoDTO);

            if (indexAtual < indexDTO && indexAtual > 0)
                return nomePoçoLocalImportaçãoAtual;
            else
                return nomePoçoLocalImportaçãoDTO;
        }

        private static string CarregarNomeDoPoço(string nomePoçoAtual, string nomePoçoLocalImportaçãoAtual, string nomePoçoDTO, string nomePoçoLocalImportaçãoDTO)
        {
            var listaPrioridade = new List<string> {"SIGEO", "LAS", "SEST" };

            if (IsNullOrEmpty(nomePoçoLocalImportaçãoDTO))
                return nomePoçoAtual;

            var indexAtual = listaPrioridade.IndexOf(nomePoçoLocalImportaçãoAtual);
            var indexDTO = listaPrioridade.IndexOf(nomePoçoLocalImportaçãoDTO);

            if (indexAtual < indexDTO && indexAtual > 0)
                return nomePoçoAtual;
            else
                return nomePoçoDTO;
        }

        private static void SubstituirLitologiasDoPoço(IEnumerable<LitologiaDTO> litologiasDto, Poço entity)
        {
            var litologias = entity.Litologias;

            foreach (var litoDto in litologiasDto)
            {
                var litoEntity = litologias.Find(l => String.Equals(l.Classificação.Nome, litoDto.Classificação, StringComparison.CurrentCultureIgnoreCase));

                if (litoEntity == null)
                {
                    throw new Exception($"Nenhuma litologia de tipo {litoDto.Classificação} foi encontrada. Certifique-se de que o caso aberto é do tipo correto.");
                }

                //litoEntity.Nome = litoEntity.Classificação.Nome;

                litoEntity.Clear();

                foreach (var pontoDto in litoDto.Pontos)
                {
                    litoEntity.AddPontoEmPm(litoEntity.ConversorProfundidade, StringUtils.ToDoubleInvariantCulture(pontoDto.Pm, 2), pontoDto.TipoRochaMnemonico, TipoProfundidade.PM, OrigemPonto.Importado);
                }

                var index = litologias.IndexOf(litoEntity);
                litologias[index] = litoEntity;
            }
        }

        private static void PreencherLitologia(IEnumerable<LitologiaDTO> litologiasDto, Poço entity, List<LitologiaParaImportarDTO> lstLitologiasImportadas = null)
        {
            if (litologiasDto == null || !litologiasDto.Any())
                return;

            var tipoCaso = entity.DadosGerais.Identificação.TipoPoço;

            if (lstLitologiasImportadas.Count() > 2)
            {
                throw new Exception("Só é possível importar, no máximo, duas litologias.");
            }

            if (tipoCaso == TipoPoço.Projeto)
            {
                if (lstLitologiasImportadas.Count() == 2)
                {
                    if (lstLitologiasImportadas[0].Tipo == lstLitologiasImportadas[1].Tipo)
                    {
                        throw new Exception($"Ambas as litologias são do mesmo tipo ({lstLitologiasImportadas[0].Tipo.ToLower()}). É necessário que uma seja prevista, e a outra adaptada.");
                    }
                }
            }
            else if (tipoCaso == TipoPoço.Monitoramento)
            {
                if (lstLitologiasImportadas.Count() == 2)
                {
                    if (lstLitologiasImportadas[0].Tipo == lstLitologiasImportadas[1].Tipo)
                    {
                        throw new Exception($"Ambas as litologias são do mesmo tipo ({lstLitologiasImportadas[0].Tipo.ToLower()}). É necessário que uma seja prevista, e a outra interpretada.");
                    }
                }
            }
            else if (tipoCaso == TipoPoço.Retroanalise)
            {
                if (lstLitologiasImportadas.Count() == 2)
                {
                    throw new Exception("Só é possível importar uma litologia em casos de retroanálise.");
                }
            }
            else
            {
                throw new Exception("O caso aberto tem um tipo desconhecido. Consulte os logs.");
            }

            foreach (var litologiaImportada in lstLitologiasImportadas)
            {
                var litoDto = litologiasDto.FirstOrDefault(l => String.Equals(l.Nome, litologiaImportada.Nome,
                    StringComparison.CurrentCultureIgnoreCase));

                if (litoDto == null)
                    litoDto = litologiasDto.FirstOrDefault(l => String.Equals(l.Classificação, litologiaImportada.Nome,
                    StringComparison.CurrentCultureIgnoreCase));

                if (litoDto == null)
                    throw new Exception("Erro no preenchimento da litologia. Não foi encontrada litologia importado. Por favor, verifique os logs.");

                litoDto.Classificação = litologiaImportada.Tipo != null ? litologiaImportada.Tipo : litoDto.Classificação;

                if (litologiaImportada.Ação == TipoDeAção.Novo)
                {
                    CriarLitologia(litoDto, entity, litologiaImportada);
                }
                else if (litologiaImportada.Ação == TipoDeAção.Sobrescrever)
                {
                    double inicioSedimentos = entity.ObterBaseDeSedimentos();

                    SobrescreverLitologia(litoDto, entity.Litologias, litologiaImportada.NovoNome, litologiaImportada.ValorTopo, litologiaImportada.ValorBase, inicioSedimentos, entity.Trajetória);
                }
                else if (litologiaImportada.Ação == TipoDeAção.Acrescentar)
                {
                    AcrescentarLitologia(litoDto, entity.Litologias, entity.Trajetória, entity);
                }

                var litoEntity = entity.Litologias.Find(l => string.Equals(l.Classificação.Nome, litoDto.Classificação, StringComparison.CurrentCultureIgnoreCase));
                litoEntity?.CompletarTrechosSenoidais(entity.Trajetória);
            }
        }

        private static void PreencherEstratigrafias(Trajetória trajetória, EstratigrafiaDTO estratigrafiaDto, Estratigrafia estratigrafia)
        {
            if (estratigrafiaDto == null)
                return;

            estratigrafia.ApagarEstratigrafia();
            foreach (var estrat in estratigrafiaDto.Itens)
            {
                foreach (var item in estrat.Value)
                {
                    estratigrafia.CriarItemEstratigrafia(trajetória, TipoEstratigrafia.ObterStringPeloTipo(estrat.Key),
                        new Profundidade(StringUtils.ToDoubleInvariantCulture(item.Pm, 2)),
                        item.Sigla, item.Descrição, item.Idade);
                }
            }

            estratigrafia.Reset();
        }

        private static void SubstituirPerfisDoPoço(IReadOnlyCollection<PerfilDTO> perfisDto, Poço poço)
        {

            poço.Perfis.Clear();

            Parallel.ForEach(perfisDto, perfilDto =>
            {
                double profundidadeSedimentos = poço.ObterBaseDeSedimentos();

                if (perfilDto.Mnemonico == nameof(DIAM_BROCA))
                {
                    AtualizaPrimeiroPontoDoPerfilDTODiametroDeBroca(perfilDto, profundidadeSedimentos);
                }

                perfilDto.PontosDTO = RemoverPontosAcimaDaProfundidadeDeSedimentos(profundidadeSedimentos, perfilDto.PontosDTO, perfilDto.TipoProfundidade, poço.DadosGerais.Geometria.MesaRotativa);

                var litologiaPoço = poço.Litologias.FirstOrDefault(x => x.Classificação == TipoLitologia.Adaptada);

                if (litologiaPoço == null)
                    litologiaPoço = poço.Litologias.FirstOrDefault(x => x.Classificação == TipoLitologia.Prevista);

                if (litologiaPoço == null)
                    litologiaPoço = poço.Litologias.FirstOrDefault(x => x.Classificação == TipoLitologia.Interpretada);

                if (litologiaPoço == null)
                    litologiaPoço = new Litologia(TipoLitologia.Adaptada, poço.Trajetória);

                var perfil = PerfisFactory.Create(perfilDto.Mnemonico, perfilDto.Nome, poço.Trajetória, litologiaPoço);

                var unidade = UnidadeMedida.ObterPorSímbolo(perfilDto.Unidade);
                var grupo = GrupoUnidades.GetGrupoUnidades(perfil.Mnemonico);

                foreach (var pontoDto in perfilDto.PontosDTO)
                {
                    var valor = StringUtils.ToDoubleInvariantCulture(pontoDto.Valor, 2);
                    var pm = StringUtils.ToDoubleInvariantCulture(pontoDto.Pm, 2);

                    if (valor < 0)
                    {
                        continue;
                    }

                    perfil.AddPontoEmPm(poço.Trajetória, pm, grupo.ConvertToUnidadePadrão(unidade, valor), TipoProfundidade.PM, pontoDto.Origem);
                }
                poço.Perfis.Add(perfil);
            });
        }

        private static void PreencherPerfis(IReadOnlyCollection<PerfilDTO> perfisDto, Poço entity, List<PerfilParaImportarDTO> lstPerfisImportados = null)
        {
            if (perfisDto == null || !perfisDto.Any())
                return;

            foreach(var perfilImportado in lstPerfisImportados)
            {
                var nomePerfil = perfilImportado.NovoNome != null ? perfilImportado.NovoNome : perfilImportado.Nome;
                var perfilDto = perfisDto.FirstOrDefault(p => string.Equals(p.Nome, nomePerfil,
                    StringComparison.CurrentCulture));

                if (perfilDto == null)
                    throw new Exception($"Erro no preenchimento do perfil. Não foi encontrado perfil importado {perfilImportado.Nome}. Por favor, verifique os logs.");

                perfilDto.Nome = IsNullOrWhiteSpace(perfilImportado.NovoNome) ? perfilDto.Nome : perfilImportado.NovoNome;

                double profundidadeSedimentos = entity.ObterBaseDeSedimentos();

                if (perfilDto.Mnemonico == "DIAM_BROCA")
                {
                    AtualizaPrimeiroPontoDoPerfilDTODiametroDeBroca(perfilDto, profundidadeSedimentos);
                }

                perfilDto.PontosDTO = RemoverPontosAcimaDaProfundidadeDeSedimentos(profundidadeSedimentos, perfilDto.PontosDTO, perfilDto.TipoProfundidade, entity.DadosGerais.Geometria.MesaRotativa);

                if (perfilImportado.Ação == TipoDeAção.Novo)
                {
                    CriarPerfil(perfilDto, entity);
                }
                else if (perfilImportado.Ação == TipoDeAção.Sobrescrever)
                {
                    SobrescreverPerfil(perfilDto, entity, perfilImportado.Nome, perfilImportado.ValorTopo, perfilImportado.ValorBase);
                }
                else if (perfilImportado.Ação == TipoDeAção.Acrescentar)
                {
                    AcrescentarPerfil(perfilDto, entity, perfilImportado.Nome);
                }
            }
        }

        private static void PreencherSapatas(IReadOnlyCollection<SapataDTO> sapatasDto, Poço entity)
        {
            if (sapatasDto == null)
                return;

            entity.Sapatas.Clear();
            var sapataFactory = entity.ObterSapataFactory();
            foreach (var dto in sapatasDto)
            {
                var sapata = sapataFactory.CriarSapata(StringUtils.ToDoubleInvariantCulture(dto.Pm, 2), dto.Diâmetro);
                entity.Sapatas.Add(sapata);
            }
        }

        private static void PreencherObjetivos(IReadOnlyCollection<ObjetivoDTO> objetivosDto, Poço entity)
        {
            if (objetivosDto == null)
                return;

            entity.Objetivos.Clear();
            var objetivoFactory = entity.ObterObjetivoFactory();
            foreach (var dto in objetivosDto)
            {
                var objetivo = objetivoFactory.CriarObjetivo(StringUtils.ToDoubleInvariantCulture(dto.Pm, 2), (TipoObjetivo)Enum.Parse(typeof(TipoObjetivo), dto.TipoObjetivo));
                entity.Objetivos.Add(objetivo);
            }
        }

        private static void PreencherEventos(IReadOnlyCollection<RegistroDTO> eventosDto, Poço entity, IReadOnlyCollection<RegistroEvento> registrosDoPoço)
        {
            if (eventosDto == null || !eventosDto.Any())
                return;

            foreach (var dto in eventosDto)
            {
                var registro = registrosDoPoço.FirstOrDefault(s => s.Nome.ToLower() == dto.Tipo.ToLower());
                if (registro != null)
                {
                    registro.ResetTrechos();
                    if (dto.Pontos.Count > 0)
                    {
                        for (int i = 0; i < dto.Pontos.Count - 1; i++)
                        {
                            var pm = StringUtils.ToDoubleInvariantCulture(dto.Pontos[i].Pm, 2);
                            var calculouPv = entity.Trajetória.TryGetTVDFromMD(pm, out double pv);
                            registro.AddTrecho(pm, pm, pv, pv, dto.Pontos[i].Comentário, entity.Trajetória);
                        }

                    }
                }
            }

            entity.RegistrosEventos = registrosDoPoço.ToList();
        }

        private static void PreencherRegistrosDePerfuração(IReadOnlyCollection<RegistroDTO> registrosDto, Poço entity, IReadOnlyCollection<RegistroEvento> registrosDoPoço)
        {
            if (registrosDto == null || !registrosDto.Any())
                return;

            foreach (var dto in registrosDto)
            {
                if (dto.Tipo.ToLower() == "gás" || dto.Tipo.ToLower() == "gas")
                    dto.Tipo = "Gás de formação";

                if (dto.Tipo.ToLower() == "registrosdepressãodeporos" || dto.Tipo.ToLower() == "registrosdepressaodeporos")
                    dto.Tipo = "Pressão de Poros";

                if (dto.Tipo.ToLower() == "pontostrabalhados")
                    dto.Tipo = "Pontos trabalhados";


                var registro = registrosDoPoço.FirstOrDefault(s => s.Nome == dto.Tipo);

                if (registro != null)
                {
                    registro.SetUnidade(dto.Unidade);

                    registro.ResetTrechos();
                    if (dto.Pontos.Count > 0)
                    {
                        for (int i = 0; i < dto.Pontos.Count; i++)
                        {
                            var pm = 0.0;
                            var pv = 0.0;
                            if (dto.Pontos[i].PmTopo != null)
                            {
                                var pmTopo = StringUtils.ToDoubleInvariantCulture(dto.Pontos[i].PmTopo, 2);
                                var pmBase = StringUtils.ToDoubleInvariantCulture(dto.Pontos[i].PmBase, 2);
                                var calculouPvTopo = entity.Trajetória.TryGetTVDFromMD(pmTopo, out double pvTopoCalculado);
                                var calculouPvBase = entity.Trajetória.TryGetTVDFromMD(pmTopo, out double pvBaseCalculado);

                                registro.AddTrecho(pmTopo, pmBase, pvTopoCalculado, pvBaseCalculado, dto.Pontos[i].Comentário, entity.Trajetória);
                            }
                            else if (dto.Pontos[i].Pm == null)
                            {
                                pv = StringUtils.ToDoubleInvariantCulture(dto.Pontos[i].Pv, 2);
                                var calculouPm = entity.Trajetória.TryGetMDFromTVD(pv, out double pmCalculado);
                                pm = pmCalculado;

                                if (registro.Tipo == TipoRegistroEvento.Evento)
                                    registro.AddTrecho(pm, pm, pv, pv, dto.Pontos[i].Comentário, entity.Trajetória);
                                else
                                {
                                    var valor = StringUtils.ToDoubleInvariantCulture(dto.Pontos[i].Valor, 2);
                                    if (dto.Unidade == "psi")
                                    {
                                        valor = UnitConverter.PsiToPpg(valor, pv);
                                        registro.SetUnidade("lb/gal");
                                    }
                                    registro.AddTrecho(pm, pv, valor, null, entity.Trajetória);
                                }
                            }
                            else
                            {
                                pm = StringUtils.ToDoubleInvariantCulture(dto.Pontos[i].Pm, 2);
                                var calculouPv = entity.Trajetória.TryGetTVDFromMD(pm, out double pvCalculado);
                                pv = pvCalculado;

                                if (registro.Tipo == TipoRegistroEvento.Evento)
                                    registro.AddTrecho(pm, pm, pv, pv, dto.Pontos[i].Comentário, entity.Trajetória);
                                else
                                {
                                    var valor = StringUtils.ToDoubleInvariantCulture(dto.Pontos[i].Valor, 2);
                                    if (dto.Unidade == "psi")
                                    {
                                        valor = UnitConverter.PsiToPpg(valor, pv);
                                        registro.SetUnidade("lb/gal");
                                    }
                                    registro.AddTrecho(pm, pv, valor, null, entity.Trajetória);
                                }
                                    
                            }

                        }

                    }
                }
            }

            entity.RegistrosEventos = registrosDoPoço.ToList();

        }

        private static void PreencherTrajetória(TrajetóriaDTO trajetóriaDto, Poço entity)
        {
            if (trajetóriaDto == null || !trajetóriaDto.Pontos.Any())
                return;

            if (trajetóriaDto.TipoDeAção == null || (trajetóriaDto.TipoDeAção != null && trajetóriaDto.TipoDeAção == TipoDeAção.Novo))
                CriarTrajetória(trajetóriaDto, entity);
            else if (trajetóriaDto.TipoDeAção != null && trajetóriaDto.TipoDeAção == TipoDeAção.Acrescentar)
                AcrescentarTrajetória(trajetóriaDto, entity);
            else if (trajetóriaDto.TipoDeAção != null && trajetóriaDto.TipoDeAção == TipoDeAção.Sobrescrever)
                SobrescreverTrajetória(trajetóriaDto, entity);
        }

        #region Métodos auxiliares

        private static void AcrescentarLitologia(LitologiaDTO litoDto, List<Litologia> litologias, IConversorProfundidade conversorProfundidade, Poço entity)
        {
            if (litoDto == null)
                throw new Exception("Erro no preenchimento da litologia. Litologia importada não encontrada. Por favor, verifique os logs.");

            var litoEntity = litologias.Find(l => String.Equals(l.Classificação.Nome, litoDto.Nome, StringComparison.CurrentCultureIgnoreCase));

            if (litoEntity == null)
                litoEntity = litologias.Find(l => String.Equals(l.Classificação.Nome, litoDto.Classificação, StringComparison.CurrentCultureIgnoreCase));

            if (litoEntity == null)
                throw new Exception("Erro no preenchimento da litologia. Litologia não encontrada. Por favor, verifique os logs.");

            if (litoEntity.ContémPontos() == false)
                throw new Exception("Não há pontos na litologia para acrescentar novos.");

            var ultimoPontoLitologia = litoEntity.UltimoPonto;

            if (StringUtils.ToDoubleInvariantCulture(litoDto.Pontos.First().Pm, 2) < ultimoPontoLitologia.Pm.Valor)
                throw new Exception("Erro no preenchimento da litologia. Primeiro ponto é inferior à litologia existente.");

            foreach (var pontoDto in litoDto.Pontos)
            {
                if (litoDto.TipoProfundidade == "Cota")
                {
                    var cota = StringUtils.ToDoubleInvariantCulture(pontoDto.Pm, 2);
                    var pv = entity.DadosGerais.Geometria.MesaRotativa + cota;
                    litoEntity.AddPontoEmPv(entity.Trajetória, pv, pontoDto.TipoRochaMnemonico, TipoProfundidade.PM, OrigemPonto.Importado);
                }
                else
                {
                    litoEntity.AddPontoEmPm(entity.Trajetória, StringUtils.ToDoubleInvariantCulture(pontoDto.Pm, 2), pontoDto.TipoRochaMnemonico, TipoProfundidade.PM, OrigemPonto.Importado);
                }
            }

            var index = litologias.IndexOf(litoEntity);
            litologias[index] = litoEntity;
        }

        private static void SobrescreverLitologia(LitologiaDTO litoDto, List<Litologia> litologias, string novoNome, double? pmTopo, double? pmBase, double inicioSedimentos, IConversorProfundidade trajetória)
        {
            if (pmBase.HasValue == false || pmTopo.HasValue == false)
                throw new Exception("Erro ao sobrescrever litologia. Valor topo ou valor base não informados");

            if (litoDto == null)
                throw new Exception("Erro no preenchimento da litologia. Por favor, verifique os logs.");

            var litoEntity = litologias.Find(l => String.Equals(l.Classificação.Nome, litoDto.Classificação, StringComparison.CurrentCultureIgnoreCase));

            if (litoEntity == null)
                throw new Exception("Erro no preenchimento da litologia. Por favor, verifique os logs.");

            if (litoEntity.ContémPontos() && litoEntity.PmBase.Valor < pmBase.Value)
                throw new Exception("Erro para sobrescrever litologia. Valor base fora do intervalo da litologia.");

            if (pmTopo < inicioSedimentos)
                throw new Exception("Erro para sobrescrever litologia. Valor topo fora da base de sedimentos.");


            var listaPms = new List<Profundidade>();
            var listaTipoRocha = new List<string>();

            foreach (var pontoDto in litoDto.Pontos)
            {
                listaPms.Add(new Profundidade(StringUtils.ToDoubleInvariantCulture(pontoDto.Pm, 2)));
                listaTipoRocha.Add(pontoDto.TipoRochaMnemonico);
            }

            var index = litologias.IndexOf(litoEntity);

            litoEntity.SobrescreverPontosEmPm(trajetória, new Profundidade(pmTopo.Value), new Profundidade(pmBase.Value), listaPms, listaTipoRocha, OrigemPonto.Importado);

            litologias[index] = litoEntity;
        }

        private static void CriarLitologia(LitologiaDTO litoDto, Poço entity, LitologiaParaImportarDTO litoParaImportar)
        {
            if (litoDto == null)
                throw new Exception("Erro no preenchimento da litologia. Por favor, verifique os logs.");

            var litologias = entity.Litologias;
            var litoEntity = litologias.Find(l => String.Equals(l.Classificação.Nome, litoDto.Classificação, StringComparison.CurrentCultureIgnoreCase));

            if (litoEntity == null)
            {
                throw new Exception($"Nenhuma litologia de tipo {litoDto.Classificação} foi encontrada. Certifique-se de que o caso aberto é do tipo correto.");
            }

            if (litoEntity.ContémPontos())
            {
                throw new Exception($"A litologia {litoDto.Classificação.ToLower()} já existe. Para substituir, use o modo 'Sobrescrever'.");
            }

            litoEntity.Clear();


            for (int i = 0; i < litoDto.Pontos.Count; i++)
            {
                if (i > 0 && litoDto.Pontos[i - 1].TipoRochaMnemonico == litoDto.Pontos[i].TipoRochaMnemonico && i < litoDto.Pontos.Count - 1)
                    continue;

                //ignorar ponto em pm 0
                if (litoDto.Pontos[i].Pm == "0.00") continue;

                if (litoDto.TipoProfundidade == "Cota")
                {
                    var cota = StringUtils.ToDoubleInvariantCulture(litoDto.Pontos[i].Pm, 7);
                    var pv = entity.DadosGerais.Geometria.MesaRotativa + cota;
                    litoEntity.AddPontoEmPv(entity.Trajetória, pv, litoDto.Pontos[i].TipoRochaMnemonico, TipoProfundidade.PM, OrigemPonto.Importado);
                }
                else
                {
                    //tratamento para, se o último ponto da litologia for maior que o último ponto da trajetória
                    //eu nao descarto o ponto e coloco o valor do último Pm da trajetória
                    if (i == litoDto.Pontos.Count -1 && StringUtils.ToDoubleInvariantCulture(litoDto.Pontos[i].Pm,2) > entity.Trajetória.Pontos.Last().Pm.Valor)                       
                        litoEntity.AddPontoEmPm(entity.Trajetória, entity.Trajetória.Pontos.Last().Pm.Valor, litoDto.Pontos[i].TipoRochaMnemonico, TipoProfundidade.PM, OrigemPonto.Importado);
                    else
                        litoEntity.AddPontoEmPm(entity.Trajetória, StringUtils.ToDoubleInvariantCulture(litoDto.Pontos[i].Pm, 2), litoDto.Pontos[i].TipoRochaMnemonico, TipoProfundidade.PM, OrigemPonto.Importado);
                }


            }

            var index = litologias.IndexOf(litoEntity);
            litologias[index] = litoEntity;
        }

        private static void AcrescentarPerfil(PerfilDTO perfilDto, Poço poço, string nomePerfil)
        {
            var perfil = poço.Perfis.First(p => String.Equals(p.Nome, nomePerfil, StringComparison.CurrentCultureIgnoreCase));

            var unidade = UnidadeMedida.ObterPorSímbolo(perfilDto.Unidade);
            var grupo = GrupoUnidades.GetGrupoUnidades(perfil.Mnemonico);


            foreach (var pontoDto in perfilDto.PontosDTO)
            {
                var pm = StringUtils.ToDoubleInvariantCulture(pontoDto.Pm, 2);
                var valor = StringUtils.ToDoubleInvariantCulture(pontoDto.Valor, 2);

                if (valor < 0 || pm < 0)
                {
                    continue;
                }

                perfil.AddPontoEmPm(poço.Trajetória, pm, grupo.ConvertToUnidadePadrão(unidade, valor), TipoProfundidade.PM, pontoDto.Origem);
            }
        }

        private static void SobrescreverPerfil(PerfilDTO perfilDto, Poço poço, string nomePerfil, double? pmTopo, double? pmBase) //TODO(Gabriel Pinheiro): Verificar a corretude das mudanças.
        {
            if (pmTopo == null || pmBase == null)
            {
                throw new Exception($"É necessário informar o topo e a base do perfil '{perfilDto.Nome}' a ser sobrescrito.");
            }

            // FirstOrDefault carrega a entidate com null caso não encontre. O Find é mais eficiente.
            //var perfil = entity.Perfis.FirstOrDefault(p => String.Equals(p.Nome, nomePerfil, StringComparison.CurrentCultureIgnoreCase)); 
            var perfil = poço.Perfis.Find(p =>
                string.Equals(p.Nome, nomePerfil, StringComparison.CurrentCultureIgnoreCase));

            if (perfil == null)
                throw new ArgumentException($"Não existe um perfil com nome: {nomePerfil} no poço: {poço.Nome}");

            var valores = new List<double>();
            var pms = new List<Profundidade>();

            foreach (var pontoDTO in perfilDto.PontosDTO)
            {
                var valor = StringUtils.ToDoubleInvariantCulture(pontoDTO.Valor, 2);
                var pm = StringUtils.ToDoubleInvariantCulture(pontoDTO.Valor, 2);

                if (valor < 0 || pm < 0)
                {
                    continue;
                }

                valores.Add(valor);
                pms.Add(new Profundidade(pm));
            }

            // verificar se a origem é a mesma para todos os pontos
            perfil.SobrescreverPontosEmPm(poço.Trajetória, new Profundidade((double)pmTopo), new Profundidade((double)pmBase), pms, valores, perfilDto.PontosDTO[0].Origem);
        }

        private static void CriarPerfil(PerfilDTO perfilDto, Poço poço)
        {
            var litologiaPoço = poço.Litologias.FirstOrDefault(x => x.Classificação == TipoLitologia.Adaptada);

            if (litologiaPoço == null)
                litologiaPoço = poço.Litologias.FirstOrDefault(x => x.Classificação == TipoLitologia.Prevista);

            if (litologiaPoço == null)
                litologiaPoço = poço.Litologias.FirstOrDefault(x => x.Classificação == TipoLitologia.Interpretada);

            if (litologiaPoço == null)
                litologiaPoço = new Litologia(TipoLitologia.Adaptada, poço.Trajetória);

            var perfil = PerfisFactory.Create(perfilDto.Mnemonico, perfilDto.Nome, poço.Trajetória, litologiaPoço);

            var unidade = UnidadeMedida.ObterPorSímbolo(perfilDto.Unidade);

            if (unidade == null)
                unidade = GrupoUnidades.GetUnidadePadrão(perfil.Mnemonico);

            var grupo = GrupoUnidades.GetGrupoUnidades(perfil.Mnemonico);


            for (int i = 0; i < perfilDto.PontosDTO.Count; i++)
            {
                var pontoDto = perfilDto.PontosDTO[i];
                var valor = StringUtils.ToDoubleInvariantCulture(pontoDto.Valor, 2);
                var pm = StringUtils.ToDoubleInvariantCulture(pontoDto.Pm, 7);

                if (valor < 0 || pm < 0)
                {
                    continue;
                }

                if (perfilDto.TipoProfundidade == "Cota")
                {
                    var cota = pm;
                    var pv = poço.DadosGerais.Geometria.MesaRotativa + cota;
                    
                    perfil.AddPontoEmPv(poço.Trajetória, pv, grupo.ConvertToUnidadePadrão(unidade, valor), TipoProfundidade.PV, OrigemPonto.Importado);

                    if (i == perfilDto.PontosDTO.Count - 1)
                    {
                        var últimoPonto = poço.Trajetória.ÚltimoPonto;
                        var ponto = perfil.UltimoPonto;

                        if (ponto.Pm.Valor < últimoPonto.Pm.Valor && Math.Abs(poço.Trajetória.PvFinal.Valor - pv) < 0.01)
                        {
                            perfil.AddPontoEmPm(poço.Trajetória, últimoPonto.Pm.Valor, grupo.ConvertToUnidadePadrão(unidade, valor), TipoProfundidade.PM, OrigemPonto.Completado);
                        }

                    }

                }
                else
                {   
                    perfil.AddPontoEmPm(poço.Trajetória, pm, grupo.ConvertToUnidadePadrão(unidade, valor), TipoProfundidade.PV, pontoDto.Origem);
                }

            }

            poço.Perfis.Add(perfil);
        }

        private static void AtualizarLitologiaComLaminaDAgua(Poço entity)
        {
            foreach (var lito in entity.Litologias)
            {
                lito.AtualizarLitologiaComLaminaDAgua(entity.DadosGerais.Geometria.OffShore.LaminaDagua, entity.Trajetória);
            }
        }

        private static void AtualizaPrimeiroPontoDoPerfilDiametroDeBroca(Poço entity)
        {
            foreach (var perfil in entity.Perfis.Where(p => p.Mnemonico == "DIAM_BROCA"))
            {
                if (Math.Abs(perfil.PmMínimo.Valor) < 0.009)
                {
                    var soma = entity.ObterBaseDeSedimentos();

                    AtualizaPrimeiroPontoDoPerfilDiametroDeBroca(entity.Trajetória, perfil, soma);
                }
            }
        }

        private static void AtualizaPrimeiroPontoDoPerfilDiametroDeBroca(IConversorProfundidade conversorProfundidade, PerfilBase perfil, double profundidadeSedimentos)
        {
            if (perfil.Mnemonico != nameof(DIAM_BROCA))
                return;

            if (Math.Abs(perfil.PmMínimo.Valor) < 0.009)
            {
                perfil.AddPontoEmPm(conversorProfundidade, profundidadeSedimentos, perfil.PrimeiroPonto.Valor, TipoProfundidade.PM, perfil.PrimeiroPonto.Origem);
            }
        }

        private static void AtualizaPrimeiroPontoDoPerfilDTODiametroDeBroca(PerfilDTO perfil, double profundidadeSedimentos)
        {
            if (perfil.Mnemonico != nameof(DIAM_BROCA))
                return;

            if (perfil.PontosDTO.Count > 0 && double.Parse(perfil.PontosDTO[0].Pm) == 0)
            {
                var listaPontosPerfil = new List<PontoDTO>();
                PontoDTO pontoPerfil = new PontoDTO
                {
                    Origem = perfil.PontosDTO[0].Origem,
                    Pm = profundidadeSedimentos.ToString(),
                    Valor = perfil.PontosDTO[0].Valor
                };
                listaPontosPerfil.Add(pontoPerfil);

                var pontosNovos = listaPontosPerfil.Union(perfil.PontosDTO.Where(p => double.Parse(p.Pm) > 0));

                perfil.PontosDTO = pontosNovos.ToList();
            }
        }

        private static List<PontoDTO> RemoverPontosAcimaDaProfundidadeDeSedimentos(double profundidadeSedimentos, List<PontoDTO> pontosPerfil, string tipoProfundidade, double mesaRotativa)
        {
            for (var i = 0; i < pontosPerfil.Count(); i++)
            {
                var profundidade = StringUtils.ToDoubleInvariantCulture(pontosPerfil[i].Pm, 7);
                if (tipoProfundidade == "Cota")
                    profundidade = profundidade + mesaRotativa;

                if (profundidade >= profundidadeSedimentos)
                {
                    return pontosPerfil.GetRange(i, pontosPerfil.Count() - i);
                }
            }

            return new List<PontoDTO>();
        }

        private static void CriarTrajetória(TrajetóriaDTO trajetóriaDto, Poço entity)
        {
            entity.Trajetória.Clear();

            for (var index = 0; index < trajetóriaDto.Pontos.Count; index++)
            {
                var ponto = trajetóriaDto.Pontos[index];
                entity.Trajetória.AddPonto(StringUtils.ToDoubleInvariantCulture(ponto.Pm, 2),
                    StringUtils.ToDoubleInvariantCulture(ponto.Inclinação, 2),
                    StringUtils.ToDoubleInvariantCulture(ponto.Azimute, 2));
            }
        }

        private static void AcrescentarTrajetória(TrajetóriaDTO trajetóriaDto, Poço entity)
        {
            for (var index = 0; index < trajetóriaDto.Pontos.Count; index++)
            {
                var ponto = trajetóriaDto.Pontos[index];
                entity.Trajetória.AddPonto(StringUtils.ToDoubleInvariantCulture(ponto.Pm, 2),
                    StringUtils.ToDoubleInvariantCulture(ponto.Inclinação, 2),
                    StringUtils.ToDoubleInvariantCulture(ponto.Azimute, 2));
            }
        }

        private static void SobrescreverTrajetória(TrajetóriaDTO trajetóriaDto, Poço entity)
        {
            var pms = new List<double>();
            var inclinações = new List<double>();
            var azimutes = new List<double>();

            foreach (var pontoDto in trajetóriaDto.Pontos)
            {
                pms.Add(StringUtils.ToDoubleInvariantCulture(pontoDto.Pm, 2));
                inclinações.Add(StringUtils.ToDoubleInvariantCulture(pontoDto.Inclinação, 2));
                azimutes.Add(StringUtils.ToDoubleInvariantCulture(pontoDto.Azimute, 2));
            }

            entity.Trajetória.SobrescreverPontos(trajetóriaDto.ValorTopo.Value, trajetóriaDto.ValorBase.Value, pms, inclinações, azimutes);
        }
        #endregion
    }
}