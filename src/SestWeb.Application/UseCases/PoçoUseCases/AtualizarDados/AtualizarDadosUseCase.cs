using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.CálculosUseCase.Pipeline;
using SestWeb.Application.UseCases.PoçoUseCases.AtualizarDados.EstratigrafiaInput;
using SestWeb.Application.UseCases.PoçoUseCases.AtualizarDados.GeometriaDoPoçoInput;
using SestWeb.Application.UseCases.PoçoUseCases.AtualizarDados.ObjetivoInput;
using SestWeb.Application.UseCases.PoçoUseCases.AtualizarDados.SapataInput;
using SestWeb.Application.Validadores;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Factories;
using SestWeb.Domain.Helpers;
using SestWeb.Domain.Importadores.Shallow.Utils;

namespace SestWeb.Application.UseCases.PoçoUseCases.AtualizarDados
{
    public class AtualizarDadosUseCase : IAtualizarDadosUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPerfilWriteOnlyRepository _perfilWriteOnlyRepository;
        private readonly IPipelineUseCase _pipeUseCase;


        public AtualizarDadosUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository
            , IPerfilWriteOnlyRepository perfilWriteOnlyRepository, IPipelineUseCase pipelineUseCase)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _perfilWriteOnlyRepository = perfilWriteOnlyRepository;
            _pipeUseCase = pipelineUseCase;
        }

        public async Task<AtualizarDadosOutput> Execute(string id, AtualizarDadosInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoço(id);

                if (poço == null)
                    return AtualizarDadosOutput.PoçoNãoEncontrado(id);

                var dadosSelecionados = new List<DadosSelecionadosEnum>();

                if (input.Identificação != null || input.Geometria != null || input.Area != null)
                    dadosSelecionados.Add(DadosSelecionadosEnum.DadosGerais);

                if (input.Objetivos != null)
                    dadosSelecionados.Add(DadosSelecionadosEnum.Objetivos);
                
                if (input.Sapatas != null)
                    dadosSelecionados.Add(DadosSelecionadosEnum.Sapatas);
                
                if (input.Estratigrafias != null)
                    dadosSelecionados.Add(DadosSelecionadosEnum.Estratigrafia);
                
                if (input.PmFinal > 0)
                    dadosSelecionados.Add(DadosSelecionadosEnum.Trajetória);

                if (input.Geometria != null && poço.DadosGerais.Geometria.MesaRotativa > 0 && !FastMath.SafeEquals(input.Geometria.MesaRotativa, poço.DadosGerais.Geometria.MesaRotativa, 0.0001))
                {
                    dadosSelecionados.Add(DadosSelecionadosEnum.MesaRotativa);
                }
                
                var poçoDto = PreencherDados(input, poço);
                var resultFactory = PoçoFactory.EditarPoço(poçoDto, poço, null, null, dadosSelecionados, null);

                if (resultFactory.result.IsValid == false)
                {
                    return AtualizarDadosOutput.DadosNãoAtualizados(string.Join("\n", resultFactory.result.Errors));
                }

                PoçoValidator validador = new PoçoValidator(_poçoReadOnlyRepository);
                var poçoValido = validador.Validate((Poço)resultFactory.Entity);

                if (poçoValido.IsValid == false)
                {
                    return AtualizarDadosOutput.DadosNãoAtualizados(string.Join("\n", poçoValido.Errors));
                }

                if (dadosSelecionados.Contains(DadosSelecionadosEnum.Trajetória) || dadosSelecionados.Contains(DadosSelecionadosEnum.MesaRotativa))
                {
                    await _pipeUseCase.Execute(poço.Cálculos.ToList(), poço);
                }

                var result = await _poçoWriteOnlyRepository.AtualizarDados(id, (Poço)resultFactory.Entity, dadosSelecionados);

                if (result)
                {
                    return AtualizarDadosOutput.DadosAtualizados(((Poço)resultFactory.Entity).Perfis, ((Poço)resultFactory.Entity).Litologias);
                }

                return AtualizarDadosOutput.DadosNãoAtualizados();
            }
            catch (Exception e)
            {
                return AtualizarDadosOutput.DadosNãoAtualizados(e.Message);
            }
        }

        private static PoçoDTO PreencherDados(AtualizarDadosInput input, Poço poço)
        {
            DadosGeraisDTO dadosGerais = null;
            TrajetóriaDTO trajetória = null;
            EstratigrafiaDTO estratigrafiaDTO = null;
            List<ObjetivoDTO> objetivosDTO = null;
            List<SapataDTO> sapatasDTO = null;

            #region Geometria

            if (input.PmFinal > 0)
            {
                trajetória = new TrajetóriaDTO();
                trajetória.Pontos.Add(new PontoTrajetóriaDTO
                {
                    Pm = input.PmFinal.ToString(),
                    Azimute = "0",
                    Inclinação = "0"
                });
            }

            if (input.Geometria != null)
            {
                dadosGerais = new DadosGeraisDTO();

                //OnShore
                dadosGerais.Geometria.OnShore.LençolFreático = input.Geometria.OnShore.LençolFreático.ToString();
                dadosGerais.Geometria.OnShore.Elevação = input.Geometria.OnShore.Elevação.ToString();
                dadosGerais.Geometria.OnShore.AlturaDeAntePoço = input.Geometria.OnShore.AlturaDeAntePoço.ToString();

                //OffShore
                if (input.Geometria.CategoriaPoço == CategoriaDoPoçoInput.OnShore)
                    dadosGerais.Geometria.OffShore.LaminaDagua = "0";
                else
                    dadosGerais.Geometria.OffShore.LaminaDagua = input.Geometria.OffShore.LaminaDagua.ToString();

                //Coordenadas
                dadosGerais.Geometria.Coordenadas.UtMx = input.Geometria.Coordenadas.UtMx.ToString();
                dadosGerais.Geometria.Coordenadas.UtMy = input.Geometria.Coordenadas.UtMy.ToString();

                dadosGerais.Geometria.MesaRotativa = input.Geometria.MesaRotativa.ToString();
                dadosGerais.Geometria.AtualizaMesaRotativaComElevação = false;

            }
            
            #endregion

            #region Identificação

            if (input.Identificação != null)
            {
                if (dadosGerais == null)
                    dadosGerais = new DadosGeraisDTO();

                dadosGerais.Identificação.NomePoço = input.Identificação.Nome;
                dadosGerais.Identificação.Sonda = input.Identificação.Sonda;
                dadosGerais.Identificação.Campo = input.Identificação.Campo;
                dadosGerais.Identificação.Companhia = input.Identificação.Companhia;
                dadosGerais.Identificação.Bacia = input.Identificação.Bacia;
                dadosGerais.Identificação.Finalidade = input.Identificação.Finalidade;
                dadosGerais.Identificação.Analista = input.Identificação.Analista;
                dadosGerais.Identificação.NívelProteção = input.Identificação.NívelProteção;
                dadosGerais.Identificação.ClassificaçãoPoço = input.Identificação.ClassificaçãoPoço;
                dadosGerais.Identificação.TipoCompletação = input.Identificação.TipoCompletação;
                dadosGerais.Identificação.ComplexidadePoço = input.Identificação.ComplexidadePoço;
                dadosGerais.Identificação.VidaÚtilPrevista = input.Identificação.VidaÚtilPrevista;
                dadosGerais.Identificação.CriticidadePoço = input.Identificação.CriticidadePoço;
                dadosGerais.Identificação.IntervençãoWorkover = input.Identificação.IntervençãoWorkover;
                
            }
            #endregion

            #region Area

            if (input.Area != null)
            {
                if (dadosGerais == null)
                    dadosGerais = new DadosGeraisDTO();

                dadosGerais.Area.DensidadeAguaMar = input.Area.DensidadeAguaMar.ToString();
                dadosGerais.Area.DensidadeSuperficie = input.Area.DensidadeSuperficie.ToString();
                dadosGerais.Area.SonicoSuperficie = input.Area.SonicoSuperficie.ToString();
                dadosGerais.Area.DTSSuperficie = input.Area.DTSSuperficie.ToString();
            }

            #endregion

            if (dadosGerais != null)
            {
                if (input.Geometria == null)
                    dadosGerais.Geometria = null;

                if (input.Area == null)
                    dadosGerais.Area = null;

                if (input.Identificação == null)
                    dadosGerais.Identificação = null;
            }

            #region Estratigrafia

            if (input.Estratigrafias != null)
            {
                estratigrafiaDTO = new EstratigrafiaDTO();

                foreach (var estratigrafia in input.Estratigrafias)
                {
                    var valorPm = StringUtils.ToDoubleInvariantCulture(estratigrafia.ProfundidadeValor, 2);

                    if (input.ProfundidadeReferênciaEstratigrafia == TipoProfundidadeEstratigrafia.PV)
                    {
                        var primeiroPontoTrajetoria = poço.Trajetória.TryGetMDFromTVD(valorPm, out double md);
                        valorPm = md;
                    }

                    estratigrafiaDTO.Adicionar(estratigrafia.Tipo.ToString(), valorPm.ToString(), estratigrafia.Sigla,
                        estratigrafia.Descrição, "0");
                }

            }

            #endregion

            #region Objetivos

            if (input.Objetivos != null)
            {
                objetivosDTO = new List<ObjetivoDTO>();

                foreach (var objetivo in input.Objetivos)
                {
                    var valorPm = objetivo.Pm;

                    if (input.ProfundidadeReferênciaObjetivo == TipoProfundidadeObjetivo.PV)
                    {
                        var primeiroPontoTrajetoria = poço.Trajetória.GetPointsByPv(new Profundidade(objetivo.Pm))[0];
                        valorPm = primeiroPontoTrajetoria.Pm.Valor; // TODO: Revisar
                    }

                    objetivosDTO.Add(new ObjetivoDTO(valorPm.ToString(), objetivo.TipoObjetivo.ToString()));
                }
            }

            #endregion

            #region Sapatas

            if (input.Sapatas != null)
            {
                sapatasDTO = new List<SapataDTO>();

                foreach (var sapata in input.Sapatas)
                {
                    var valorPm = sapata.Pm;

                    if (input.ProfundidadeReferênciaSapata == TipoProfundidadeSapata.PV)
                    {
                        var primeiroPontoTrajetoria = poço.Trajetória.GetPointsByPv(new Profundidade(sapata.Pm))[0];
                        valorPm = primeiroPontoTrajetoria.Pm.Valor;
                    }

                    sapatasDTO.Add(new SapataDTO
                    {
                        Diâmetro = sapata.Diâmetro,
                        Pm = valorPm.ToString()
                    });
                }
            }

            #endregion

            PoçoDTO poçoDTO = PoçoDTOFactory.CriarPoçoDTO(trajetória, null, dadosGerais, null, sapatasDTO, objetivosDTO, estratigrafiaDTO, null, null);

            return poçoDTO;
        }
    }
}
