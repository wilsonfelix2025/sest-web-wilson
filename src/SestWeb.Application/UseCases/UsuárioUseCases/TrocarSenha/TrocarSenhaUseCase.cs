using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.UsuárioUseCases.TrocarSenha
{
    class TrocarSenhaUseCase : ITrocarSenhaUseCase
    {
        private readonly IUsuarioWriteOnlyRepository _usuarioWriteOnlyRepository;

        public TrocarSenhaUseCase(IUsuarioWriteOnlyRepository usuarioWriteOnlyRepository)
        {
            _usuarioWriteOnlyRepository = usuarioWriteOnlyRepository;
        }

        public async Task<TrocarSenhaUseCaseOutput> Execute(string idUsuario, string senhaAntiga, string novaSenha)
        {
            var result = await _usuarioWriteOnlyRepository.TrocarSenha(idUsuario, senhaAntiga, novaSenha);

            if (result)
            {
                return TrocarSenhaUseCaseOutput.SenhaTrocada();
            }

            return TrocarSenhaUseCaseOutput.SenhaNãoTrocada();
        }
    }
}
