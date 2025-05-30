using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Domain.Entities.Cálculos.Base.TrechosDeCálculo
{
    public class TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico : ITrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico
    {
        public TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico(GrupoLitologico grupoLitológico, double topo, double basee, Correlação ucs, Correlação coesa, Correlação angat, Correlação restr, Correlação biot)
        {
            GrupoLitológico = grupoLitológico;
            Topo = topo;
            Base = basee;
            Ucs = ucs;
            Coesa = coesa;
            Angat = angat;
            Restr = restr;
            Biot = biot;
        }

        public GrupoLitologico GrupoLitológico { get; private set; }
        public double Topo { get; private set; }
        public double Base { get; private set; }
        public Correlação Ucs { get; private set; }
        public Correlação Coesa { get; private set; }
        public Correlação Angat { get; private set; }
        public Correlação Restr { get; private set; }
        public Correlação Biot { get; private set; }
    }
}
