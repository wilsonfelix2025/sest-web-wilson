namespace SestWeb.Api.UseCases.Usuário.RegistrarUsuario
{
    public class RegistrarUsuarioRequest
    {
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}
