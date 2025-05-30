namespace SestWeb.Application.UseCases.UsuárioUseCases.RegistrarUsuario
{
    public class RegistrarUsuarioInput
    {
        public RegistrarUsuarioInput(string nome, string sobrenome, string email, string senha)
        {
            Nome = nome;
            Sobrenome = sobrenome;
            Email = email;
            Senha = senha;
        }

        public string Email { get; }
        public string Nome { get; }
        public string Senha { get; }
        public string Sobrenome { get; }
    }
}
