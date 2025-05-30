using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterTodasCorrelações
{
    internal class ObterTodasCorrelaçõesUseCase : IObterTodasCorrelaçõesUseCase
    {
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoPoçoReadOnlyRepository _correlaçãoPoçoReadOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public ObterTodasCorrelaçõesUseCase(ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, ICorrelaçãoPoçoReadOnlyRepository correlaçãoPoçoReadOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoPoçoReadOnlyRepository = correlaçãoPoçoReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<ObterTodasCorrelaçõesOutput> Execute(string idPoço)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return ObterTodasCorrelaçõesOutput.PoçoNãoEncontrado(idPoço);
                }

                var correlações = await GetAllCorrs(idPoço);

                if (correlações.Count == 0)
                {
                    return ObterTodasCorrelaçõesOutput.CorrelaçõesNãoEncontradas();
                }

                return ObterTodasCorrelaçõesOutput.CorrelaçõesObtidas(correlações);
            }
            catch (Exception e)
            {
                return ObterTodasCorrelaçõesOutput.CorrelaçõesNãoObtidas(e.Message);
            }
        }

        private async Task<IReadOnlyCollection<Correlação>> GetAllCorrs(string idPoço)
        {
            List<Correlação> corrs = new List<Correlação>();

            var correlações = await _correlaçãoReadOnlyRepository.ObterTodasCorrelações();

            if (correlações?.Count > 0)
                corrs.AddRange(correlações);

            var correlaçõesPoço = await _correlaçãoPoçoReadOnlyRepository.ObterTodasCorrelaçõesPoço(idPoço);

            if (correlaçõesPoço?.Count > 0)
                corrs.AddRange(correlaçõesPoço);

            return corrs;
        }
    }
}
