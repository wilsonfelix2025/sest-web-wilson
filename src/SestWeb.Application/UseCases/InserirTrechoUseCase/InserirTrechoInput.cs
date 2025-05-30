
using SestWeb.Domain.Enums;
using System.Collections.Generic;

namespace SestWeb.Application.UseCases.InserirTrechoUseCase
{
    public class InserirTrechoInput
    {
        public TipoDeTrechoEnum TipoDeTrecho { get; set; }
        public TipoTratamentoTrechoEnum TipoTratamento { get; set; }
        public double PMLimite { get; set; }
        public List<string> LitologiasSelecionadas { get; set; }
        public string NomeNovoPerfil { get; set; }
    }
}
