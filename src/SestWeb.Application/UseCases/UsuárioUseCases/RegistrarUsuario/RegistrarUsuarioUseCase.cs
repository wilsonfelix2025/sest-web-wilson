using System;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;

namespace SestWeb.Application.UseCases.UsuárioUseCases.RegistrarUsuario
{
    public class RegistrarUsuarioUseCase : IRegistrarUsuarioUseCase
    {
        private readonly IUsuarioWriteOnlyRepository _usuarioWriteOnlyRepository;

        public RegistrarUsuarioUseCase(IUsuarioWriteOnlyRepository usuarioWriteOnlyRepository)
        {
            _usuarioWriteOnlyRepository = usuarioWriteOnlyRepository;
        }

        public async Task<RegistrarUsuarioOutput> Execute(string nome, string sobrenome, string email, string senha)
        {
            try
            {
                var usuario = await _usuarioWriteOnlyRepository.AddUser(email, nome, sobrenome, senha);
                return RegistrarUsuarioOutput.UsuarioRegistradoComSucesso(usuario);
            }
            catch (Exception e)
            {
                //TODO logar exceção aqui
                return RegistrarUsuarioOutput.UsuarioNãoRegistrado(e.Message);
            }
        }
    }
}
