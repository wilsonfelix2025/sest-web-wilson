using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterCorrelaçõesPorTipo
{
    internal class ObterCorrelaçõesPorTipoUseCase : IObterCorrelaçõesPorTipoUseCase
    {
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoPoçoReadOnlyRepository _correlaçãoPoçoReadOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public ObterCorrelaçõesPorTipoUseCase(ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, ICorrelaçãoPoçoReadOnlyRepository correlaçãoPoçoReadOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoPoçoReadOnlyRepository = correlaçãoPoçoReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<ObterCorrelaçõesPorTipoOutput> Execute(string idPoço, string mnemônico)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return ObterCorrelaçõesPorTipoOutput.PoçoNãoEncontrado(idPoço);
                }

                var correlações = await GetCorrsByMnemonic(idPoço,mnemônico);

                if (correlações.Count == 0)
                {
                    return ObterCorrelaçõesPorTipoOutput.CorrelaçõesNãoEncontradas(mnemônico);
                }

                return ObterCorrelaçõesPorTipoOutput.CorrelaçõesObtidas(correlações);
            }
            catch (Exception e)
            {
                return ObterCorrelaçõesPorTipoOutput.CorrelaçõesNãoObtidas(e.Message);
            }
        }

        private async Task<List<Correlação>> GetCorrsByMnemonic(string idPoço, string mnemônico)
        {
            List<Correlação> correlações = new List<Correlação>();

            var corrs = await _correlaçãoReadOnlyRepository.ObterTodasCorrelações(); //ObterCorrelaçõesPorMnemônico(mnemônico);

            if (corrs.Count > 0)
            {
                foreach (var corr in corrs)
                {
                    if (corr.PerfisSaída.Tipos == null)
                        break;

                    var tipo = corr.PerfisSaída.Tipos[0];
                    if (tipo == mnemônico)
                        correlações.Add(corr);
                }
            }

            var correlaçõesPoço = await _correlaçãoPoçoReadOnlyRepository.ObterTodasCorrelaçõesPoço(idPoço);

            if (correlaçõesPoço.Count > 0)
            {
                foreach (var corr in correlaçõesPoço)
                {
                    if (corr.PerfisSaída.Tipos == null)
                        break;

                    var tipo = corr.PerfisSaída.Tipos[0];
                    if (tipo == mnemônico && correlações.Find(c => c.Nome.Equals(corr.Nome)) == null)
                        correlações.Add(corr);
                }
            }

            return correlações;
        }
    }
}
