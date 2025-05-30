using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec
{
    public class RelacionamentoPropMecEmCriação
    {
        public string GrupoLitológico { get; }
        public string NomeAutor { get; }
        public string ChaveAutor { get; }
        public Correlação CorrUcs { get; }
        public Correlação CorrCoesa { get; }
        public Correlação CorrAngat { get; }
        public Correlação CorrRestr { get; }
        public string Origem { get; }

        public RelacionamentoPropMecEmCriação(string grupoLitológico, string origem, string nomeAutor, string chaveAutor, Correlação corrUcs, Correlação corrCoesa, Correlação corrAngat, Correlação corrRestr)
        {
            GrupoLitológico = grupoLitológico;
            NomeAutor = nomeAutor;
            ChaveAutor = chaveAutor;
            CorrUcs = corrUcs;
            CorrCoesa = corrCoesa;
            CorrAngat = corrAngat;
            CorrRestr = corrRestr;
            Origem = origem;
        }
    }
}
