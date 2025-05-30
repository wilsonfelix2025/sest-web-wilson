﻿namespace SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper
{
    /// <summary>
    /// Error codes.
    /// </summary>
    public enum ErrorCodes
    {
        /// <summary>
        /// Unexpected binary operator found
        /// </summary>
        UNEXPECTED_OPERATOR = 0,

        /// <summary>
        /// Token cant be identified.
        /// </summary>
        UNASSIGNABLE_TOKEN = 1,

        /// <summary>
        /// Unexpected end of formula. (Example: "2+sin(")
        /// </summary>
        UNEXPECTED_EOF = 2,

        /// <summary>
        /// An unexpected comma has been found. (Example: "1,23")
        /// </summary>
        UNEXPECTED_ARG_SEP = 3,

        /// <summary>
        /// An unexpected argument has been found
        /// </summary>
        UNEXPECTED_ARG = 4,

        /// <summary>
        /// An unexpected value token has been found
        /// </summary>
        UNEXPECTED_VAL = 5,

        /// <summary>
        /// An unexpected variable token has been found
        /// </summary>
        UNEXPECTED_VAR = 6,

        /// <summary>
        /// Unexpected Parenthesis, opening or closing
        /// </summary>
        UNEXPECTED_PARENS = 7,

        /// <summary>
        /// A string has been found at an inapropriate position
        /// </summary>
        UNEXPECTED_STR = 8,

        /// <summary>
        /// A string function has been called with a different type of argument
        /// </summary>
        STRING_EXPECTED = 9,

        /// <summary>
        /// A numerical function has been called with a non value type of argument
        /// </summary>
        VAL_EXPECTED = 10,

        /// <summary>
        /// Missing parens. (Example: "3*sin(3")
        /// </summary>
        MISSING_PARENS = 11,

        /// <summary>
        /// Unexpected function found. (Example: "sin(8)cos(9)")
        /// </summary>
        UNEXPECTED_FUN = 12,

        /// <summary>
        /// Unterminated string constant. (Example: "3*valueof("hello)")
        /// </summary>
        UNTERMINATED_STRING = 13,

        /// <summary>
        /// Too many function parameters
        /// </summary>
        TOO_MANY_PARAMS = 14,

        /// <summary>
        /// Too few function parameters. (Example: "ite(1&lt;2,2)")
        /// </summary>
        TOO_FEW_PARAMS = 15,

        /// <summary>
        /// binary operators may only be applied to value items of the same type
        /// </summary>
        OPRT_TYPE_CONFLICT = 16,

        /// <summary>
        /// result is a string
        /// </summary>
        STR_RESULT = 17,

        // Invalid Parser input Parameters

        /// <summary>
        /// Invalid function, variable or constant name.
        /// </summary>
        INVALID_NAME = 18,

        /// <summary>
        /// Invalid binary operator identifier
        /// </summary>
        INVALID_BINOP_IDENT = 19,

        /// <summary>
        /// Invalid function, variable or constant name.
        /// </summary>
        INVALID_INFIX_IDENT = 20,

        /// <summary>
        /// Invalid function, variable or constant name.
        /// </summary>
        INVALID_POSTFIX_IDENT = 21,

        /// <summary>
        /// Trying to overload builtin operator
        /// </summary>
        BUILTIN_OVERLOAD = 22,

        /// <summary>
        /// Invalid callback function pointer 
        /// </summary>
        INVALID_FUN_PTR = 23,

        /// <summary>
        /// Invalid variable pointer 
        /// </summary>
        INVALID_VAR_PTR = 24,

        /// <summary>
        /// The Expression is empty
        /// </summary>
        EMPTY_EXPRESSION = 25,

        /// <summary>
        /// Name conflict
        /// </summary>
        NAME_CONFLICT = 26,

        /// <summary>
        /// Invalid operator priority
        /// </summary>
        OPT_PRI = 27,

        /// <summary>
        /// Catch division by zero, sqrt(-1), log(0) (currently unused)
        /// </summary>
        DOMAIN_ERROR = 28,

        /// <summary>
        /// Division by zero (currently unused)
        /// </summary>
        DIV_BY_ZERO = 29,

        /// <summary>
        /// Generic error
        /// </summary>
        GENERIC = 30,

        /// <summary>
        /// Conflict with current locale
        /// </summary>
        LOCALE = 31,

        /// <summary>
        /// Unexpected conditional
        /// </summary>
        UNEXPECTED_CONDITIONAL = 32,

        /// <summary>
        /// Missing the else clause
        /// </summary>
        MISSING_ELSE_CLAUSE = 33,

        /// <summary>
        /// Misplaced colon
        /// </summary>
        MISPLACED_COLON = 34,

        // internal errors

        /// <summary>
        /// Internal error of any kind.
        /// </summary>
        INTERNAL_ERROR = 35,

        // The last two are special entries 

        /// <summary>
        /// This is no error code, It just stores just the total number of error codes
        /// </summary>
        COUNT,

        /// <summary>
        /// Undefined message, placeholder to detect unassigned error messages
        /// </summary>
        UNDEFINED = -1
    }
}
