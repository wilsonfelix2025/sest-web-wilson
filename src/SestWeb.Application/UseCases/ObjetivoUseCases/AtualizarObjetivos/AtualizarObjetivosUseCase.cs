using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Application.Validadores;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.UseCases.ObjetivoUseCases.AtualizarObjetivos
{
    public class AtualizarObjetivosUseCase : IAtualizarObjetivosUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public AtualizarObjetivosUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository,
            IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<AtualizarObjetivosOutput> Execute(string idPoço, AtualizarObjetivosInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoço(idPoço);

                if (poço == null)
                    return AtualizarObjetivosOutput.PoçoNãoEncontrado(idPoço);

                var dadosSelecionados = new List<DadosSelecionadosEnum>();
                dadosSelecionados.Add(DadosSelecionadosEnum.Estratigrafia);

                var poçoDto = PreencherDados(input, poço.Trajetória);
                var resultFactory =
                    PoçoFactory.EditarPoço(poçoDto, poço, null, null, dadosSelecionados, null);

                if (resultFactory.result.IsValid == false)
                {
                    return AtualizarObjetivosOutput.ObjetivosNãoAtualizados(string.Join("\n", resultFactory.result.Errors));
                }

                PoçoValidator validador = new PoçoValidator(_poçoReadOnlyRepository);
                var poçoValido = validador.Validate((Poço)resultFactory.Entity);

                if (poçoValido.IsValid == false)
                {
                    return AtualizarObjetivosOutput.ObjetivosNãoAtualizados(string.Join("\n", poçoValido.Errors));
                }

                var poçoEntity = (Poço)resultFactory.Entity;
                var result = await _poçoWriteOnlyRepository.AtualizarObjetivos(idPoço, poçoEntity.Objetivos);

                if (result)
                {
                    return AtualizarObjetivosOutput.ObjetivosAtualizados();
                }

                return AtualizarObjetivosOutput.ObjetivosNãoAtualizados();
            }
            catch (Exception e)
            {
                return AtualizarObjetivosOutput.ObjetivosNãoAtualizados(e.Message);
            }
        }

        private static PoçoDTO PreencherDados(AtualizarObjetivosInput input, Trajetória trajetória)
        {
            List<ObjetivoDTO> objetivosDTO = new List<ObjetivoDTO>();

            foreach (var objetivo in input.Objetivos)
            {
                var valorPm = objetivo.Pm;

                if (input.ProfundidadeReferência == TipoProfundidade.PV)
                {
                    var primeiroPontoTrajetoria = trajetória.GetPointsByPv(new Profundidade(objetivo.Pm))[0];
                    valorPm = primeiroPontoTrajetoria.Pm.Valor; // TODO: Revisar
                }

                objetivosDTO.Add(new ObjetivoDTO(objetivo.Pm.ToString(), objetivo.Tipo.ToString()));
            }

            var poçoDTO = PoçoDTOFactory.CriarPoçoDTO(null, null, null, null, null, objetivosDTO, null, null, null);

            return poçoDTO;
        }
    }
}
