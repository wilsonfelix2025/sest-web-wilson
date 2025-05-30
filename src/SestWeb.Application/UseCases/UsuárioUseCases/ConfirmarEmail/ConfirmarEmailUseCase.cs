using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.UsuárioUseCases.ConfirmarEmail
{
    class ConfirmarEmailUseCase : IConfirmarEmailUseCase
    {
        private readonly IUsuarioWriteOnlyRepository _usuarioWriteOnlyRepository;

        public ConfirmarEmailUseCase(IUsuarioWriteOnlyRepository usuarioWriteOnlyRepository)
        {
            _usuarioWriteOnlyRepository = usuarioWriteOnlyRepository;
        }

        public async Task<ConfirmarEmailOutput> Execute(string idUsuario, string codigo)
        {
            var result = await _usuarioWriteOnlyRepository.ConfirmarEmail(idUsuario, codigo);

            if (result)
            {
                return ConfirmarEmailOutput.EmailConfirmado();
            }

            return ConfirmarEmailOutput.EmailNãoConfirmado();

        }
    }
}
