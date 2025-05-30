using System;
using MongoDB.Bson;

using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Poço;

namespace SestWeb.Application.UseCases.PoçoUseCases.AtualizarState
{
    internal class AtualizarStateUseCase : IAtualizarStateUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public AtualizarStateUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository,
            IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<AtualizarStateOutput> Execute(string id, AtualizarStateInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoço(id);

                if (poço == null)
                    return AtualizarStateOutput.PoçoNãoEncontrado(id);

                poço.State.Tabs = ProcessarTabs(input, poço);
                poço.State.ProfundidadeExibição = input.ProfundidadeExibição;
                if (input.PosicaoDivisaoAreaGrafica != null)
                {
                    poço.State.PosicaoDivisaoAreaGrafica = input.PosicaoDivisaoAreaGrafica;
                }

                var result = await _poçoWriteOnlyRepository.AtualizarPoço(poço);

                if (result) return AtualizarStateOutput.StateAtualizado(poço.State);

                return AtualizarStateOutput.StateNãoAtualizado();
            }
            catch (Exception e)
            {
                return AtualizarStateOutput.StateNãoAtualizado(e.Message);
            }
        }

        private List<Aba> ProcessarTabs(AtualizarStateInput input, Poço poço)
        {
            var poçoTabs = new List<Aba>();

            //Tabs
            var inputTabs = input.Tabs;
            foreach (var inputTab in inputTabs)
            {
                Aba aba;
                if (inputTab.Id != null)
                {
                    aba = new Aba(inputTab.Id, inputTab.Name, inputTab.Fixa);
                }
                else
                {
                    aba = new Aba(inputTab.Name, inputTab.Fixa);
                }

                foreach (var gráficoInput in inputTab.Data)
                {
                    Gráfico gráfico;
                    if (gráficoInput.Id != null)
                    {
                        gráfico = new Gráfico(gráficoInput.Id, gráficoInput.Name, gráficoInput.Largura);
                    }
                    else
                    {
                        gráfico = new Gráfico(gráficoInput.Name, gráficoInput.Largura);
                    }
                    gráfico.MaxX = gráficoInput.MaxX;
                    gráfico.MinX = gráficoInput.MinX;
                    gráfico.Intervalo = gráficoInput.Intervalo;
                    foreach (var itemInput in gráficoInput.Items)
                    {
                        var item = new Item
                        {
                            IdPoço = itemInput.IdPoço,
                            Id = itemInput.Id,
                            AdicionadoTrend = itemInput.AdicionadoTrend
                        };
                        gráfico.Items.Add(item);
                    }
                    aba.Data.Add(gráfico);
                }

                poçoTabs.Add(aba);
            }
            poço.State.IdAbaAtual = ObjectId.Parse(input.IdAbaAtual);

            return poçoTabs;
        }
    }
}