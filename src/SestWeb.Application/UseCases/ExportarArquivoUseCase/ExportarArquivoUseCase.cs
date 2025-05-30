using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Application.Validadores;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Exportadores;
using SestWeb.Domain.Exportadores.Base;
using SestWeb.Domain.Exportadores.CSV;
using SestWeb.Domain.Exportadores.LAS;
using SestWeb.Domain.Helpers;

namespace SestWeb.Application.UseCases.ExportarArquivoUseCase
{
    internal class ExportarArquivoUseCase : IExportarArquivoUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;

        public ExportarArquivoUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPerfilReadOnlyRepository perfilReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
        }

        public async Task<ExportarArquivoOutput> Execute(ExportarArquivoInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);

                if (poço == null)
                {
                    return ExportarArquivoOutput.RequestInválido($"poço com ID {input.IdPoço} não foi encontrado.");
                }

                if (input.Topo < poço.ObterBaseDeSedimentos())
                {
                    return ExportarArquivoOutput.RequestInválido($"o topo deve estar abaixo da profundidade inicial de sedimentos.");
                }

                if (input.Base > poço.Trajetória.ÚltimoPonto.Pm.Valor)
                {
                    return ExportarArquivoOutput.RequestInválido($"a base não pode estar abaixo do último ponto da trajetória.");
                }

                if (input.Intervalo < 1)
                {
                    return ExportarArquivoOutput.RequestInválido($"o menor intervalo possível é 1 metro.");
                }

                var tipoExportaçãoÉVálido = Enum.TryParse(input.Arquivo, out TipoExportação tipoExportação);

                if (!tipoExportaçãoÉVálido)
                {
                    return ExportarArquivoOutput.RequestInválido($"tipo de exportação inválido selecionado: {input.Arquivo}");
                }

                var perfis = new List<PerfilBase>();
                
                foreach (var idPerfil in input.Perfis)
                {
                    var perfil = await _perfilReadOnlyRepository.ObterPerfil(idPerfil);
                    
                    if (perfil == null)
                    {
                        return ExportarArquivoOutput.RequestInválido($"perfil com ID {idPerfil} não foi encontrado.");
                    }

                    perfis.Add(perfil);
                }

                var configurações = new ConfiguraçõesExportador
                {
                    Base = input.Base,
                    Topo = input.Topo,
                    Intervalo = input.Intervalo,
                    DeveExportarTrajetoria = input.Trajetoria,
                    DeveExportarLitologia = input.Litologia,
                    DeveExportarPv = input.Pv,
                    DeveExportarCota = input.Cota
                };

                IExportadorBase exportador;
                byte[] conteudoArquivo;
                MemoryStream bytesArquivo;

                switch (tipoExportação)
                {
                    case TipoExportação.LAS:
                        exportador = new ExportadorLas(poço, perfis, configurações); break;
                    case TipoExportação.CSV:
                        exportador = new ExportadorCsv(poço, perfis, configurações); break;
                    default:
                        return ExportarArquivoOutput.Erro("tpo inválido de exportador");
                }

                conteudoArquivo = exportador.Exportar();
                bytesArquivo = new MemoryStream(conteudoArquivo);
                return ExportarArquivoOutput.ExportaçãoConcluída("Exportação concluída", bytesArquivo, $"{poço.Nome}.las");
            }
            catch (Exception e)
            {
                return ExportarArquivoOutput.Erro(e.Message);
            }
        }
    }
}
