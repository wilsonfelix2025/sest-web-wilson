using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas
{
    public class CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas
    {
        public CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas(GrupoLitologico grupoLitológico, Correlação ucs, Correlação coesa, Correlação angat, Correlação restr, Correlação biot = null)
        {
            GrupoLitológico = grupoLitológico;
            Ucs = ucs;
            Coesa = coesa;
            Angat = angat;
            Restr = restr;
            Biot = Biot;
        }

        public GrupoLitologico GrupoLitológico { get; private set; }
        public Correlação Ucs { get; private set; }
        public Correlação Coesa { get; private set; }
        public Correlação Angat { get; private set; }
        public Correlação Restr { get; private set; }
        public Correlação Biot { get; private set; }
    }
}
