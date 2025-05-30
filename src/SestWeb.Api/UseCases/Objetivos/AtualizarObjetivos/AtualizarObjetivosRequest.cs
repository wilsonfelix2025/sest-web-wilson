using SestWeb.Application.UseCases.ObjetivoUseCases.AtualizarObjetivos;

namespace SestWeb.Api.UseCases.Objetivos.AtualizarObjetivos
{
    /// <summary>
    /// Dados necessários para atualizar objetivos
    /// </summary>
    public class AtualizarObjetivosRequest
    {
        /// <summary>
        /// Se os topos das profundidades das sapatas estão em PM ou PV.
        /// </summary>
        public TipoProfundidade ProfundidadeReferência;
        /// <summary>
        /// Array com as estratigrafias do poço.
        /// </summary>
        public ObjetivoChild[] Objetivos { get; set; }
    }
}
