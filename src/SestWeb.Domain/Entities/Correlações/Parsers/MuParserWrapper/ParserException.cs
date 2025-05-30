using System;

namespace SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper
{
    /// <summary>
    /// Error class of the parser.
    /// </summary>
    public class ParserException : Exception
    {
        /// <summary>
        /// Gets or sets the expression with error.
        /// </summary>
        public string Expr { get; set; }

        /// <summary>
        /// Gets or sets the invalid token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the position of the error in expression.
        /// </summary>
        public int Pos { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public ErrorCodes Code { get; set; }

        /// <summary>
        /// Class constructor.
        /// </summary>
        public ParserException()
        {
        }

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="expr">The expression with error</param>
        /// <param name="token">The invalid token</param>
        /// <param name="pos">The position of the error in expression</param>
        /// <param name="code">The error code</param>
        public ParserException(string message,
            string expr,
            string token,
            int pos,
            ErrorCodes code)
            : base(message)
        {
            this.Expr = expr;
            this.Token = token;
            this.Pos = pos;
            this.Code = code;
        }
    }
}
