using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Relatório;

namespace SestWeb.Application.UseCases.BaixarRelatórioUseCase
{
    internal class BaixarRelatórioUseCase : IBaixarRelatórioUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public BaixarRelatórioUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;            
        }

        public async Task<BaixarRelatórioOutput> Execute(BaixarRelatórioInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);
                if (poço == null)
                    return BaixarRelatórioOutput.Falha("Poço não encontrado");

                var configurações = PreencherConfiguraçõesRelatório(input);
                var relatório = new Relatório(poço, configurações);
                var bytes = relatório.ExportarBytes();
                return BaixarRelatórioOutput.Sucesso("Arquivo gerado com sucesso.", new MemoryStream(bytes), $"{poço.Nome}.jpg");
            }
            catch (Exception e)
            {
                return BaixarRelatórioOutput.Falha(e.Message);
            }
        }

        private ConfiguraçãoRelatório PreencherConfiguraçõesRelatório(BaixarRelatórioInput input)
        {
            var estratigrafias = new List<TrackRelatório>();
            var gráficos = new List<TrackRelatório>();

            foreach (var estratigrafia in input.Estratigrafias)
            {
                estratigrafias.Add(new TrackRelatório
                {
                    Titulo = estratigrafia.Título,
                    Data = estratigrafia.Data,
                    Curvas = estratigrafia.Curvas
                });
            }

            foreach (var gráfico in input.Gráficos)
            {
                gráficos.Add(new TrackRelatório
                {
                    Titulo = gráfico.Título,
                    Data = gráfico.Data,
                    Curvas = gráfico.Curvas
                });
            }

            return new ConfiguraçãoRelatório
            {
                MostrarNome = input.Nome,
                MostrarTipoPoço = input.Tipo,
                MostrarProfundidadeFinal = input.ProfundidadeFinal,
                MostrarMR = input.AlturaMR,
                MostrarLDA = input.Lda,
                Trajetória = new TrackRelatório
                {
                    Titulo = input.Trajetória.Título,
                    Data = input.Trajetória.Data,
                    Curvas = input.Trajetória.Curvas
                },
                Litologia = new TrackRelatório
                {
                    Titulo = input.Litologia.Título,
                    Data = input.Litologia.Data,
                    Curvas = input.Litologia.Curvas
                },
                Estratigrafias = estratigrafias,
                Graficos = gráficos
            };
        }
    }
}
