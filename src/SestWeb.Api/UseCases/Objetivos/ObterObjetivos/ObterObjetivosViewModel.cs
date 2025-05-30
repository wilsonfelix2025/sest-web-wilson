using SestWeb.Domain.Entities.Poço.Objetivos;

namespace SestWeb.Api.UseCases.Objetivos.ObterObjetivos
{
    /// <summary>
    /// ViewModel do caso de uso Obter objetivos.
    /// </summary>
    public class ObterObjetivosViewModel
    {
        /// <inheritdoc />
        public ObterObjetivosViewModel(Objetivo objetivo)
        {
            ProfundidadeMedida = objetivo.Pm;
            TipoObjetivo = objetivo.TipoObjetivo.ToString();
        }

        /// <summary>
        /// Profundidade medida.
        /// </summary>
        public double ProfundidadeMedida { get; }

        /// <summary>
        /// Tipo do objetivo.
        /// </summary>
        public string TipoObjetivo { get; }
    }
}
