using System.Collections.Generic;
using System.Text.RegularExpressions;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.ObtidosNoDomínio;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.TiposVálidosCorrelação;
using SestWeb.Domain.Entities.Correlações.PerfisEntradaCorrelação;
using SestWeb.Domain.Entities.Correlações.TokensCorrelação;

namespace SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.AnalizadorExpressão
{
    public static class ExpressionAnalyser
    {
        #region Properties

        public static List<string> TiposPerfisVálidos => TiposVálidos.Perfis;

        #endregion

        #region Methods

        public static bool OcorreCálculoPorGrupoLitológico(string expression)
        {
            if (expression == null)
            {
                //TODO (Vanessa Chalub) Verificar pq está caindo nesse metodo apos devolver a criação do calculo de perfis.
                return false;
                //throw new ArgumentException($"Expressão não inicializada.");
            }
            return new Regex($@"\b{Tokens.TokenDeGrupoLitológico}\b").IsMatch(expression);
        }

        public static bool OcorreCálculoApartirDeProfundidadeInicial(string expression)
        {
            if (expression == null)
            {
                //TODO (Vanessa Chalub) Verificar pq está caindo nesse metodo apos devolver a criação do calculo de perfis.
                return false;
                //throw new ArgumentException($"Expressão não inicializada.");
            }
            return new Regex($@"\b{Tokens.TokenDeProfundidadeInicial}\b").IsMatch(expression);
        }

        public static bool OcorreCálculoComStepFixo(string expression)
        {
            if (expression == null)
            {
                //TODO (Vanessa Chalub) Verificar pq está caindo nesse metodo apos devolver a criação do calculo de perfis.
                return false;
                //throw new ArgumentException($"Expressão não inicializada.");
            }
            return new Regex($@"\b{Tokens.TokenDeCálculoComStepFixo}\b").IsMatch(expression);
        }

        public static bool OcorreCálculoComDensidadeÁguaDoMar(string expression)
        {
            if (expression == null)
            {
                //TODO (Vanessa Chalub) Verificar pq está caindo nesse metodo apos devolver a criação do calculo de perfis.
                return false;
                //throw new ArgumentException($"Expressão não inicializada.");
            }
            return new Regex($@"\b{Tokens.TokenDeDensidadeÁguaMar}\b").IsMatch(expression);
        }

        public static bool OcorreCálculoComCategoriaDoPoço(string expression)
        {
            if (expression == null)
            {
                //TODO (Vanessa Chalub) Verificar pq está caindo nesse metodo apos devolver a criação do calculo de perfis.
                return false;
                //throw new ArgumentException($"Expressão não inicializada.");
            }
            return new Regex($@"\b{Tokens.TokenDeCategoriaDoPoco}\b").IsMatch(expression);
        }

        public static bool OcorreCálculoComPoçoOffShore(string expression)
        {
            if (expression == null)
            {
                //TODO (Vanessa Chalub) Verificar pq está caindo nesse metodo apos devolver a criação do calculo de perfis.
                return false;
                //throw new ArgumentException($"Expressão não inicializada.");
            }
            return new Regex($@"\b{Tokens.TokenDePoçoOffShore}\b").IsMatch(expression);
        }

        public static bool OcorreCálculoComPoçoOnShore(string expression)
        {
            if (expression == null)
            {
                //TODO (Vanessa Chalub) Verificar pq está caindo nesse metodo apos devolver a criação do calculo de perfis.
                return false;
                //throw new ArgumentException($"Expressão não inicializada.");
            }
            return new Regex($@"\b{Tokens.TokenDePoçoOnShore}\b").IsMatch(expression);
        }

        public static bool OcorreCálculoComMesaRotativa(string expression)
        {
            if (expression == null)
            {
                //TODO (Vanessa Chalub) Verificar pq está caindo nesse metodo apos devolver a criação do calculo de perfis.
                return false;
                //throw new ArgumentException($"Expressão não inicializada.");
            }
            return new Regex($@"\b{Tokens.TokenDeMesaRotativa}\b").IsMatch(expression);
        }

        public static bool OcorreCálculoComComLâminaDAgua(string expression)
        {
            if (expression == null)
            {
                //TODO (Vanessa Chalub) Verificar pq está caindo nesse metodo apos devolver a criação do calculo de perfis.
                return false;
                //throw new ArgumentException($"Expressão não inicializada.");
            }
            return new Regex($@"\b{Tokens.TokenDeLâminaDAgua}\b").IsMatch(expression);
        }

        public static bool OcorreCálculoComRhobInicial(string expression)
        {
            if (expression == null)
            {
                //TODO (Vanessa Chalub) Verificar pq está caindo nesse metodo apos devolver a criação do calculo de perfis.
                return false;
                //throw new ArgumentException($"Expressão não inicializada.");
            }
            return new Regex($@"\b{Tokens.TokenDeRhobInicial}\b").IsMatch(expression);
        }

        public static bool OcorreCálculoComAlturaDeAntepoço(string expression)
        {
            if (expression == null)
            {
                //TODO (Vanessa Chalub) Verificar pq está caindo nesse metodo apos devolver a criação do calculo de perfis.
                return false;
                //throw new ArgumentException($"Expressão não inicializada.");
            }
            return new Regex($@"\b{Tokens.TokenDeAlturaDeAntepoço}\b").IsMatch(expression);
        }

        public static bool OcorreCálculoPorTrecho(string expression)
        {
            if (expression == null)
            {
                //TODO (Vanessa Chalub) Verificar pq está caindo nesse metodo apos devolver a criação do calculo de perfis.
                return false;
                //throw new ArgumentException($"Expressão não inicializada.");
            }
            return new Regex($@"\b{Tokens.TokenDeCálculoPorTrecho}\b").IsMatch(expression);
        }

        public static bool OcorrePerfisQuePodemSerAdquiridosNoDomínio(string expression)
        {
            if (expression == null)
            {
                //TODO (Vanessa Chalub) Verificar pq está caindo nesse metodo apos devolver a criação do calculo de perfis.
                return false;
                //throw new ArgumentException($"Expressão não inicializada.");
            }

            foreach (var perfilDoDomínio in ObterPerfisQuePodemSerAdquiridosNoDomínio())
            {
                if (new Regex($@"\b{perfilDoDomínio}\b").IsMatch(expression))
                    return true;
            }

            return false;
        }

        public static List<string> ObterPerfisQuePodemSerAdquiridosNoDomínio()
        {
            return PerfisObtidosNoDomínio.Perfis();
        }

        public static List<string> ObterPerfisQuePodemSerAdquiridosNoDomínioPresentesNaCorrelação(string expression)
        {
            if (expression == null)
            {
                return null;
            }

            if (!OcorrePerfisQuePodemSerAdquiridosNoDomínio(expression)) return null;

            List<string> perfisQuePodemSerAdquiridosNoDomínio = new List<string>();

            foreach (var perfilDoDomínio in ObterPerfisQuePodemSerAdquiridosNoDomínio())
            {
                if (new Regex($@"\b{perfilDoDomínio}\b").IsMatch(expression))
                {
                    perfisQuePodemSerAdquiridosNoDomínio.Add(perfilDoDomínio);
                }
            }

            return perfisQuePodemSerAdquiridosNoDomínio;
        }

        // TODO(RCM): Falta implementação de identificação genérica
        // Todo(RCM): Testar se o perfil de entrada é necessário somente num dos lados de uma atribuição através de um operador ternario. Exemplo: (RHOB > 0) PORO = RHOB + 1 : PORO = DTS
        public static bool OcorrePerfilDeEntradaQueSóApareceDeUmLadoDeUmaAtribuiçãoCondicionalPorOperadorTernário(string expression)
        {
            if (expression == null)
            {
                //TODO (Vanessa Chalub) Verificar pq está caindo nesse metodo apos devolver a criação do calculo de perfis.
                return false;
                //throw new ArgumentException($"Expressão não inicializada.");
            }

            foreach (var perfil in PerfisEntrada.Identify(expression))
            {
                if (new Regex($@"\(\s*\b{perfil}\b\s*[=><]+\s*\w\)\s*\?").IsMatch(expression))
                {
                    return true;
                }
            }

            return false;
        }

        public static List<string> ObterPerfisDeEntradaQueSóApareceDeUmLadoDeUmaAtribuiçãoCondicionalPorOperadorTernário(string expression)
        {
            if (expression == null)
            {
                //TODO (Vanessa Chalub) Verificar pq está caindo nesse metodo apos devolver a criação do calculo de perfis.
                return null;
                //throw new ArgumentException($"Expressão não inicializada.");
            }

            List<string> perfis = new List<string>();

            foreach (var perfil in PerfisEntrada.Identify(expression))
            {
                if (new Regex($@"\(\s*\b{perfil}\b\s*[=><]+\s*\w\)\s*\?").IsMatch(expression))
                {
                    perfis.Add(perfil);
                }
            }

            return perfis;
        }

        #endregion
    }
}
