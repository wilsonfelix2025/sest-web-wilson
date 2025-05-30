using System.Collections.Generic;
using SestWeb.Domain.Enums;

namespace SestWeb.Api.UseCases.InserirTrecho
{
    public class InserirTrechoRequest
    {
        public string PerfilId { get; set; }
        public string NomeNovoPerfil { get; set; }
        public TipoDeTrechoEnum TipoDeTrecho { get; set; }
        public TipoTratamentoTrechoEnum TipoTratamento { get; set; }
        public double PMLimite { get; set; }
        public List<string> LitologiasSelecionadas { get; set; }
    }
}
