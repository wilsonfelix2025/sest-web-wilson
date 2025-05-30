namespace SestWeb.Api.UseCases.Perfil.CriarPerfil
{
    /// <summary>
    /// Informações necessárias para a criação do perfil.
    /// </summary>
    public class CriarPerfilRequest
    {
        /// <summary>
        /// Id do poço.
        /// </summary>
        public string IdPoço { get; set; }
        /// <summary>
        /// Nome do perfil.
        /// </summary>
        public string Nome { get; set; }
        /// <summary>
        /// Menmônico do perfil.
        /// </summary>
        public string Mnemônico { get; set; }
    }
}
