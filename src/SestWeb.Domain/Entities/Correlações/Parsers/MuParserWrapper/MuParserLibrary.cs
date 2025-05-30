using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper
{
    internal static class MuParserLibrary
    {
        // o constructor será executado quando for feita a chamada de alguma
        // função da classe
        static MuParserLibrary()
        {
            try
            {
                ExtractResourceToFile(@"SestWeb.Domain.muparser.dll", "muparser.dll");
                //EmbeddedAssemblyExtractor.ExtractEmbeddedAssemblyToFile("muparser.dll");
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"MuParserLibrary: {e.Message}", e);
            }
        }

        #region Funções para carregar biblioteca do Windows

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string libname);

        private static void WindowsLoadMuParser()
        {
            // pega a pasta atual da biblioteca
            string libPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(MuParserLibrary)).Location);

            // verifica se o processo atual é 32 ou 64 bits
            if (Environment.Is64BitProcess)
            {
                // adiciona o x64 no caminho
                //libPath += Path.DirectorySeparatorChar + @"x64" + Path.DirectorySeparatorChar;
            }
            else
            {
                // adiciona o x86 no caminho
                //libPath += Path.DirectorySeparatorChar + @"x86" + Path.DirectorySeparatorChar;
            }

            /*
             * Como é Windows, então é só adicionar o .dll na biblioteca.
             */
            //libPath = Path.GetFullPath(Path.Combine(libPath, @"muparser.dll"));

            // carrega a biblioteca
            LoadLibrary(libPath);
        }

        #endregion

        #region Linux 

        [DllImport("libdl.so")]
        private static extern IntPtr dlopen(string filename, int flags);

        [DllImport("libdl.so")]
        private static extern IntPtr dlsym(IntPtr handle, string symbol);

        const int RTLD_NOW = 2; // for dlopen's flags 

        private static void LinuxLoadMuParser()
        {
            //IntPtr moduleHandle = dlopen(modulePath, RTLD_NOW);
        }

        #endregion

        #region Extract from embbeded resources

        private static void ExtractResourceToFile(string resourceName, string filename)
        {
            var domainResourceName = @"SestWeb.Domain.muparser.dll";
#if DEBUG
            Console.WriteLine($"*** Loading muparsing ***");
#endif
            string assemblyName = @"SestWeb.Domain.dll";
            var assemblyDirectory = GetAssemblyDirectory();

#if DEBUG
            Console.WriteLine($"*** assemblyDirectory: {assemblyDirectory}");
#endif

            if (assemblyDirectory.Contains("SestWeb.Domain.Tests"))
            {
                resourceName = @"SestWeb.Domain.Tests.muparser.dll";
                assemblyName = @"SestWeb.Domain.Tests.dll";
#if DEBUG
                Console.WriteLine($"*** Test Project running... assemblyName: {assemblyName} , resourceName: {resourceName}");
#endif
            }
            else if (assemblyDirectory.Contains("SestWeb.Application.Tests"))
            {
                resourceName = @"SestWeb.Application.Tests.muparser.dll";
                assemblyName = @"SestWeb.Application.Tests.dll";
#if DEBUG
                Console.WriteLine($"*** Test Project running... assemblyName: {assemblyName} , resourceName: {resourceName}");
#endif

            }
            else if (assemblyDirectory.Contains("SestWeb.Api.Tests"))
            {
                resourceName = @"SestWeb.Api.Tests.muparser.dll";
                assemblyName = @"SestWeb.Api.Tests.dll";
#if DEBUG
                Console.WriteLine($"*** Test Project running... assemblyName: {assemblyName} , resourceName: {resourceName}");
#endif

            }
            else if (assemblyDirectory.Contains(@"SestWeb.Domain.muparser.dll"))
            {
                resourceName = @"SestWeb.Domain.muparser.dll";
                assemblyName = @"SestWeb.Domain.dll";
#if DEBUG
                Console.WriteLine($"*** Domain Project running... assemblyName: {assemblyName} , resourceName: {resourceName}");
#endif
            }

            // Path.Combine necessário para setar corretamente slash/backslash
            var assemblyPath = Path.Combine(assemblyDirectory, assemblyName);
#if DEBUG
            Console.WriteLine($"*** Project assemblyPath: {assemblyPath}");
#endif

            if (!File.Exists(assemblyPath))
                Console.WriteLine($"*** Assembly {assemblyName} not found in {assemblyPath}");
#if DEBUG
            Console.WriteLine($"*** Loading assembly: {assemblyName} at {assemblyPath}");
#endif
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            if (assembly==null)
                Console.WriteLine($"*** assembly {assemblyName} at {assemblyPath} not loaded.");

            var domainAssembly = Assembly.GetAssembly(typeof(MuParserLibrary));

            if (File.Exists(filename))
            {
#if DEBUG
                Console.WriteLine($"*** {filename} found, deleting...");
#endif
                File.Delete(filename);
#if DEBUG
                Console.WriteLine($"*** {filename} deleted");
#endif
            }

            if (!File.Exists(filename))
            {
#if DEBUG
                Console.WriteLine($"*** assemblyPath: {filename} loading...");
#endif

                using (Stream s = domainAssembly.GetManifestResourceStream(domainResourceName))
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    byte[] buffer = new byte[s.Length];
#if DEBUG
                    Console.WriteLine($"*** reading {resourceName}");
#endif
                    s.Read(buffer, 0, buffer.Length);
#if DEBUG
                    Console.WriteLine($"*** {resourceName} read.");
#endif

                    if (!fs.CanWrite)
                    {
#if DEBUG
                        Console.WriteLine($"*** FileStream: cant write.");
#endif
                        //throw new IOException($"Não é possível escrever o arquivo: {filename}");
                    }
#if DEBUG
                    Console.WriteLine($"*** {filename} being created...");
#endif
                    fs.Write(buffer, 0, buffer.Length);
#if DEBUG
                    Console.WriteLine($"*** {filename} created.");
#endif
                }
            }
            else
            {
#if DEBUG
                Console.WriteLine($"*** assemblyPath: {filename} already loaded.");
#endif
            }
        }

        public static string GetAssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        #endregion

        #region API

        #region API Original

        /*
         * As funções que retornam string fazem com que o .NET tente liberar o
         * endereço de memória.
         */

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr mupCreate(int nBaseType);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupRelease(IntPtr a_hParser);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr mupGetExpr(IntPtr a_hParser);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupSetExpr(IntPtr a_hParser, string a_szExpr);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupSetVarFactory(IntPtr a_hParser,
            IntFactoryFunction a_pFactory,
            IntPtr pUserData);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr mupGetVersion(IntPtr a_hParser);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern double mupEval(IntPtr a_hParser);

        // não funciona tentar fazer direto
        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr mupEvalMulti(IntPtr a_hParser, ref int nNum);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupEvalBulk(IntPtr a_hParser, double[] a_fResult, int nSize);

        // Defining callbacks / variables / constants
        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineFun0(IntPtr a_hParser, string a_szName, FunType0 a_pFun, bool a_bOptimize);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineFun1(IntPtr a_hParser, string a_szName, FunType1 a_pFun, bool a_bOptimize);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineFun2(IntPtr a_hParser, string a_szName, FunType2 a_pFun, bool a_bOptimize);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineFun3(IntPtr a_hParser, string a_szName, FunType3 a_pFun, bool a_bOptimize);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineFun4(IntPtr a_hParser, string a_szName, FunType4 a_pFun, bool a_bOptimize);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineFun5(IntPtr a_hParser, string a_szName, FunType5 a_pFun, bool a_bOptimize);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineFun6(IntPtr a_hParser, string a_szName, FunType6 a_pFun, bool a_bOptimize);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineFun7(IntPtr a_hParser, string a_szName, FunType7 a_pFun, bool a_bOptimize);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineFun8(IntPtr a_hParser, string a_szName, FunType8 a_pFun, bool a_bOptimize);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineFun9(IntPtr a_hParser, string a_szName, FunType9 a_pFun, bool a_bOptimize);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineFun10(IntPtr a_hParser, string a_szName, FunType10 a_pFun, bool a_bOptimize);

        // Defining bulkmode functions

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineBulkFun0(IntPtr a_hParser, string a_szName, BulkFunType0 a_pFun);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineBulkFun1(IntPtr a_hParser, string a_szName, BulkFunType1 a_pFun);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineBulkFun2(IntPtr a_hParser, string a_szName, BulkFunType2 a_pFun);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineBulkFun3(IntPtr a_hParser, string a_szName, BulkFunType3 a_pFun);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineBulkFun4(IntPtr a_hParser, string a_szName, BulkFunType4 a_pFun);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineBulkFun5(IntPtr a_hParser, string a_szName, BulkFunType5 a_pFun);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineBulkFun6(IntPtr a_hParser, string a_szName, BulkFunType6 a_pFun);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineBulkFun7(IntPtr a_hParser, string a_szName, BulkFunType7 a_pFun);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineBulkFun8(IntPtr a_hParser, string a_szName, BulkFunType8 a_pFun);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineBulkFun9(IntPtr a_hParser, string a_szName, BulkFunType9 a_pFun);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineBulkFun10(IntPtr a_hParser, string a_szName, BulkFunType10 a_pFun);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineBulkFun11(IntPtr a_hParser, string a_szName, BulkFunType11 a_pFun);

        // string functions
        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineStrFun1(IntPtr a_hParser, string a_szName, StrFunType1 a_pFun);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineStrFun2(IntPtr a_hParser, string a_szName, StrFunType2 a_pFun);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineStrFun3(IntPtr a_hParser, string a_szName, StrFunType3 a_pFun);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineMultFun(IntPtr a_hParser,
                                           string a_szName,
                                           MultFunType a_pFun,
                                           bool a_bOptimize);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineOprt(IntPtr a_hParser,
                                        string a_szName,
                                        FunType2 a_pFun,
                                        uint a_nPrec,
                                        int a_nOprtAsct,
                                        bool a_bOptimize);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineConst(IntPtr a_hParser,
                                         string a_szName,
                                         double a_fVal);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineStrConst(IntPtr a_hParser,
                                            string a_szName,
                                            string a_sVal);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineVar(IntPtr a_hParser,
                                       string a_szName,
                                       IntPtr a_fVar);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineBulkVar(IntPtr a_hParser,
                                       string a_szName,
                                       IntPtr a_fVar);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefinePostfixOprt(IntPtr a_hParser,
                                               string a_szName,
                                               FunType1 a_pOprt,
                                               bool a_bOptimize);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineInfixOprt(IntPtr a_hParser,
                                             string a_szName,
                                             FunType1 a_pOprt,
                                             bool a_bOptimize);

        // Define character sets for identifiers
        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineNameChars(IntPtr a_hParser, string a_szCharset);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineOprtChars(IntPtr a_hParser, string a_szCharset);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupDefineInfixOprtChars(IntPtr a_hParser, string a_szCharset);

        // Remove all / single variables
        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupRemoveVar(IntPtr a_hParser, string a_szName);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupClearVar(IntPtr a_hParser);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupClearConst(IntPtr a_hParser);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupClearOprt(IntPtr a_hParser);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupClearFun(IntPtr a_hParser);

        // Querying variables / expression variables / constants
        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mupGetExprVarNum(IntPtr a_hParser);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mupGetVarNum(IntPtr a_hParser);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mupGetConstNum(IntPtr a_hParser);

        // essas duas funções são perigosas já que não da para saber o tamanho dos arrays
        // public static extern void mupGetExprVar(IntPtr a_hParser, uint a_iVar, ref string a_pszName, ref double[] a_pVar);
        // public static extern void mupGetVar(IntPtr a_hParser, uint a_iVar, ref string a_pszName, ref double[] a_pVar);

        /*
         * Não dá para passar um ref de uma string e nem um array de byte. Tem
         * que ser a referência para um ponteiro.
         */
        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupGetConst(IntPtr a_hParser,
            uint a_iVar,
            ref IntPtr a_pszName,
            ref double a_pVar);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupSetArgSep(IntPtr a_hParser, char cArgSep);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupSetDecSep(IntPtr a_hParser, char cArgSep);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupSetThousandsSep(IntPtr a_hParser, char cArgSep);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupResetLocale(IntPtr a_hParser);

        // Add value recognition callbacks
        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupAddValIdent(IntPtr a_hParser, IdentFunction a_func);

        // Error handling
        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool mupError(IntPtr a_hParser);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupErrorReset(IntPtr a_hParser);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupSetErrorHandler(IntPtr a_hParser, ErrorFuncType a_pErrHandler);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr mupGetErrorMsg(IntPtr a_hParser);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mupGetErrorCode(IntPtr a_hParser);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mupGetErrorPos(IntPtr a_hParser);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr mupGetErrorToken(IntPtr a_hParser);

        #endregion

        #region API adicionada

        /*
         * Adicionando as funções que não são suportadas pela API original da
         * DLL muparser mas que são suportadas pela biblioteca de classes da
         * mesma.
         */

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupClearInfixOprt(IntPtr a_hParser);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupClearPostfixOprt(IntPtr a_hParser);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupEnableBuiltInOprt(IntPtr a_hParser, bool oprtEn);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mupEnableOptimizer(IntPtr a_hParser, bool optmEn);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr mupValidNameChars(IntPtr a_hParser);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr mupValidOprtChars(IntPtr a_hParser);

        [DllImport("muparser", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr mupValidInfixOprtChars(IntPtr a_hParser);

        #endregion

        #endregion
    }
}
