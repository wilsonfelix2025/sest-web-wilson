using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.CálculosUseCase.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.UseCases.PoçoUseCases.AtualizarDados;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Enums;

namespace SestWeb.Application.UseCases.TrajetóriaUseCases.EditarTrajetória
{
    internal class EditarTrajetóriaUseCase : IEditarTrajetóriaUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPerfilWriteOnlyRepository _perfilWriteOnlyRepository;
        private readonly IPipelineUseCase _pipeUseCase;


        public EditarTrajetóriaUseCase(IPoçoReadOnlyRepository TrajetóriaReadOnlyRepository, IPoçoWriteOnlyRepository TrajetóriaWriteOnlyRepository
            , IPerfilWriteOnlyRepository PerfilWriteOnlyRepository, IPipelineUseCase pipeline)
        {
            _poçoReadOnlyRepository = TrajetóriaReadOnlyRepository;
            _poçoWriteOnlyRepository = TrajetóriaWriteOnlyRepository;
            _perfilWriteOnlyRepository = PerfilWriteOnlyRepository;
            _pipeUseCase = pipeline;
        }

        public async Task<EditarTrajetóriaOutput> Execute(string idPoço, PontoTrajetóriaInput[] pontosDeTrajetória)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return EditarTrajetóriaOutput.PoçoNãoEncontrado(idPoço);
                }

                var poço = await _poçoReadOnlyRepository.ObterPoço(idPoço);

                if (pontosDeTrajetória.Length == 0)
                {
                    poço.Trajetória.GerarTrajetóriaPadrão();
                } else
                {
                    poço.Trajetória.Clear();

                    foreach (var ponto in pontosDeTrajetória)
                    {
                        poço.Trajetória.AddPonto(ponto.Pm, ponto.Inclinação, ponto.Azimute);
                    }
                }

                poço.AtualizarPvs();

                var dadosSelecionados = new List<DadosSelecionadosEnum>();
                dadosSelecionados.Add(DadosSelecionadosEnum.Trajetória);

                await _pipeUseCase.Execute(poço.Cálculos.ToList(), poço);
                var result = await _poçoWriteOnlyRepository.AtualizarDados(idPoço, poço, dadosSelecionados);

                if(!result)
                    return EditarTrajetóriaOutput.TrajetóriaNãoEditada("Trajetória não alterada.");

                return EditarTrajetóriaOutput.TrajetóriaEditada(poço.Trajetória, poço.Perfis, poço.Litologias);
            }
            catch (Exception e)
            {
                return EditarTrajetóriaOutput.TrajetóriaNãoEditada(e.Message);
            }
        }
    }
}
