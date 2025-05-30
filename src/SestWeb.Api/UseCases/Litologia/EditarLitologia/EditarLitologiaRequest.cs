using SestWeb.Domain.Entities.LitologiaDoPoco;
using System.Collections.Generic;

namespace SestWeb.Api.UseCases.Litologia.EditarLitologia
{
    /// <summary>
    /// Dados para edição de uma litologia.
    /// </summary>
    public class EditarLitologiaRequest
    {
        /// <summary>
        /// Tipo de litologia: "Prevista" ou "Adaptada".
        /// </summary>
        public string TipoLitologia { get; set; }

        public Dictionary<string, string>[] Pontos { get; set; }
    }
}
