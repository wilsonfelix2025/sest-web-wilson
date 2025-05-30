namespace SestWeb.Api.UseCases.Estratigrafia.AdicionarItemEstratigrafia
{
    /// <summary>
    /// Dados necessários para adicionar um item de estratigrafia
    /// </summary>
    public class AdicionarItemEstratigrafiaRequest
    {
        /// <summary>
        /// Tipo de estratigrafia
        /// </summary>
        public string Tipo { get; set; }
        /// <summary>
        /// Profundidade medida
        /// </summary>
        public double PM { get; set; }
        /// <summary>
        /// Sigla
        /// </summary>
        public string Sigla { get; set; }
        /// <summary>
        /// Descrição
        /// </summary>
        public string Descrição { get; set; }
        /// <summary>
        /// Idade
        /// </summary>
        public string Idade { get; set; }
    }
}
