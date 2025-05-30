namespace SestWeb.Api.UseCases.Litologia.ObterLitologia
{
    /// <summary>
    /// Dados para obtenção de uma litologia.
    /// </summary>
    public class ObterLitologiaRequest
    {
        /// <summary>
        /// Id do poço.
        /// </summary>
        public string IdPoço { get; set; }

        /// <summary>
        /// Id da litologia.
        /// </summary>
        public string IdLitologia { get; set; }
    }
}
