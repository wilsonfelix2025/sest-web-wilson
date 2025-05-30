using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.UsuárioUseCases.EsqueceuSenha
{
    internal class EsqueceuSenhaUseCase : IEsqueceuSenhaUseCase
    {
        private readonly IUsuarioWriteOnlyRepository _usuarioWriteOnlyRepository;

        public EsqueceuSenhaUseCase(IUsuarioWriteOnlyRepository usuarioWriteOnlyRepository)
        {
            _usuarioWriteOnlyRepository = usuarioWriteOnlyRepository;
        }

        public async Task<EsqueceuSenhaUseCaseOutput> Execute(string email)
        {
            var codigoReset = await _usuarioWriteOnlyRepository.EnviarEmailDeRecuperaçãoSenha(email);

            if (codigoReset == null)
            {
                return EsqueceuSenhaUseCaseOutput.UsuarioNaoEncontrado();
            }

            return EsqueceuSenhaUseCaseOutput.EmailEnviado(codigoReset);
        }
    }
}
