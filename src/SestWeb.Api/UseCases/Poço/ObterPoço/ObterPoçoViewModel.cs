using SestWeb.Application.UseCases.PoçoUseCases.ObterPoço;

namespace SestWeb.Api.UseCases.Poço.ObterPoço
{
    /// <summary>
    /// ViewModel do caso de uso Obter poço.
    /// </summary>
    public class ObterPoçoViewModel
    {
        /// <inheritdoc />
        public ObterPoçoViewModel(ObterPoçoOutput poçoOutput)
        {
            Id = poçoOutput.Poço.Id.ToString();
            Nome = poçoOutput.Poço.Nome;
            TipoPoço = poçoOutput.Poço.TipoPoço.ToString();
        }

        /// <summary>
        /// Id do poço.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Nome do poço.
        /// </summary>
        public string Nome { get; }

        /// <summary>
        /// Tipo do poço.
        /// </summary>
        public string TipoPoço { get; }
    }
}
