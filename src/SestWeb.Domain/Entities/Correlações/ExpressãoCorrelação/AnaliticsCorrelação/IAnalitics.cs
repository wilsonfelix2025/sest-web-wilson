using System.Collections.Generic;

namespace SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.AnaliticsCorrelação
{
    public interface IAnalitics 
    {
        List<string> PerfisQuePodemSerAdquiridosNoDomínioPresentesNaCorrelação { get; }

        List<string> PerfisDeEntradaQueSóApareceDeUmLadoDeUmaAtribuiçãoCondicionalPorOperadorTernário { get; }

        List<string> PerfisQuePodemSerAdquiridosNoDomínio { get; }

        bool TemCálculoPorGrupoLitológico { get; }

        bool TemCálculoApartirDeProfundidadeInicial { get; }

        bool TemCálculoComStepFixo { get; }

        bool TemCálculoComRhobInicial { get; }

        bool TemCálculoComAlturaDeAntepoço { get; }

        bool TemCálculoComMesaRotativa { get; }

        bool TemCálculoComDensidadeAguaDoMar { get; }

        bool TemCálculoComCategoriaDoPoco { get; }

        bool TemCálculoComPoçoOffShore { get; }

        bool TemCálculoComPoçoOnShore { get; }

        bool TemCálculoComLâminaDAgua { get; }

        bool TemCálculoPorTrecho { get; }

        bool TemPerfisQuePossamSerAdquiridosNoDomínio { get; }

        bool TemPerfilDeEntradaQueSóApareceDeUmLadoDeUmaAtribuiçãoCondicionalPorOperadorTernário { get; }
    }
}
