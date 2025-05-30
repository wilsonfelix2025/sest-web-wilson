using System;
using System.ComponentModel;

namespace SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper
{
    public class MuParserNetWrapper : IDisposable
    {
        private bool _disposed = false;

        public MuParserNetWrapper(string nome)
        {
            Nome = nome;
            Init();
        }
        public string Nome { get; private set; }

        public Parser Parser { get; private set; } // TODO(RCM): Remover a exposição desse cara

        private void Init()
        {
            Parser = new Parser();
        }

        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).

                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                //Parser.Dispose();
                Parser = null;
                _disposed = true;
            }
        }

        // Use C# destructor syntax for finalization code.
        ~MuParserNetWrapper()
        {
            // Simply call Dispose(false).
            //Dispose(false);
            //var msg = $"[---Parser---] {Nome} ({GetType().Name}) ({GetHashCode()}) Finalized";
            //Debug.WriteLine(msg);
        }

        public enum ParserErrorCodes
        {
            [Description("Operador binário não esperado")]
            UNEXPECTED_OPERATOR,
            [Description("Token não identificado")]
            UNASSIGNABLE_TOKEN,
            [Description("Fim de fórmula não esperado")]
            UNEXPECTED_EOF,
            [Description("Separador de argumentos não esperado foi encontrado")]
            UNEXPECTED_ARG_SEP,
            [Description("Argumento não esperado foi encontrado")]
            UNEXPECTED_ARG,
            [Description("Um token de valor inesperado foi encontrado")]
            UNEXPECTED_VAL,
            [Description("Uma variável não esperada foi encontrada")]
            UNEXPECTED_VAR,
            [Description("Parênteses não esperado, abrindo ou fechando")]
            UNEXPECTED_PARENS,
            [Description("Uma string foi encontrada em uma posição inapropriada")]
            UNEXPECTED_STR,
            [Description("A função de string foi chamada com um tipo diferente de argumento")]
            STRING_EXPECTED,
            [Description("Uma função numérica foi chamado com um tipo não numérico como argumento")]
            VAL_EXPECTED,
            [Description("Faltando parênteses")]
            MISSING_PARENS,
            [Description("Função inesperada encontrada")]
            UNEXPECTED_FUN,
            [Description("Constante string não terminada")]
            UNTERMINATED_STRING,
            [Description("Muitos parâmetros de função")]
            TOO_MANY_PARAMS,
            [Description("Poucos parâmetros de função")]
            TOO_FEW_PARAMS,
            [Description("Operadores binários só podem ser aplicados a itens numéricos do mesmo tipo")]
            OPRT_TYPE_CONFLICT,
            [Description("Resultado é uma string")]
            STR_RESULT,
            [Description("Nome inválido")]
            INVALID_NAME,
            [Description("Identificador de operador binário inválido")]
            INVALID_BINOP_IDENT,
            [Description("Identificador de operador infixo inválido")]
            INVALID_INFIX_IDENT,
            [Description("Identificador de operador de sufixo inválido")]
            INVALID_POSTFIX_IDENT,
            [Description("Tentando sobrecarregar o operador padrão")]
            BUILTIN_OVERLOAD,
            [Description("Invalid callback function pointer")]
            INVALID_FUN_PTR,
            [Description("Invalid variable pointer")]
            INVALID_VAR_PTR,
            [Description("A expressão está vazia")]
            EMPTY_EXPRESSION,
            [Description("Conflito de nome")]
            NAME_CONFLICT,
            [Description("Prioridade de Operador inválido")]
            OPT_PRI,
            [Description("Divisão por zero detectada, sqrt(-1), log(0)")]
            DOMAIN_ERROR,
            [Description("Divisão por zero detectada")]
            DIV_BY_ZERO,
            [Description("Error that does not fit any other code but is not an internal error")]
            GENERIC,
            [Description("Conflict with current locale")]
            LOCALE,
            [Description("Operador if then else não esperado")]
            UNEXPECTED_CONDITIONAL,
            [Description("Else não encontrado")]
            MISSING_ELSE_CLAUSE,
            [Description("Misplaced colon")]
            MISPLACED_COLON,
            [Description("Erro interno")]
            INTERNAL_ERROR
        }
    }
}
