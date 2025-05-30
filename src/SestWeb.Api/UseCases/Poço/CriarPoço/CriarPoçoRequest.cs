using SestWeb.Domain.Entities.DadosGeraisDoPoco;

namespace SestWeb.Api.UseCases.Poço.CriarPoço
{
    /// <summary>
    /// Dados de entrada para caso de uso Criar poço.
    /// </summary>
    public class CriarPoçoRequest
    {
        /// <summary>
        /// Nome do poço.
        /// </summary>
        public string Nome { get; set; }
        /// <summary>
        /// Tipo do poço. Deve ser uma das opções: Projeto, Monitoramento, Retroanalise.
        /// Caso não seja informado, será criado um poço do tipo Projeto.
        /// </summary>
        public TipoPoço TipoPoço { get; set; }
    }
}
