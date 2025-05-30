using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterCorrelaçõesPorChaveUsuário
{
    internal class ObterCorrelaçõesPorChaveUsuárioUseCase : IObterCorrelaçõesPorChaveUsuárioUseCase
    {
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoPoçoReadOnlyRepository _correlaçãoPoçoReadOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public ObterCorrelaçõesPorChaveUsuárioUseCase(ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, ICorrelaçãoPoçoReadOnlyRepository correlaçãoPoçoReadOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoPoçoReadOnlyRepository = correlaçãoPoçoReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<ObterCorrelaçõesPorChaveUsuárioOutput> Execute(string idPoço, string userKey)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return ObterCorrelaçõesPorChaveUsuárioOutput.PoçoNãoEncontrado(idPoço);
                }

                var corrsByUserKey = await GetCorrsByUserKey(idPoço,userKey);

                if (corrsByUserKey.Count == 0)
                {
                    return ObterCorrelaçõesPorChaveUsuárioOutput.CorrelaçõesNãoEncontradas(userKey);
                }

                return ObterCorrelaçõesPorChaveUsuárioOutput.CorrelaçõesObtidas(corrsByUserKey);
            }
            catch (Exception e)
            {
                return ObterCorrelaçõesPorChaveUsuárioOutput.CorrelaçõesNãoObtidas(e.Message);
            }
        }

        private async Task<IReadOnlyCollection<Correlação>> GetCorrsByUserKey(string idPoço, string userKey)
        {
            List<Correlação> corrsByUserKey = new List<Correlação>();

            var correlações = await _correlaçãoReadOnlyRepository.ObterCorrelaçõesPorChaveUsuário(userKey);

            if(correlações?.Count > 0)
                corrsByUserKey.AddRange(correlações);

            var correlaçõesPoço =
                await _correlaçãoPoçoReadOnlyRepository.ObterCorrelaçõesPoçoPorChaveUsuário(idPoço, userKey);

            if(correlaçõesPoço?.Count > 0)
                corrsByUserKey.AddRange(correlaçõesPoço);

            return corrsByUserKey;
        }
    }
}
