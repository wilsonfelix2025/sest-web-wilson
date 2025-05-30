﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper
{
    public class Parser
    {
        // handler para o parser
        private IntPtr parserHandler;

        #region Atributos de apoio as funções das variáveis

        // coleção com as variáveis
        private Dictionary<string, ParserVariable> vars;

        // função de identificação passada pelo usuário
        private FactoryFunction factoryFunc;

        // objeto da callback passada para a biblioteca do muParser
        private ParserCallback factoryCallback;

        // ponteiro do objeto passado pelo usuário
        private GCHandle ptrUserdata;

        #endregion

        #region Atributos de apoio as funções de identificação de tokens

        // armazena os delegates para eles não serem deletados
        private List<ParserCallback> identFunctionsCallbacks;

        #endregion

        #region Atributos de apoio as funções de definição de funções de cálculo

        // armazena os delegates para eles não serem deletados
        private Dictionary<string, ParserCallback> funcCallbacks;

        #endregion

        #region Atributos de apoio as funções de definição de operadores

        private Dictionary<string, ParserCallback> infixOprtCallbacks;
        private Dictionary<string, ParserCallback> postfixOprtCallbacks;
        private Dictionary<string, ParserCallback> oprtCallbacks;

        #endregion

        #region Propriedades

        /// <summary>
        /// Gets or sets the parser expression.
        /// </summary>
        public string Expr
        {
            get
            {
                return Marshal.PtrToStringAnsi(MuParserLibrary.mupGetExpr(this.parserHandler));
            }
            set
            {
                MuParserLibrary.mupSetExpr(this.parserHandler, value ?? string.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the list of valid chars to be used in variables names.
        /// </summary>
        /// <remarks>The get function of this property is not supported if using the original
        /// muParser library instead of the Sest-compatible library (which
        /// is available at Sest repository.</remarks>
        public string NameChars
        {
            get
            {
                // checa se a biblioteca do muParser é compatível
                this.CheckLibraryVersion();

                return Marshal.PtrToStringAnsi(MuParserLibrary.mupValidNameChars(this.parserHandler));
            }
            set
            {
                MuParserLibrary.mupDefineNameChars(this.parserHandler, value ?? string.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the list of valid chars to be used as binary operators.
        /// </summary>
        /// <remarks>The get function of this property is not supported if using the original
        /// muParser library instead of the Sest-compatible library (which
        /// is available at Sest repository.</remarks>
        public string OprtChars
        {
            get
            {
                // checa se a biblioteca do muParser é compatível
                this.CheckLibraryVersion();

                return Marshal.PtrToStringAnsi(MuParserLibrary.mupValidOprtChars(this.parserHandler));
            }
            set
            {
                MuParserLibrary.mupDefineOprtChars(this.parserHandler, value ?? string.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the list of valid chars to be used as unary infix operators.
        /// </summary>
        /// <remarks>The get function of this property is not supported if using the original
        /// muParser library instead of the Sest-compatible library (which
        /// is available at Sest repository.</remarks>
        public string InfixOprtChars
        {
            get
            {
                // checa se a biblioteca do muParser é compatível
                this.CheckLibraryVersion();

                return Marshal.PtrToStringAnsi(MuParserLibrary.mupValidInfixOprtChars(this.parserHandler));
            }
            set
            {
                MuParserLibrary.mupDefineInfixOprtChars(this.parserHandler, value ?? string.Empty);
            }
        }

        /// <summary>
        /// Gets the list of available variables to be used in expressions.
        /// </summary>
        public Dictionary<string, ParserVariable> Vars
        {
            get
            {
                /*
		         * A princípio não é para ter variáveis a mais no parser do que tem
		         * nesta lista.
		         */
                return this.vars;
            }
        }

        /// <summary>
        /// Gets the list of available constants to be used in expressions.
        /// </summary>
        public Dictionary<string, double> Consts
        {
            get
            {
                // lista de consts
                var consts = new Dictionary<string, double>();

                int numConsts = MuParserLibrary.mupGetConstNum(this.parserHandler);
                for (int i = 0; i < numConsts; i++)
                {

                    /*
                     * A biblioteca do muParser conta com um buffer interno que
                     * é preenchido com o nome da constante. A função ajusta o
                     * ponteiro passado para apontar para esse buffer. A gerência
                     * deste buffer é feita pelo próprio muParser.
                     */
                    IntPtr buffConstName = new IntPtr();
                    double value = 0.0;
                    MuParserLibrary.mupGetConst(this.parserHandler, (uint)i, ref buffConstName, ref value);

                    string constName = Marshal.PtrToStringAnsi(buffConstName);

                    consts[constName] = value;
                }

                return consts;
            }
        }

        /// <summary>
        /// Gets the list of available functions to be used in expressions.
        /// </summary>
        public Dictionary<string, ParserCallback> Functions
        {
            get
            {
                return this.funcCallbacks;
            }
        }

        /// <summary>
		/// Gets the list of available unary infix operators to be used in expressions.
		/// </summary>
		public Dictionary<string, ParserCallback> InfixOprts
        {
            get
            {
                return this.infixOprtCallbacks;
            }
        }

        /// <summary>
        /// Gets the list of available unary postfix operators to be used in expressions.
        /// </summary>
        public Dictionary<string, ParserCallback> PostfixOprts
        {
            get
            {
                return this.postfixOprtCallbacks;
            }
        }

        /// <summary>
        /// Gets the list of available binary operators to be used in expressions.
        /// </summary>
        public Dictionary<string, ParserCallback> Oprts
        {
            get
            {
                return this.oprtCallbacks;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ErrorFuncType ErrorFuncHandler { get; private set; }

        #endregion

        #region Funções

        /// <summary>
        /// Class constructor. It initialize the muParser structures.
        /// </summary>
        protected internal Parser()
        {
            // inicializa o parser
            this.parserHandler = MuParserLibrary.mupCreate(0);

            // inicializa o dicionário com as variáveis
            this.vars = new Dictionary<string, ParserVariable>();

            // inicializa as listas de delegates
            this.identFunctionsCallbacks = new List<ParserCallback>();
            this.funcCallbacks = new Dictionary<string, ParserCallback>();

            this.infixOprtCallbacks = new Dictionary<string, ParserCallback>();
            this.postfixOprtCallbacks = new Dictionary<string, ParserCallback>();
            this.oprtCallbacks = new Dictionary<string, ParserCallback>();

            // inicializa o delegate de factory
            this.factoryCallback = new ParserCallback(new IntFactoryFunction(this.VarFactoryCallback));

            // ajusta a função de tratamento de erros
            ErrorFuncHandler = this.ErrorHandler;
            MuParserLibrary.mupSetErrorHandler(this.parserHandler, ErrorFuncHandler);

            InitInternalFunctions();
        }

        private bool disposed = false;
        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                MuParserLibrary.mupRelease(this.parserHandler);
                disposed = true;
            }
        }

        // Use C# destructor syntax for finalization code.
        ~Parser()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }

        ///// <summary>
        ///// Class destructor.
        ///// </summary>
        //~Parser()
        //{
        //    // finaliza o parser
        //    MuParserLibrary.mupRelease(this.parserHandler);
        //}

        private void InitInternalFunctions()
        {
            //DefineFun("Σ", SUM); temporariamente desativado. Precisa implementar no c++
            DefineFun("INI", InitialValue);
        }

        private double InitialValue(string arg0)
        {
            if (arg0 == null)
            {
                throw new ParserException($"Variável {arg0} não reconhecida.", this.Expr, arg0, 0, ErrorCodes.STRING_EXPECTED);
            }

            ParserVariable variable;
            Vars.TryGetValue(arg0, out variable);
            if (variable == null)
            {
                throw new ParserException($"Variável {arg0} não reconhecida.", this.Expr, arg0, 0, ErrorCodes.UNEXPECTED_VAR);
            }
            if (variable.ValueArray.Length == 0)
            {
                throw new ParserException($"Variável {arg0} está vazia.", this.Expr, arg0, 0, ErrorCodes.VAL_EXPECTED);
            }
            return variable.ValueArray[0];
        }

        /// <summary>
        /// Sest exclusive Function
        /// </summary>
        /// <param name="bulkIndex"></param>
        /// <param name="threadIndex"></param>
        /// <param name="arg0"></param>
        /// <returns></returns>
        private double SUM(int bulkIndex, int threadIndex, string arg0)
        {
            if (arg0 == null)
            {
                throw new ParserException($"Variável {arg0} não reconhecida.", this.Expr, arg0, 0, ErrorCodes.STRING_EXPECTED);
            }

            double soma = 0.0;
            ParserVariable variable;
            Vars.TryGetValue(arg0, out variable);
            if (variable == null) return soma;

            object lockObject = new object();
            double sum = 0.0d;
            Parallel.For(1, bulkIndex + 1,
                () => 0.0d,

                (index, loopState, partialResult) =>
                {
                    return variable.ValueArray[index] + partialResult;
                },

                (localPartialSum) =>
                {
                    lock (lockObject)
                    {
                        sum += localPartialSum;
                    }
                });

            return sum;
        }

        /// <summary>
        /// Checks if the muParser library is compatible with Sest.
        /// If not, it throws an exception.
        /// </summary>
        private void CheckLibraryVersion()
        {
            // esta função não é suportada pela versão original do muParser
            if (!this.GetVersion().Contains("Sest"))
                throw new NotSupportedException("Esta função só é suportada pela versão customizada do muParser.");
        }

        /// <summary>
        /// Error handler. It loads the ParserError exception.
        /// </summary>
        private void ErrorHandler()
        {
            IntPtr ptrMessage = MuParserLibrary.mupGetErrorMsg(this.parserHandler);
            string message = Marshal.PtrToStringAnsi(ptrMessage);

            IntPtr ptrToken = MuParserLibrary.mupGetErrorToken(this.parserHandler);
            string token = Marshal.PtrToStringAnsi(ptrToken);

            string expr = this.Expr;
            ErrorCodes code = (ErrorCodes)MuParserLibrary.mupGetErrorCode(this.parserHandler);
            int pos = MuParserLibrary.mupGetErrorPos(this.parserHandler);

            // lança a exceção
            throw new ParserException(message, expr, token, pos, code);
        }

        /// <summary>
        /// Returns the muParser version.
        /// </summary>
        /// <returns>The string with the muParser library version. It will contain
        /// the '-Sest' string appended if compiled from Sest
        /// repository. If using the original muParser library, some functions
        /// will not work</returns>
        public string GetVersion()
        {
            IntPtr ptrVersion = MuParserLibrary.mupGetVersion(this.parserHandler);
            return Marshal.PtrToStringAnsi(ptrVersion);
        }

        /// <summary>
        /// Defines a parser variable.
        /// </summary>
        /// <param name="name">The variable name</param>
        /// <param name="var">The variable initial value</param>
        /// <returns>The parser variable reference</returns>
        /// <exception cref="ParserException">Throws if any parser error occurs</exception>
        public ParserVariable DefineVar(string name, double var)
        {
            // cria a variável
            ParserVariable parserVar = new ParserVariable(name, var);

            // ajusta a variável
            MuParserLibrary.mupDefineVar(this.parserHandler, name, parserVar.Pointer);

            // adiciona a variável na lista de variáveis
            this.vars[name] = parserVar;

            return parserVar;
        }

        /// <summary>
        /// Defines a parser variable.
        /// </summary>
        /// <param name="name">The variable name</param>
        /// <param name="var">A list of values. They will be used in bulk mode</param>
        /// <returns>The parser variable reference</returns>
        /// <exception cref="ParserException">Throws if any parser error occurs</exception>
        public ParserVariable DefineVar(string name, double[] var)
        {
            // cria a variável
            ParserVariable parserVar = new ParserVariable(name, var);

            // ajusta a variável
            MuParserLibrary.mupDefineVar(this.parserHandler, name, parserVar.Pointer);

            // adiciona a variável na lista de variáveis
            this.vars[name] = parserVar;

            return parserVar;
        }

        /// <summary>
        /// Removes a variable from parser if it exists. If not, nothing will be done.
        /// </summary>
        /// <param name="name">The variable name</param>
        public void RemoveVar(string name)
        {
            // remove a variável
            MuParserLibrary.mupRemoveVar(this.parserHandler, name);

            // e remove a variável da lista interna
            this.vars.Remove(name);
        }

        /// <summary>
        /// Removes all variables from parser.
        /// </summary>
        public void ClearVar()
        {
            // remove todas as variáveis
            MuParserLibrary.mupClearVar(this.parserHandler);
            this.vars.Clear();
        }

        /// <summary>
        /// Internal callback that will call the factory function.
        /// </summary>
        /// <param name="name">The new variable name</param>
        /// <param name="userData">A user defined context object</param>
        /// <returns>The variable pointer to muParser</returns>
        private IntPtr VarFactoryCallback(string name, IntPtr userData)
        {
            /*
             * Ignora o userdata da função e pega o objeto pinnado.
             */

            // pega o objeto
            object u = null;
            // ajusta apenas se o ponteiro estiver alocado
            if (this.ptrUserdata.IsAllocated)
                u = this.ptrUserdata.Target;

            // chama a função definida
            ParserVariable v = this.factoryFunc(name, u);
            // lança uma exceção se não for retornada uma variável
            if (v == null)
                throw new ParserException("Invalid var object", this.Expr, "", 0, ErrorCodes.INVALID_NAME);

            // adiciona na lista de variáveis
            this.vars[name] = v;

            // retorna o ponteiro para esta variável
            return v.Pointer;
        }

        /// <summary>
        /// Set a function that can create variable pointer for unknown expression variables.
        /// </summary>
        /// <param name="func">The variable factory function</param>
        /// <param name="userData">A user defined context object</param>
        public void SetVarFactory(FactoryFunction func, object userData = null)
        {
            // libera o objeto anterior caso estiver alocado
            if (this.ptrUserdata.IsAllocated)
                this.ptrUserdata.Free();

            // aloca o objeto do usuário para impedir que ele seja removido
            if (userData != null)
                this.ptrUserdata = GCHandle.Alloc(userData, GCHandleType.Pinned);

            // ajusta a função (ignora o último parâmetro já que o objeto do usuário vai estar no parser)
            MuParserLibrary.mupSetVarFactory(
                this.parserHandler, (IntFactoryFunction)this.factoryCallback.Function, new IntPtr());

            this.factoryFunc = func;
        }

        /// <summary>
        /// Defines a parser constant.
        /// </summary>
        /// <param name="name">The constant name</param>
        /// <param name="value">The constant value</param>
        /// <exception cref="ParserException">Throws if the name contains invalid signs</exception>
        public void DefineConst(string name, double value)
        {
            MuParserLibrary.mupDefineConst(this.parserHandler, name, value);
        }

        /// <summary>
        /// Defines a parser string constant.
        /// </summary>
        /// <param name="name">The constant name</param>
        /// <param name="value">The constant string value</param>
        /// <exception cref="ParserException">Throws if the name contains invalid signs</exception>
        public void DefineStrConst(string name, string value)
        {
            MuParserLibrary.mupDefineStrConst(this.parserHandler, name, value);
        }

        /// <summary>
        /// Clears all constants.
        /// </summary>
        void ClearConst()
        {
            MuParserLibrary.mupClearConst(this.parserHandler);
        }

        /// <summary>
        /// Add a value parsing function. When parsing an expression muParser
        /// tries to detect values in the expression string using different
        /// valident callbacks. Thuis it's possible to parse for hex values
        /// binary values and floating point values.
        /// </summary>
        /// <param name="identFunction">The callback function</param>
        public void AddValIdent(IdentFunction identFunction)
        {
            // bloqueia o GC de mover o delegate
            ParserCallback callback = new ParserCallback(identFunction);

            // passa a chamada
            MuParserLibrary.mupAddValIdent(this.parserHandler, identFunction);

            this.identFunctionsCallbacks.Add(callback);
        }

        /// <summary>
        /// Define a parser function without arguments.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <param name="allowOpt">A flag indicating this function may be optimized</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, FunType0 func, bool allowOpt = true)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineFun0(this.parserHandler, name, func, allowOpt);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with one argument.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <param name="allowOpt">A flag indicating this function may be optimized</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, FunType1 func, bool allowOpt = true)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineFun1(this.parserHandler, name, func, allowOpt);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with two arguments.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <param name="allowOpt">A flag indicating this function may be optimized</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, FunType2 func, bool allowOpt = true)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineFun2(this.parserHandler, name, func, allowOpt);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with three arguments.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <param name="allowOpt">A flag indicating this function may be optimized</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, FunType3 func, bool allowOpt = true)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineFun3(this.parserHandler, name, func, allowOpt);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with four arguments.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <param name="allowOpt">A flag indicating this function may be optimized</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, FunType4 func, bool allowOpt = true)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineFun4(this.parserHandler, name, func, allowOpt);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with five arguments.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <param name="allowOpt">A flag indicating this function may be optimized</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, FunType5 func, bool allowOpt = true)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineFun5(this.parserHandler, name, func, allowOpt);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with six arguments.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <param name="allowOpt">A flag indicating this function may be optimized</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, FunType6 func, bool allowOpt = true)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineFun6(this.parserHandler, name, func, allowOpt);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with seven arguments.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <param name="allowOpt">A flag indicating this function may be optimized</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, FunType7 func, bool allowOpt = true)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineFun7(this.parserHandler, name, func, allowOpt);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with eight arguments.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <param name="allowOpt">A flag indicating this function may be optimized</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, FunType8 func, bool allowOpt = true)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineFun8(this.parserHandler, name, func, allowOpt);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with nine arguments.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <param name="allowOpt">A flag indicating this function may be optimized</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, FunType9 func, bool allowOpt = true)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineFun9(this.parserHandler, name, func, allowOpt);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with ten arguments.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <param name="allowOpt">A flag indicating this function may be optimized</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, FunType10 func, bool allowOpt = true)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineFun10(this.parserHandler, name, func, allowOpt);

            this.funcCallbacks.Add(name, callback);
        }


        /// <summary>
        /// Define a parser function without arguments for bulk mode.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, BulkFunType0 func)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineBulkFun0(this.parserHandler, name, func);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with one argument for bulk mode.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, BulkFunType1 func)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineBulkFun1(this.parserHandler, name, func);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with two arguments for bulk mode.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, BulkFunType2 func)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineBulkFun2(this.parserHandler, name, func);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with three arguments for bulk mode.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, BulkFunType3 func)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineBulkFun3(this.parserHandler, name, func);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with four arguments for bulk mode.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, BulkFunType4 func)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineBulkFun4(this.parserHandler, name, func);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with five arguments for bulk mode.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, BulkFunType5 func)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineBulkFun5(this.parserHandler, name, func);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with six arguments for bulk mode.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, BulkFunType6 func)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineBulkFun6(this.parserHandler, name, func);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with seven arguments for bulk mode.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, BulkFunType7 func)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineBulkFun7(this.parserHandler, name, func);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with eight arguments for bulk mode.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, BulkFunType8 func)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineBulkFun8(this.parserHandler, name, func);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with nine arguments for bulk mode.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, BulkFunType9 func)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineBulkFun9(this.parserHandler, name, func);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with ten arguments for bulk mode.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, BulkFunType10 func)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineBulkFun10(this.parserHandler, name, func);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with ten arguments for bulk mode.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, BulkFunType11 func)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineBulkFun11(this.parserHandler, name, func);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function with a variable argument list.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <param name="allowOpt">A flag indicating this function may be optimized</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, MultFunType func, bool allowOpt = true)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineMultFun(this.parserHandler, name, func, allowOpt);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function taking a string as an argument.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, StrFunType1 func)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineStrFun1(this.parserHandler, name, func);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function taking a string and a value as arguments.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, StrFunType2 func)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineStrFun2(this.parserHandler, name, func);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Define a parser function taking a string and two values as arguments.
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <param name="func">The callback function</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineFun(string name, StrFunType3 func)
        {
            // cria a função
            ParserCallback callback = new ParserCallback(func);

            // adiciona no muParser
            MuParserLibrary.mupDefineStrFun3(this.parserHandler, name, func);

            this.funcCallbacks.Add(name, callback);
        }

        /// <summary>
        /// Clears all functions definitions.
        /// </summary>
        public void ClearFun()
        {
            MuParserLibrary.mupClearFun(this.parserHandler);
            this.funcCallbacks.Clear();
        }

        /// <summary>
        /// Define a unary infix operator.
        /// </summary>
        /// <param name="identifier">The operator identifier</param>
        /// <param name="func">The callback function</param>
        /// <param name="allowOpt">A flag indicating this function may be optimized</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineInfixOprt(string identifier, FunType1 func, bool allowOpt = true)
        {
            // bloqueia o GC de mover o delegate
            ParserCallback callback = new ParserCallback(func);

            MuParserLibrary.mupDefineInfixOprt(this.parserHandler, identifier, func, allowOpt);

            this.infixOprtCallbacks.Add(identifier, callback);
        }

        /// <summary>
        /// Define a unary postfix operator.
        /// </summary>
        /// <param name="identifier">The operator identifier</param>
        /// <param name="func">The callback function</param>
        /// <param name="allowOpt">A flag indicating this function may be optimized</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefinePostfixOprt(string identifier, FunType1 func, bool allowOpt = true)
        {
            // bloqueia o GC de mover o delegate
            ParserCallback callback = new ParserCallback(func);

            MuParserLibrary.mupDefinePostfixOprt(this.parserHandler, identifier, func, allowOpt);

            this.postfixOprtCallbacks.Add(identifier, callback);
        }

        /// <summary>
        /// Define a binary operator.
        /// </summary>
        /// <param name="identifier">The operator identifier</param>
        /// <param name="func">The callback function</param>
        /// <param name="precedence">The operator precedence</param>
        /// <param name="associativity">The associativity of the operator</param>
        /// <param name="allowOpt">A flag indicating this function may be optimized</param>
        /// <exception cref="ParserException">Throws if there is a name conflict</exception>
        public void DefineOprt(string identifier, FunType2 func, uint precedence = 0, OprtAssociativity associativity = OprtAssociativity.LEFT, bool allowOpt = false)
        {
            // bloqueia o GC de mover o delegate
            ParserCallback callback = new ParserCallback(func);

            MuParserLibrary.mupDefineOprt(this.parserHandler, identifier, func, precedence, (int)associativity, allowOpt);

            this.oprtCallbacks.Add(identifier, callback);
        }

        /// <summary>
        /// Clears all infix operators.
        /// </summary>
        /// <remarks>This function is not supported if using the original
        /// muParser library instead of the Sest-compatible library (which
        /// is available at Sest repository).</remarks>
        public void CleanInfixOprt()
        {
            // checa se a biblioteca do muParser é compatível
            this.CheckLibraryVersion();

            MuParserLibrary.mupClearInfixOprt(this.parserHandler);
            this.infixOprtCallbacks.Clear();
        }

        /// <summary>
        /// Clears all postfix operators.
        /// </summary>
        /// <remarks>This function is not supported if using the original
        /// muParser library instead of the Sest-compatible library (which
        /// is available at Sest repository).</remarks>
        public void CleanPostfixOprt()
        {
            // checa se a biblioteca do muParser é compatível
            this.CheckLibraryVersion();

            MuParserLibrary.mupClearPostfixOprt(this.parserHandler);
            this.postfixOprtCallbacks.Clear();
        }

        /// <summary>
        /// Clears all operators.
        /// </summary>
        public void CleanOprt()
        {
            MuParserLibrary.mupClearOprt(this.parserHandler);
            this.oprtCallbacks.Clear();
        }

        /// <summary>
        /// Enable or disable the built in binary operators. If you disable the
        /// built in binary operators there will be no binary operators defined.
        /// Thus you must add them manually one by one. It is not possible to
        /// disable built in operators selectively.
        /// </summary>
        /// <param name="oprtEn">Indicates if the operators will be enable or disabled</param>
        /// <remarks>This function is not supported if using the original
        /// muParser library instead of the Sest-compatible library (which
        /// is available at Sest repository).</remarks>
        public void EnableBuiltInOprt(bool oprtEn = true)
        {
            // checa se a biblioteca do muParser é compatível
            this.CheckLibraryVersion();

            MuParserLibrary.mupEnableBuiltInOprt(this.parserHandler, oprtEn);
        }

        /// <summary>
        /// Enable or disable the formula optimization feature. 
        /// </summary>
        /// <param name="optmEn">Indicates if the optimizer will be enable or disabled</param>
        /// <remarks>This function is not supported if using the original
        /// muParser library instead of the Sest-compatible library (which
        /// is available at Sest repository).</remarks>
        public void EnableOptimizer(bool optmEn = true)
        {
            // checa se a biblioteca do muParser é compatível
            this.CheckLibraryVersion();

            MuParserLibrary.mupEnableOptimizer(this.parserHandler, optmEn);
        }

        /// <summary>
        /// Set argument separator.
        /// </summary>
        /// <param name="value">The argument separator character</param>
        public void SetArgSep(char value)
        {
            MuParserLibrary.mupSetArgSep(this.parserHandler, value);
        }

        /// <summary>
        /// Set the decimal separator. By default muparser uses the "C" locale.
        /// The decimal separator of this locale is overwritten by the one
        /// provided here.
        /// </summary>
        /// <param name="value">The decimal separator character</param>
        public void SetDecSep(char value)
        {
            MuParserLibrary.mupSetDecSep(this.parserHandler, value);
        }

        /// <summary>
        /// Sets the thousands operator. By default muparser uses the "C"
        /// locale. The thousands separator of this locale is overwritten by the
        /// one provided here.
        /// </summary>
        /// <param name="value">The thousands separator character</param>
        public void SetThousandsSep(char value = '\0')
        {
            MuParserLibrary.mupSetThousandsSep(this.parserHandler, value);
        }

        /// <summary>
        /// Resets the locale. The default locale used "." as decimal separator,
        /// no thousands separator and "," as function argument separator.
        /// </summary>
        public void ResetLocale()
        {
            MuParserLibrary.mupResetLocale(this.parserHandler);
        }

        //void SetVarFactory(FactoryFunction ^func, Object ^userData){ }

        /// <summary>
        /// Calculate the result from the expression formula.
        /// </summary>
        /// <exception cref="ParserException">Throws if no formula is set or in case
        /// of any other error related to the formula</exception>
        public double Eval()
        {
            return MuParserLibrary.mupEval(this.parserHandler);
        }

        /// <summary>
        /// Calculate the results from the expression formula.
        /// </summary>
        /// <param name="bulkSize">The number of times that the formula will be
        /// calculated</param>
        /// <returns>The list with the results of the calculation</returns>
        /// <exception cref="ParserException">Throws if no formula is set or in case
        /// of any other error related to the formula</exception>
        public double[] EvalBulk(int bulkSize)
        {
            // aloca o vetor de resposta
            double[] result = new double[bulkSize];

            /*
             * Os PInvoke já fazem o pin do objeto.
             */

            // executa o comando
            MuParserLibrary.mupEvalBulk(this.parserHandler, result, bulkSize);

            return result;
        }

        /// <summary>
        /// Calculate the results from the expression with multiples formulas.
        /// </summary>
        /// <returns>The list with the results of the calculations</returns>
        /// <exception cref="ParserException">Throws if no formula is set or in case
        /// of any other error related to the formula</exception>
        public double[] EvalMulti()
        {
            /*
		     * O vetor retornado pela função Eval é gerenciado pelo próprio parser,
		     * não sendo necessário desalocá-lo.
		     */

            int n = 0;

            IntPtr result = MuParserLibrary.mupEvalMulti(this.parserHandler, ref n);

            // aloca o array de retorno do .net
            double[] ret = new double[n];

            // copia o resultado
            Marshal.Copy(result, ret, 0, n);
            return ret;
        }

        #endregion
    }
}
