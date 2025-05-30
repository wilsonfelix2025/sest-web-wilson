using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.RegistrosEventos;
using SestWeb.Domain.Exportadores.Base;
using SestWeb.Domain.Exportadores.CSV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.ExportarRegistros
{
    public class ExportarRegistrosUseCase : IExportarRegistrosUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IRegistrosEventosReadOnlyRepository _registrosReadOnlyRepository;

        public ExportarRegistrosUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IRegistrosEventosReadOnlyRepository registrosReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _registrosReadOnlyRepository = registrosReadOnlyRepository;
        }

        public async Task<ExportarRegistrosOutput> Execute(ExportarRegistrosInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);

                if (poço == null)
                {
                    return ExportarRegistrosOutput.RequestInválido($"poço com ID {input.IdPoço} não foi encontrado.");
                }
             
                var registrosDoPoço = await _registrosReadOnlyRepository.ObterRegistrosEventosDeUmPoço(input.IdPoço);
                var registros = registrosDoPoço.Where(r => input.Registros.Contains(r.Nome)).ToList();


                var exportador = new ExportadorCsv(poço, registros); 
                byte[] conteudoArquivo;
                MemoryStream bytesArquivo;

                conteudoArquivo = exportador.ExportarRegistros();
                bytesArquivo = new MemoryStream(conteudoArquivo);
                return ExportarRegistrosOutput.ExportaçãoConcluída("Exportação concluída", bytesArquivo, $"{poço.Nome}.csv");
            }
            catch (Exception e)
            {
                return ExportarRegistrosOutput.Erro(e.Message);
            }
        }
    }
}
