using SestWeb.Application.UseCases.PoçoUseCases.ObterPoços;

namespace SestWeb.Api.UseCases.Poço.ObterPoços
{
    /// <summary>
    /// ViewModel do caso de uso Obter poço.
    /// </summary>
    public class ObterPoçosViewModel
    {
        /// <inheritdoc />
        public ObterPoçosViewModel(PoçoOutput poçoOutput)
        {
            Id = poçoOutput.Id;
            Nome = poçoOutput.Nome;
            TipoPoço = poçoOutput.TipoPoço.ToString();
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
