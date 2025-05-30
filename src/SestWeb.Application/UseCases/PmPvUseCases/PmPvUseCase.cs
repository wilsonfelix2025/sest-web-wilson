using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.PmPvUseCases
{
    internal class PmPvUseCase : IPmPvUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public PmPvUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<PmPvOutput> Execute(string profType)
        {
            var poços = await _poçoReadOnlyRepository.ObterPoços();

            if(poços== null || poços.Count.Equals(0))
                return PmPvOutput.ConversãoNãoRealizada("Não há poços.");

            foreach (var poço in poços)
            {
                if(profType.Trim().Equals("pv"))
                    poço.ConverterParaPv();
                else
                    poço.ConverterParaPm();

                await _poçoWriteOnlyRepository.AtualizarPoço(poço);
            }

            return PmPvOutput.ConversãoRealizada();
        }
    }
}
