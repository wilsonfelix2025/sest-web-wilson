using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.PoçoWeb.ResponseItem;

namespace SestWeb.Application.UseCases.RegistrosEventosUseCases.ReiniciarRegistroEvento
{
    internal class ReiniciarRegistroEventoUseCase : IReiniciarRegistroEventoUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IRegistrosEventosReadOnlyRepository _registrosEventosReadOnlyRepository;
        private readonly IRegistrosEventosWriteOnlyRepository _registrosEventosWriteOnlyRepository;
        public ReiniciarRegistroEventoUseCase(IPoçoReadOnlyRepository PoçoReadOnlyRepository, IRegistrosEventosWriteOnlyRepository RegistrosEventosWriteOnlyRepository, IRegistrosEventosReadOnlyRepository RegistrosEventosReadOnlyRepository)
        {
            _poçoReadOnlyRepository = PoçoReadOnlyRepository;
            _registrosEventosWriteOnlyRepository = RegistrosEventosWriteOnlyRepository;
            _registrosEventosReadOnlyRepository = RegistrosEventosReadOnlyRepository;
        }

        public async Task<ReiniciarRegistroEventoOutput> Execute(string id)
        {
            try
            {
                var existeRegistro = await _registrosEventosReadOnlyRepository.ExisteRegistroEventoComId(id);
                if (!existeRegistro)
                {
                    return ReiniciarRegistroEventoOutput.RegistroEventoNãoEncontrado(id);
                }

                var registro = await _registrosEventosReadOnlyRepository.ObterRegistroEvento(id);

                registro.ResetTrechos();
                registro.SetUnidade(null);

                var poço = await _poçoReadOnlyRepository.ObterPoço(registro.IdPoço);

                await _registrosEventosWriteOnlyRepository.AtualizarRegistroEvento(poço, registro);

                return ReiniciarRegistroEventoOutput.RegistroEventoReiniciado();
            }
            catch (Exception e)
            {
                return ReiniciarRegistroEventoOutput.RegistroEventoNãoReiniciado(e.Message);
            }
        }
    }
}
