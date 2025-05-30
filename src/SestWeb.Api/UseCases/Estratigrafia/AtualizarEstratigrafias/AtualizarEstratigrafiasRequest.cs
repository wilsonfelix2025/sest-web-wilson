using SestWeb.Application.UseCases.EstratigrafiaUseCases.AtualizarEstratigrafias;

namespace SestWeb.Api.UseCases.Estratigrafia.AtualizarEstratigrafias
{
    /// <summary>
    /// Dados necessários para adicionar um item de estratigrafia
    /// </summary>
    public class AtualizarEstratigrafiasRequest
    {
        /// <summary>
        /// Se os topos das profundidades das estratigrafias estão em PM ou PV.
        /// </summary>
        public TipoProfundidade ProfundidadeReferência;
        /// <summary>
        /// Array com as estratigrafias do poço.
        /// </summary>
        public EstratigrafiaChild[] Estratigrafias { get; set; }
    }
}
