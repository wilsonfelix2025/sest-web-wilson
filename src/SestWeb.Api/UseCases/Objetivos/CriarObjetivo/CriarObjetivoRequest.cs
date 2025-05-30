using SestWeb.Domain.Entities.Poço.Objetivos;

namespace SestWeb.Api.UseCases.Objetivos.CriarObjetivo
{
    /// <summary>
    /// Dados para criação de um novo objetivo.
    /// </summary>
    public class CriarObjetivoRequest
    {
        /// <summary>
        /// Profundidade medida do objetivo.
        /// </summary>
        public double ProfundidadeMedida { get; set; }
        /// <summary>
        /// Tipo do objetivo: Primário ou Secundário.
        /// Caso não seja informado, será criado um objetivo do tipo Primário.
        /// </summary>
        public TipoObjetivo TipoObjetivo { get; set; }
    }
}
