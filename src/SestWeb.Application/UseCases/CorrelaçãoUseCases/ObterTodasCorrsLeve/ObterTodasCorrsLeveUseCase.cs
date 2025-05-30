using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterTodasCorrsLeve
{
    internal class ObterTodasCorrsLeveUseCase : IObterTodasCorrsLeveUseCase
    {
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoPoçoReadOnlyRepository _correlaçãoPoçoReadOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public ObterTodasCorrsLeveUseCase(ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, ICorrelaçãoPoçoReadOnlyRepository correlaçãoPoçoReadOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoPoçoReadOnlyRepository = correlaçãoPoçoReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<ObterTodasCorrsLeveOutput> Execute(string idPoço)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return ObterTodasCorrsLeveOutput.PoçoNãoEncontrado(idPoço);
                }

                var correlações = await LightweightGetAllCorrs(idPoço);

                if (correlações.Count == 0)
                {
                    return ObterTodasCorrsLeveOutput.CorrelaçõesNãoEncontradas();
                }

                return ObterTodasCorrsLeveOutput.CorrelaçõesObtidas(correlações);
            }
            catch (Exception e)
            {
                return ObterTodasCorrsLeveOutput.CorrelaçõesNãoObtidas(e.Message);
            }
        }

        private async Task<IReadOnlyCollection<Correlação>> LightweightGetAllCorrs(string idPoço)
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
