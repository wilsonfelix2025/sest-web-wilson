
using System.Collections.Generic;
using SestWeb.Domain.Enums;

namespace SestWeb.Application.UseCases.PerfilUseCases.ObterQtdPontosPerfil
{
    public class ObterQtdPontosPerfilInput
    {
        public double PmLimite { get; set; }
        public List<string> LitologiasSelecionadas { get; set; }
        public TipoDeTrechoEnum TipoTrecho { get; set; }
    }
}
