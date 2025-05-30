using System;
using System.Text.RegularExpressions;
using SestWeb.Domain.Entities.Correlações.TokensCorrelação;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Domain.Entities.Correlações.NormalizadorCorrelação
{
    public static class Normalizador
    {
        #region Methods

        public static string NormalizarExpressão(string expression )
        {
            var expressãoNãoNormalizada = expression;
            expressãoNãoNormalizada = NormalizarGrupoLitológico(expressãoNãoNormalizada);
            expressãoNãoNormalizada = NormalizarConstantesEVariáveis(expressãoNãoNormalizada);
            return expressãoNãoNormalizada;
        }

        private static string NormalizarConstantesEVariáveis(string expressãoNãoNormalizada)
        {
            var pattern = @"(\bconst\b |\bvar\b)";
            return Regex.Replace(expressãoNãoNormalizada, pattern, string.Empty);
        }

        private static string NormalizarGrupoLitológico(string expression)
        {
            var expressãoNãoNormalizada = expression;

            if (OcorreCálculoPorGrupoLitológico(expression))
            {
                var gruposLitológicos = GrupoLitologico.GetNames();
                foreach (var grupo in gruposLitológicos)
                {
                    if (new Regex($@"\b{grupo}\b").IsMatch(expressãoNãoNormalizada))
                    {
                        var glValue = gruposLitológicos.IndexOf(grupo); // Todo(RCM): testar isso8u7
                        expressãoNãoNormalizada = Regex.Replace(expressãoNãoNormalizada, $@"\b{grupo}\b",
                            glValue.ToString());
                    }
                }
            }
            return expressãoNãoNormalizada;
        }

        private static bool OcorreCálculoPorGrupoLitológico(string expression)
        {
            if (expression == null)
            {
                throw new ArgumentException($"Expressão não inicializada.");
            }
            return new Regex($@"\b{Tokens.TokenDeGrupoLitológico}\b").IsMatch(expression);
        }

        #endregion
    }
}
