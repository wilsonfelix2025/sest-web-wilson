namespace SestWeb.Api.UseCases.Perfil.ObterPerfisPorTipo
{
    /// <summary>
    /// Dados de entrada para caso de uso Obter perfis por tipo.
    /// </summary>
    public class ObterPerfisPorTipoRequest
    {
        /// <summary>
        /// Id do poço.
        /// </summary>
        public string IdPoço { get; set; }
        /// <summary>
        /// Mnemônico desejado.
        /// </summary>
        public string Mnemônico { get; set; }
    }
}
