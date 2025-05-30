using System;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.LoaderCorrelaçõesSistema;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.LoadCorrelaçõesSistema
{
    internal class LoadCorrelaçõesSistemaUseCase : ILoadCorrelaçõesSistemaUseCase
    {
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoWriteOnlyRepository _correlaçãoWriteOnlyRepository;
        private readonly ILoaderCorrelações _loaderCorrelações;

        public LoadCorrelaçõesSistemaUseCase(ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, ICorrelaçãoWriteOnlyRepository correlaçãoWriteOnlyRepository, ILoaderCorrelações loaderCorrelações)
        {
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoWriteOnlyRepository = correlaçãoWriteOnlyRepository;
            _loaderCorrelações = loaderCorrelações;
        }

        public async Task<LoadCorrelaçõesSistemaOutput> Execute()
        {
            try
            {
                var correlações = _loaderCorrelações.Load().Cast<Correlação>().ToList();

                var result = await _correlaçãoWriteOnlyRepository.InsertCorrelaçõesSistema(correlações);

                if(!result)
                    return LoadCorrelaçõesSistemaOutput.CorrelaçõesNãoCarregadas("Correlações não Carregadas.");

                return LoadCorrelaçõesSistemaOutput.CorrelaçõesCarregadas(correlações);
            }
            catch (Exception e)
            {
                return LoadCorrelaçõesSistemaOutput.CorrelaçõesNãoCarregadas(e.Message);
            }
        }
    }
}
