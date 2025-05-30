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
using SestWeb.Domain.Importadores.Shallow.Utils;

namespace SestWeb.Application.UseCases.EstratigrafiaUseCases.AtualizarEstratigrafias
{
    internal class AtualizarEstratigrafiasUseCase : IAtualizarEstratigrafiasUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public AtualizarEstratigrafiasUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<AtualizarEstratigrafiasOutput> Execute(string idPoço, AtualizarEstratigrafiasInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoço(idPoço);

                if (poço == null)
                    return AtualizarEstratigrafiasOutput.PoçoNãoEncontrado(idPoço);

                var poçoDto = PreencherDados(input, poço.Trajetória);
                var resultFactory = PoçoFactory.EditarPoço(poçoDto, poço, null, null, new List<DadosSelecionadosEnum>(), null);

                if (resultFactory.result.IsValid == false)
                {
                    return AtualizarEstratigrafiasOutput.EstratigrafiasNãoAtualizadas(string.Join("\n", resultFactory.result.Errors));
                }

                PoçoValidator validador = new PoçoValidator(_poçoReadOnlyRepository);
                var poçoValido = validador.Validate((Poço)resultFactory.Entity);

                if (poçoValido.IsValid == false)
                {
                    return AtualizarEstratigrafiasOutput.EstratigrafiasNãoAtualizadas(string.Join("\n", poçoValido.Errors));
                }

                var poçoEntity = (Poço)resultFactory.Entity;
                var result = await _poçoWriteOnlyRepository.AtualizarEstratigrafia(idPoço, poçoEntity.Estratigrafia);

                if (result)
                {
                    return AtualizarEstratigrafiasOutput.EstratigrafiasAtualizadas();
                }

                return AtualizarEstratigrafiasOutput.EstratigrafiasNãoAtualizadas();
            }
            catch (Exception e)
            {
                return AtualizarEstratigrafiasOutput.EstratigrafiasNãoAtualizadas(e.Message);
            }
        }

        private static PoçoDTO PreencherDados(AtualizarEstratigrafiasInput input, Trajetória trajetória)
        {
            EstratigrafiaDTO estratigrafiaDTO = new EstratigrafiaDTO();
            
            foreach (var estratigrafia in input.Estratigrafias)
            {
                var valorPm = StringUtils.ToDoubleInvariantCulture(estratigrafia.ProfundidadeValor, 2);

                if (input.ProfundidadeReferência == TipoProfundidade.PV)
                {
                    var primeiroPontoTrajetoria = trajetória.GetPointsByPv(new Profundidade(valorPm))[0];
                    valorPm = primeiroPontoTrajetoria.Pm.Valor;
                }

                estratigrafiaDTO.Adicionar(estratigrafia.Tipo.ToString(), valorPm.ToString(), estratigrafia.Sigla, estratigrafia.Descrição, "0");
            }

            PoçoDTO poçoDTO = PoçoDTOFactory.CriarPoçoDTO(null, null, null, null, null, null, estratigrafiaDTO, null, null);

            return poçoDTO;
        }
    }
}
