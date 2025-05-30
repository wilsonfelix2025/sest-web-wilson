using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterCorrelaçõesUsuários
{
    internal class ObterCorrelaçõesUsuáriosUseCase : IObterCorrelaçõesUsuáriosUseCase
    {
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly ICorrelaçãoPoçoReadOnlyRepository _correlaçãoPoçoReadOnlyRepository;

        public ObterCorrelaçõesUsuáriosUseCase(ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, ICorrelaçãoPoçoReadOnlyRepository correlaçãoPoçoReadOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _correlaçãoPoçoReadOnlyRepository = correlaçãoPoçoReadOnlyRepository;
        }

        public async Task<ObterCorrelaçõesUsuáriosOutput> Execute(string idPoço)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return ObterCorrelaçõesUsuáriosOutput.PoçoNãoEncontrado(idPoço);
                }

                var userCorrs = await GetUserCorrs(idPoço);

                if (userCorrs.Count == 0)
                {
                    return ObterCorrelaçõesUsuáriosOutput.CorrelaçõesNãoEncontradas();
                }

                return ObterCorrelaçõesUsuáriosOutput.CorrelaçõesObtidas(userCorrs);
            }
            catch (Exception e)
            {
                return ObterCorrelaçõesUsuáriosOutput.CorrelaçõesNãoObtidas(e.Message);
            }
        }

        private async Task<IReadOnlyCollection<Correlação>> GetUserCorrs(string idPoço)
        {
            List<Correlação> userCorrs = new List<Correlação>(); 

            var correlações = await _correlaçãoReadOnlyRepository.ObterCorrelaçõesDeUsuários();

            if(correlações?.Count > 0)
                userCorrs.AddRange(correlações);

            var correlaçõesPoço = await _correlaçãoPoçoReadOnlyRepository.ObterTodasCorrelaçõesPoço(idPoço);

            if(correlaçõesPoço?.Count > 0)
                userCorrs.AddRange(correlaçõesPoço);

            return userCorrs;
        }
     }
}
