namespace SestWeb.Api.UseCases.Usuário.ConfirmarEmail
{
    public class ConfirmarEmailRequest
    {
        public string IdUsuario { get; set; }
        public string Codigo { get; set; }
    }
}
