namespace SestWeb.Api.UseCases.Litologia.CriarLitologia
{
    /// <summary>
    /// Dados para criação de uma nova litologia.
    /// </summary>
    public class CriarLitologiaRequest
    {
        /// <summary>
        /// Tipo de litologia: "Prevista" ou "Adaptada".
        /// </summary>
        public string TipoLitologia { get; set; }
    }
}
