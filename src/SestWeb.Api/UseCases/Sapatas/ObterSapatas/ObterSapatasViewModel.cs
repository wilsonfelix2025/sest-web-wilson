using SestWeb.Domain.Entities.Poço.Sapatas;
using SestWeb.Domain.Helpers;

namespace SestWeb.Api.UseCases.Sapatas.ObterSapatas
{
    /// <summary>
    /// ViewModel do caso de uso Obter sapatas.
    /// </summary>
    public class ObterSapatasViewModel
    {
        /// <inheritdoc />
        public ObterSapatasViewModel(Sapata sapata)
        {
            ProfundidadeMedida = sapata.Pm;
            Diâmetro = new FractionalNumber(sapata.Diâmetro).StringResult;
        }

        /// <summary>
        /// Profundidade medida da sapata.
        /// </summary>
        public double ProfundidadeMedida { get; }

        /// <summary>
        /// Diâmetro da sapata.
        /// </summary>
        public string Diâmetro { get; }
    }
}
