using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.UsuárioUseCases.RegistrarUsuario
{
    public interface IRegistrarUsuarioUseCase
    {
        /// <summary>
        /// Registra um novo usuário ao sistema (sem privilágios de administrador).
        /// </summary>
        /// <param name="nome">Nome do usuário.</param>
        /// <param name="sobrenome">Sobrenome do usuário.</param>
        /// <param name="email">Email do usuário (@puc-rio ou @petrobras).</param>
        /// <param name="senha">Senha do usuário (deve conter letras maiúsculas, números e caracteres especiais).</param>
        /// <returns>Usuario registrado.</returns>
        Task<RegistrarUsuarioOutput> Execute(string nome, string sobrenome, string email, string senha);
    }
}
