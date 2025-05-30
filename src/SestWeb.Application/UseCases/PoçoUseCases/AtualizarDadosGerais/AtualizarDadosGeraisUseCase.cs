using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.PoçoUseCases.AtualizarDadosGerais.GeometriaDoPoçoInput;
using SestWeb.Application.Validadores;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.UseCases.PoçoUseCases.AtualizarDadosGerais
{
    internal class AtualizarDadosGeraisUseCase : IAtualizarDadosGeraisUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public AtualizarDadosGeraisUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<AtualizarDadosGeraisOutput> Execute(string id, AtualizarDadosGeraisInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoço(id);

                if (poço == null)
                    return AtualizarDadosGeraisOutput.PoçoNãoEncontrado(id);

                var atualizaTrajetória = false;
                var dadosSelecionados = new List<DadosSelecionadosEnum>();
                dadosSelecionados.Add(DadosSelecionadosEnum.DadosGerais);
                if (input.Geometria.ÉVertical)
                {
                    atualizaTrajetória = true;
                    dadosSelecionados.Add(DadosSelecionadosEnum.Trajetória);
                }

                var poçoDto = PreencherDados(input);
                var resultFactory = PoçoFactory.EditarPoço(poçoDto,poço,null,null, dadosSelecionados, null);

                if (resultFactory.result.IsValid == false)
                {
                    return AtualizarDadosGeraisOutput.DadosGeraisNãoAtualizados(string.Join("\n", resultFactory.result.Errors));
                }
                
                PoçoValidator validador = new PoçoValidator(_poçoReadOnlyRepository);
                var poçoValido = validador.Validate((Poço)resultFactory.Entity);

                if (poçoValido.IsValid == false)
                {
                    return AtualizarDadosGeraisOutput.DadosGeraisNãoAtualizados(string.Join("\n", poçoValido.Errors));
                }

                var result = await _poçoWriteOnlyRepository.AtualizarDadosGerais(id, (Poço)resultFactory.Entity, atualizaTrajetória);

                if (result)
                {
                    return AtualizarDadosGeraisOutput.DadosGeraisAtualizados();
                }

                return AtualizarDadosGeraisOutput.DadosGeraisNãoAtualizados();
            }
            catch (Exception e)
            {
                return AtualizarDadosGeraisOutput.DadosGeraisNãoAtualizados(e.Message);
            }
        }

        private static PoçoDTO PreencherDados(AtualizarDadosGeraisInput input)
        {
            DadosGeraisDTO dadosGerais = new DadosGeraisDTO();
            TrajetóriaDTO trajetória = null;

            #region Geometria

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

            if (input.Geometria.ÉVertical)
            {
                trajetória = new TrajetóriaDTO();
                trajetória.Pontos.Add(new PontoTrajetóriaDTO {
                    Pm = input.Geometria.PmFinal.ToString(),
                    Azimute = "0",
                    Inclinação = "0"
                });
            }
            #endregion

            #region Identificação

            dadosGerais.Identificação.Nome = input.Identificação.Nome;
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

            #endregion

            #region Area

            dadosGerais.Area.DensidadeAguaMar = input.Area.DensidadeAguaMar.ToString();
            dadosGerais.Area.DensidadeSuperficie = input.Area.DensidadeSuperficie.ToString();
            dadosGerais.Area.SonicoSuperficie = input.Area.SonicoSuperficie.ToString();
            dadosGerais.Area.DTSSuperficie = input.Area.DTSSuperficie.ToString();

            #endregion

            PoçoDTO poçoDTO = PoçoDTOFactory.CriarPoçoDTO(trajetória, null, dadosGerais, null, null,null,null, null, null);

            return poçoDTO;
        }
    }
}
