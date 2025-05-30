namespace SestWeb.Api.UseCases.Usuário.ResetarSenha
{
    public class ResetarSenhaRequest
    {
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Codigo { get; set; }
    }
}
