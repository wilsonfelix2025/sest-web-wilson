using System.Collections.Generic;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.AnalizadorExpressão;

namespace SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.AnaliticsCorrelação
{
    public class Analitics : IAnalitics
    {
        private readonly string _expressãoBruta;

        public Analitics(string expressão)
        {
            _expressãoBruta = expressão;
        }

        #region Properties

        public List<string> PerfisQuePodemSerAdquiridosNoDomínioPresentesNaCorrelação => ExpressionAnalyser.ObterPerfisQuePodemSerAdquiridosNoDomínioPresentesNaCorrelação(_expressãoBruta);

        public List<string> PerfisDeEntradaQueSóApareceDeUmLadoDeUmaAtribuiçãoCondicionalPorOperadorTernário => ExpressionAnalyser.ObterPerfisDeEntradaQueSóApareceDeUmLadoDeUmaAtribuiçãoCondicionalPorOperadorTernário(_expressãoBruta);

        public List<string> PerfisQuePodemSerAdquiridosNoDomínio => ExpressionAnalyser.ObterPerfisQuePodemSerAdquiridosNoDomínio();

        public bool TemCálculoPorGrupoLitológico => ExpressionAnalyser.OcorreCálculoPorGrupoLitológico(_expressãoBruta);

        public bool TemCálculoApartirDeProfundidadeInicial => ExpressionAnalyser.OcorreCálculoApartirDeProfundidadeInicial(_expressãoBruta);

        public bool TemCálculoComStepFixo => ExpressionAnalyser.OcorreCálculoComStepFixo(_expressãoBruta);

        public bool TemCálculoComRhobInicial => ExpressionAnalyser.OcorreCálculoComRhobInicial(_expressãoBruta);

        public bool TemCálculoComAlturaDeAntepoço => ExpressionAnalyser.OcorreCálculoComAlturaDeAntepoço(_expressãoBruta);

        public bool TemCálculoComMesaRotativa => ExpressionAnalyser.OcorreCálculoComMesaRotativa(_expressãoBruta);

        public bool TemCálculoComDensidadeAguaDoMar => ExpressionAnalyser.OcorreCálculoComDensidadeÁguaDoMar(_expressãoBruta);

        public bool TemCálculoComCategoriaDoPoco => ExpressionAnalyser.OcorreCálculoComCategoriaDoPoço(_expressãoBruta);

        public bool TemCálculoComPoçoOffShore => ExpressionAnalyser.OcorreCálculoComPoçoOffShore(_expressãoBruta);

        public bool TemCálculoComPoçoOnShore => ExpressionAnalyser.OcorreCálculoComPoçoOnShore(_expressãoBruta);

        public bool TemCálculoComLâminaDAgua => ExpressionAnalyser.OcorreCálculoComComLâminaDAgua(_expressãoBruta);

        public bool TemCálculoPorTrecho => ExpressionAnalyser.OcorreCálculoPorTrecho(_expressãoBruta);

        public bool TemPerfisQuePossamSerAdquiridosNoDomínio => ExpressionAnalyser.OcorrePerfisQuePodemSerAdquiridosNoDomínio(_expressãoBruta);

        public bool TemPerfilDeEntradaQueSóApareceDeUmLadoDeUmaAtribuiçãoCondicionalPorOperadorTernário => ExpressionAnalyser.OcorrePerfilDeEntradaQueSóApareceDeUmLadoDeUmaAtribuiçãoCondicionalPorOperadorTernário(_expressãoBruta);

        #endregion
    }
}
