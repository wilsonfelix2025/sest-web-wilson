namespace SestWeb.Api.UseCases.Litologia.RemoverLitologia
{
    /// <summary>
    /// Dados de entrada para caso de uso Remover litologia.
    /// </summary>
    public class RemoverLitologiaRequest
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
