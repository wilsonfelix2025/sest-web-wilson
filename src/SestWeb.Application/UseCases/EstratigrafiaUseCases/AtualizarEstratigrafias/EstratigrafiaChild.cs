namespace SestWeb.Application.UseCases.EstratigrafiaUseCases.AtualizarEstratigrafias
{
    public class EstratigrafiaChild
    {
        /// <summary>
        /// A profundidade do topo da estratigrafia.
        /// </summary>
        public string ProfundidadeValor;
        /// <summary>
        /// O tipo da estratigrafia.
        /// </summary>
        public TipoEstratigrafia Tipo;
        /// <summary>
        /// Sigla da estratigrafia.
        /// </summary>
        public string Sigla { get; set; }
        /// <summary>
        /// Descrição da estratigrafia.
        /// </summary>
        public string Descrição { get; set; }
    }
}
