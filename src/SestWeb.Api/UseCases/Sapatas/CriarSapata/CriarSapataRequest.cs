namespace SestWeb.Api.UseCases.Sapatas.CriarSapata
{
    /// <summary>
    /// Dados de entrada para caso de uso Criar sapata.
    /// </summary>
    public class CriarSapataRequest
    {
        /// <summary>
        /// Profundidade medida da sapata.
        /// </summary>
        public double ProfundidadeMedida { get; set; }
        /// <summary>
        /// Diâmetro da sapata em polegadas.
        /// </summary>
        public string DiâmetroSapata { get; set; }
    }
}
