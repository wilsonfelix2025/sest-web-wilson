using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Application.Services;
using SestWeb.Domain.Relatório;

namespace SestWeb.Application.UseCases.GerarRelatórioUseCase
{
    internal class GerarRelatórioUseCase : IGerarRelatórioUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IFileService _fileService;

        public GerarRelatórioUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IFileService fileService)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _fileService = fileService;
        }

        public async Task<GerarRelatórioOutput> Execute(GerarRelatórioInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);
                if (poço == null)
                    return GerarRelatórioOutput.Falha("Poço não encontrado");

                var configurações = PreencherConfiguraçõesRelatório(input);
                var relatório = new Relatório(poço, configurações);
                var caminho = _fileService.SalvarArquivoPúblico();
                var image = await relatório.Exportar(caminho, "jpg");

                return GerarRelatórioOutput.Sucesso(image);
            }
            catch (Exception e)
            {
                return GerarRelatórioOutput.Falha(e.Message);
            }
        }

        private ConfiguraçãoRelatório PreencherConfiguraçõesRelatório(GerarRelatórioInput input)
        {
            var estratigrafias = new List<TrackRelatório>();
            var gráficos = new List<TrackRelatório>();

            foreach (var estratigrafia in input.Estratigrafias) {
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
