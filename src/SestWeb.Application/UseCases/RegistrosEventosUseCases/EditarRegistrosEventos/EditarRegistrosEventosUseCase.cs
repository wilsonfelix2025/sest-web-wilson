using SestWeb.Application.Repositories;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Enums;

namespace SestWeb.Application.UseCases.RegistrosEventosUseCases.EditarRegistrosEventos
{
    internal class EditarRegistrosEventosUseCase : IEditarRegistrosEventosUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IRegistrosEventosReadOnlyRepository _registrosEventosReadOnlyRepository;
        private readonly IRegistrosEventosWriteOnlyRepository _registrosEventosWriteOnlyRepository;

        public EditarRegistrosEventosUseCase(IPoçoReadOnlyRepository PoçoReadOnlyRepository, IPoçoWriteOnlyRepository PoçoWriteOnlyRepository, IRegistrosEventosWriteOnlyRepository RegistrosEventosWriteOnlyRepository, IRegistrosEventosReadOnlyRepository RegistrosEventosReadOnlyRepository)
        {
            _poçoReadOnlyRepository = PoçoReadOnlyRepository;
            _poçoWriteOnlyRepository = PoçoWriteOnlyRepository;
            _registrosEventosWriteOnlyRepository = RegistrosEventosWriteOnlyRepository;
            _registrosEventosReadOnlyRepository = RegistrosEventosReadOnlyRepository;
        }

        public async Task<EditarRegistrosEventosOutput> Execute(string idPoço, TipoRegistroEvento tipo, TrechoRegistroEventoInput[] trechos, MarcadorRegistroEventoInput[] marcadores)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return EditarRegistrosEventosOutput.PoçoNãoEncontrado(idPoço);
                }

                var poço = await _poçoReadOnlyRepository.ObterPoço(idPoço);
                var trajetória = poço.Trajetória;

                var registros = await _registrosEventosReadOnlyRepository.ObterRegistrosEventosDeUmPoço(idPoço);
                List<TrechoRegistroEventoInput> _trechos = trechos.ToList();
                List<MarcadorRegistroEventoInput> _marcadores = marcadores.ToList();
                foreach (var registro in registros)
                {
                    if (registro.Tipo == tipo)
                    {
                        registro.ResetTrechos();
                        if (_marcadores.Count > 0)
                        {
                            for (int i = _marcadores.Count - 1; i >= 0; i--)
                            {
                                if (_marcadores[i].IdRegistroEvento == registro.Id.ToString())
                                {
                                    registro.SetEstiloVisual(_marcadores[i].Marcador, _marcadores[i].CorDoMarcador, _marcadores[i].ContornoDoMarcador);

                                    if (_marcadores[i].ValorPadrão.HasValue)
                                    {
                                        registro.SetValorPadrão(_marcadores[i].ValorPadrão.Value);
                                    }

                                    if (_marcadores[i].Unidade != null && _trechos.Count > 0)
                                    {
                                        registro.SetUnidade(_marcadores[i].Unidade);
                                    }
                                    else
                                    {
                                        registro.SetUnidade(null);
                                    }

                                    _marcadores.RemoveAt(i);
                                }
                            }
                        }
                        if (_trechos.Count > 0)
                        {
                            for (int i = _trechos.Count - 1; i >= 0; i--)
                            {
                                if (_trechos[i].IdRegistroEvento == registro.Id.ToString())
                                {
                                    if (_trechos[i].Valor.HasValue)
                                    {
                                        registro.AddTrecho(_trechos[i].Pm, _trechos[i].Pv, _trechos[i].Valor.Value, _trechos[i].Comentário, trajetória);
                                    }
                                    else
                                    {
                                        registro.AddTrecho(_trechos[i].PmTopo, _trechos[i].PmBase, _trechos[i].PvTopo, _trechos[i].PvBase, _trechos[i].Comentário, trajetória);
                                    }

                                    _trechos.RemoveAt(i);
                                }
                            }
                        }
                    }
                }

                await _registrosEventosWriteOnlyRepository.AtualizarRegistrosEventos(poço, registros);
                await _poçoWriteOnlyRepository.AtualizarPoço(poço);

                return EditarRegistrosEventosOutput.RegistrosEventosEditados(poço.RegistrosEventos);
            }
            catch (Exception e)
            {
                return EditarRegistrosEventosOutput.RegistrosEventosNãoEditados(e.Message);
            }
        }
    }
}
