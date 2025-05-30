using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.UsuárioUseCases.ResetarSenha
{
    class ResetarSenhaUseCase : IResetarSenhaUseCase
    {
        private readonly IUsuarioWriteOnlyRepository _usuarioWriteOnlyRepository;

        public ResetarSenhaUseCase(IUsuarioWriteOnlyRepository usuarioWriteOnlyRepository)
        {
            _usuarioWriteOnlyRepository = usuarioWriteOnlyRepository;
        }

        public async Task<ResetarSenhaUseCaseOutput> Execute(string email, string novaSenha, string codigo)
        {
            var result = await _usuarioWriteOnlyRepository.ResetarSenha(email, novaSenha, codigo);

            if (result)
            {
                return ResetarSenhaUseCaseOutput.SenhaResetada();
            }

            return ResetarSenhaUseCaseOutput.SenhaNãoResetada();
        }
    }
}
