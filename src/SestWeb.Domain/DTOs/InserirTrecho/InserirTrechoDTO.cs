using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Enums;
using System.Collections.Generic;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.DTOs.InserirTrecho
{
    public class InserirTrechoDTO
    {
        public TipoDeTrechoEnum TipoDeTrecho { get; set; }
        public List<string> LitologiasSelecionadas { get; set; }
        public PerfilBase PerfilSelecionado { get; set; }
        public double PmLimite { get; set; }
        public TipoTratamentoTrechoEnum TipoTratamentoTrecho { get; set; }
        public double BaseDeSedimentos { get; set; }
        public string NovoNome { get; set; }
        public double ValorTopo { get; set; }
        public double ÚltimoPontoTrajetória { get; set; }
        public IConversorProfundidade TrajetóriaPoço { get; set; }
        public Litologia LitologiaPoço { get; set; }
    }
}
