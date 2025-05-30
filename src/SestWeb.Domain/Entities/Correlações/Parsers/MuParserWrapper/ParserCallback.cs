using System;
using System.Runtime.InteropServices;

namespace SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper
{
    /// <summary>
    /// Class of the parser callbacks.
    /// </summary>
    public class ParserCallback
    {
        private Delegate func;

        /*
		 * Quando a função passa a ser um callback da biblioteca o objeto do
		 * delegate não deve ser apagado pelo GC.
		 */
        private GCHandle ptrFunc;

        /// <summary>
        /// Gets the delegate which represents the callback.
        /// </summary>
        public Delegate Function
        {
            get
            {
                return this.func;
            }
        }

        /// <summary>
        /// Class constructor. It receives a delegate that will represent the
        /// callback triggered by the muParser library. It also blocks the
        /// garbage colletor to destroy the delegate object.
        /// </summary>
        /// <param name="func">The callback delegate object</param>
        public ParserCallback(Delegate func)
        {
            this.func = func;

            // bloqueia o GC de apagar o objeto do delegate
            this.ptrFunc = GCHandle.Alloc(func);
        }

        /// <summary>
        /// Class destructor. It releases the delegate object to be removed by
        /// the garbage collector.
        /// </summary>
        ~ParserCallback()
        {
            // libera o GC para apagar o objeto
            this.ptrFunc.Free();
        }
    }
}
