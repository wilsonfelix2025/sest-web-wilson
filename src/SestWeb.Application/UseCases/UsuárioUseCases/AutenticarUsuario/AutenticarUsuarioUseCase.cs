using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.UsuárioUseCases.AutenticarUsuario
{
    class AutenticarUsuarioUseCase : IAutenticarUsuarioUseCase
    {
        private readonly IUsuarioReadOnlyRepository _readOnlyRepository;

        public AutenticarUsuarioUseCase(IUsuarioReadOnlyRepository readOnlyRepository)
        {
            _readOnlyRepository = readOnlyRepository;
        }

        public async Task<AutenticarUsuarioOutput> Execute(string email, string senha)
        {
            try
            {
                var usuario = await _readOnlyRepository.GetUserByPassword(email, senha);

                if (usuario.EmailConfirmado)
                {
                    return AutenticarUsuarioOutput.UsuarioAutenticado(usuario);
                }

                return AutenticarUsuarioOutput.EmailNãoConfirmado();

            }
            catch (Exception e)
            {
                return AutenticarUsuarioOutput.UsuarioNaoAutenticado(e.Message);
            }
        }
    }
}
