using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.InserirTrechoUseCase;

namespace SestWeb.Application.Validadores
{
    public class InserirTrechoValidator
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly InserirTrechoInput _input;
        private readonly string _idPoço;

        public InserirTrechoValidator(IPerfilReadOnlyRepository perfilReadOnlyRepository, InserirTrechoInput input, string idPoço)
        {
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _input = input;
            _idPoço = idPoço;
        }

        public async Task<string> Validate()
        {
            if (_input == null)
            {
                return ($"Dados de entrada não preenchidos");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(_input.NomeNovoPerfil))
                    return ($"Dados de entrada não preenchidos");
            }

            var existePerfilComMesmoNome = await _perfilReadOnlyRepository.ExistePerfilComMesmoNome(_input.NomeNovoPerfil, _idPoço);

            if (existePerfilComMesmoNome)
            {
                return ($"{_input.NomeNovoPerfil} - já existe perfil com esse nome");
            }

            return string.Empty;
        }


    }
}
