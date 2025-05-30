using System.Collections.Generic;

namespace SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.AnalizadorExpressão
{
    public interface IExpressionAnalyser
    {
        List<string> TiposPerfisVálidos { get; }

        bool OcorreCálculoPorGrupoLitológico(string expression);

        bool OcorreCálculoApartirDeProfundidadeInicial(string expression);

        bool OcorreCálculoComStepFixo(string expression);

        bool OcorreCálculoComDensidadeÁguaDoMar(string expression);

        bool OcorreCálculoComCategoriaDoPoço(string expression);

        bool OcorreCálculoComPoçoOffShore(string expression);

        bool OcorreCálculoComPoçoOnShore(string expression);

        bool OcorreCálculoComMesaRotativa(string expression);

        bool OcorreCálculoComComLâminaDAgua(string expression);

        bool OcorreCálculoComRhobInicial(string expression);

        bool OcorreCálculoComAlturaDeAntepoço(string expression);

        bool OcorreCálculoPorTrecho(string expression);

        bool OcorrePerfisQuePodemSerAdquiridosNoDomínio(string expression);

        List<string> ObterPerfisQuePodemSerAdquiridosNoDomínio();

        List<string> ObterPerfisQuePodemSerAdquiridosNoDomínioPresentesNaCorrelação(string expression);

        // TODO(RCM): Falta implementação de identificação genérica
        // Todo(RCM): Testar se o perfil de entrada é necessário somente num dos lados de uma atribuição através de um operador ternario. Exemplo: (RHOB > 0) PORO = RHOB + 1 : PORO = DTS
        bool OcorrePerfilDeEntradaQueSóApareceDeUmLadoDeUmaAtribuiçãoCondicionalPorOperadorTernário(string expression);

        List<string> ObterPerfisDeEntradaQueSóApareceDeUmLadoDeUmaAtribuiçãoCondicionalPorOperadorTernário(
            string expression);
    }
}
