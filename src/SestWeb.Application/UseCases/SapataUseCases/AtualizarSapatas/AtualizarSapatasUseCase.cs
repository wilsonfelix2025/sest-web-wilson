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

namespace SestWeb.Application.UseCases.SapataUseCases.AtualizarSapatas
{
    internal class AtualizarSapatasUseCase : IAtualizarSapatasUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public AtualizarSapatasUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository,
            IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<AtualizarSapatasOutput> Execute(string idPoço, AtualizarSapatasInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoço(idPoço);

                if (poço == null)
                    return AtualizarSapatasOutput.PoçoNãoEncontrado(idPoço);

                var dadosSelecionados = new List<DadosSelecionadosEnum>();
                dadosSelecionados.Add(DadosSelecionadosEnum.Sapatas);

                var poçoDto = PreencherDados(input, poço.Trajetória);
                var resultFactory =
                    PoçoFactory.EditarPoço(poçoDto, poço, null, null, dadosSelecionados, null);

                if (resultFactory.result.IsValid == false)
                {
                    return AtualizarSapatasOutput.SapatasNãoAtualizadas(string.Join("\n", resultFactory.result.Errors));
                }

                PoçoValidator validador = new PoçoValidator(_poçoReadOnlyRepository);
                var poçoValido = validador.Validate((Poço) resultFactory.Entity);

                if (poçoValido.IsValid == false)
                {
                    return AtualizarSapatasOutput.SapatasNãoAtualizadas(string.Join("\n", poçoValido.Errors));
                }

                var poçoEntity = (Poço) resultFactory.Entity;
                var result = await _poçoWriteOnlyRepository.AtualizarSapatas(idPoço, poçoEntity.Sapatas);

                if (result)
                {
                    return AtualizarSapatasOutput.SapatasAtualizadas();
                }

                return AtualizarSapatasOutput.SapatasNãoAtualizadas();
            }
            catch (Exception e)
            {
                return AtualizarSapatasOutput.SapatasNãoAtualizadas(e.Message);
            }
        }

        private static PoçoDTO PreencherDados(AtualizarSapatasInput input, Trajetória trajetória)
        {
            List<SapataDTO> sapatasDTO = new List<SapataDTO>();

            foreach (var sapata in input.Sapatas)
            {
                var valorPm = sapata.Pm;

                if (input.ProfundidadeReferência == TipoProfundidade.PV)
                {
                    var primeiroPontoTrajetoria = trajetória.GetPointsByPv(new Profundidade(sapata.Pm))[0];
                    valorPm = primeiroPontoTrajetoria.Pm.Valor;
                }

                sapatasDTO.Add(new SapataDTO
                {
                    Diâmetro = sapata.Diâmetro,
                    Pm = sapata.Pm.ToString()
                });
            }

            PoçoDTO poçoDTO = PoçoDTOFactory.CriarPoçoDTO(null, null, null, null, sapatasDTO, null, null, null, null);

            return poçoDTO;
        }
    }
}
