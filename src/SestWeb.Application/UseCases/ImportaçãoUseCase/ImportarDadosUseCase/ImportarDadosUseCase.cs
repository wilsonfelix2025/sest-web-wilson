using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Application.Validadores;
using SestWeb.Domain.Entities.Poço;

namespace SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase
{
    public abstract class ImportarDadosUseCase<T> where T : class
    {
        public readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        public readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        public readonly IPerfilWriteOnlyRepository _perfilWriteOnlyRepository;

        protected Poço Poço { get; private set; }       

        public ImportarDadosUseCase(IPoçoWriteOnlyRepository poçoWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository
            , IPerfilWriteOnlyRepository perfilWriteOnlyRepository)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _perfilWriteOnlyRepository = perfilWriteOnlyRepository;
        }

        
        public async Task<string> ValidaçõesIniciais(string poçoId, T obj)
        {
            var validador = new ImportarDadosValidator();

            var result = string.Empty;

            result = validador.ValidarDadosParaImportarNãoNulo(obj);
            
            if (!string.IsNullOrWhiteSpace(result))
                return result;

            Poço = await _poçoReadOnlyRepository.ObterPoço(poçoId);

            if (Poço == null)
            {
                result = "Não foi possível encontrar poço com id " + poçoId;
                return result;
            }

            return result;
        }
       
    }
}
