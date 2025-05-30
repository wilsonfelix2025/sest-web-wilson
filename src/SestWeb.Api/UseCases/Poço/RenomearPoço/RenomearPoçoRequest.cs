namespace SestWeb.Api.UseCases.Poço.RenomearPoço
{
    /// <summary>
    /// Dados de entrada para caso de uso Renomear poço.
    /// </summary>
    public class RenomearPoçoRequest
    {
        /// <summary>
        /// Novo nome do poço.
        /// </summary>
        public string NomePoço { get; set; }
    }
}
