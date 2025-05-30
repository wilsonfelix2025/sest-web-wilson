using System;
using System.Collections.Generic;
using System.Linq;

namespace SestWeb.Domain.Entities.Correlações.TokensCorrelação
{
    public static class Tokens
    {

        #region Properties

        public static string TokenDeGrupoLitológico => ObterTokenDeGrupoLitológico();

        public static string TokenDeCálculoPorTrecho => ObterTokenDeCálculoPorTrecho();

        public static string TokenDeProfundidadeInicial => ObterTokenDeProfundidadeInicial();

        public static string TokenDeCálculoComStepFixo => ObterTokenDeCálculoComStepFixo();

        public static string TokenDeDensidadeÁguaMar => ObterTokenDeDensidadeÁguaMar();

        public static string TokenDeCategoriaDoPoco => ObterTokenDeCategoriaDoPoco();

        public static string TokenDePoçoOffShore => ObterTokenDePoçoOffShore();

        public static string TokenDePoçoOnShore => ObterTokenDePoçoOnShore();

        public static string TokenDeMesaRotativa => ObterTokenDeMesaRotativa();

        public static string TokenDeLâminaDAgua => ObterTokenDeLâminaDAgua();

        public static string TokenDeAlturaDeAntepoço => ObterTokenDeAlturaDeAntepoço();

        public static string TokenDeRhobInicial => ObterTokenDeRhobInicial();

        #endregion

        #region Methods

        public static List<string> GetAll()
        {
            return Enum.GetNames(typeof(TokensEnum)).ToList();
        }

        private static string ObterTokenDeGrupoLitológico()
        {
            return TokensEnum.GRUPO_LITOLOGICO.ToString();
        }

        private static string ObterTokenDeCálculoPorTrecho()
        {
            return TokensEnum.PROFUNDIDADE.ToString();
        }

        private static string ObterTokenDeProfundidadeInicial()
        {
            return TokensEnum.PROFUNDIDADE_INICIAL.ToString();
        }

        private static string ObterTokenDeCálculoComStepFixo()
        {
            return TokensEnum.STEP.ToString();
        }

        private static string ObterTokenDeDensidadeÁguaMar()
        {
            return TokensEnum.DENSIDADE_AGUA_MAR.ToString();
        }

        private static string ObterTokenDeCategoriaDoPoco()
        {
            return TokensEnum.CATEGORIA_POCO.ToString();
        }

        private static string ObterTokenDePoçoOffShore()
        {
            return TokensEnum.OFFSHORE.ToString();
        }

        private static string ObterTokenDePoçoOnShore()
        {
            return TokensEnum.ONSHORE.ToString();
        }

        private static string ObterTokenDeMesaRotativa()
        {
            return TokensEnum.MESA_ROTATIVA.ToString();
        }

        private static string ObterTokenDeLâminaDAgua()
        {
            return TokensEnum.LAMINA_DAGUA.ToString();
        }

        private static string ObterTokenDeAlturaDeAntepoço()
        {
            return TokensEnum.ALTURA_ANTEPOCO.ToString();
        }

        private static string ObterTokenDeRhobInicial()
        {
            return TokensEnum.RHOB_INICIAL.ToString();
        }

        #endregion
    }
}
