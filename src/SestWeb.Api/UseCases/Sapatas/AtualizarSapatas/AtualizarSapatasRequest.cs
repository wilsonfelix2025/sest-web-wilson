
using SestWeb.Application.UseCases.SapataUseCases.AtualizarSapatas;

namespace SestWeb.Api.UseCases.Sapatas.AtualizarSapatas
{
    /// <summary>
    /// Dados necessários para adicionar sapatas
    /// </summary>
    public class AtualizarSapatasRequest
    {
        /// <summary>
        /// Se os topos das profundidades das sapatas estão em PM ou PV.
        /// </summary>
        public TipoProfundidade ProfundidadeReferência;
        /// <summary>
        /// Array com as estratigrafias do poço.
        /// </summary>
        public SapataChild[] Sapatas { get; set; }
    }
}
