namespace SestWeb.Api.UseCases.Estratigrafia.RemoverItemEstratigrafia
{
    /// <summary>
    /// Dados necessários para remover um item de estratigrafia
    /// </summary>
    public class RemoverItemEstratigrafiaRequest
    {
        /// <summary>
        /// Tipo de estratigrafia
        /// </summary>
        public string Tipo { get; set; }
        /// <summary>
        /// Profundidade medida
        /// </summary>
        public double Pm { get; set; }
    }
}
