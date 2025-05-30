using System;
using System.Runtime.InteropServices;

namespace SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper
{
    /// <summary>
    /// Class of the parser variable.
    /// </summary>
    public class ParserVariable
    {
        private string name;
        private double[] valueArray;

        // este ponteiro é necessário para evitar que o GC mova o array
        private GCHandle ptrValueArray;

        /// <summary>
        /// Gets the variable name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets os sets the variable value. If the variable is an array, it
        /// gets or sets the first value.
        /// </summary>
        public double Value
        {
            get
            {
                // retorna o primeiro valor do array
                return this.valueArray[0];
            }
            set
            {
                this.valueArray[0] = value;
            }
        }

        /// <summary>
        /// Gets os sets the variable value as an array. If the variable isn't an array, it
        /// gets the value as an array with a single value.
        /// </summary>
        public double[] ValueArray
        {
            get
            {
                return this.valueArray;
            }
        }

        /// <summary>
        /// Gets the variable pointer. This will be used to create variables in
        /// muParser structure.
        /// </summary>
        public IntPtr Pointer
        {
            get
            {
                // o objeto deve estar 'pinned' para esta função funcionar
                return this.ptrValueArray.AddrOfPinnedObject();
            }
        }

        /// <summary>
        /// Class constructor. It creates a variable with a single default
        /// value.
        /// </summary>
        /// <param name="name">The variable name</param>
        public ParserVariable(string name)
        {
            this.name = name;
            this.valueArray = new double[1];
            this.valueArray[0] = 0.0;

            // evita que o objeto do array seja movido
            this.ptrValueArray = GCHandle.Alloc(this.valueArray, GCHandleType.Pinned);
        }

        /// <summary>
        /// Class constructor. It creates a variable with a single value.
        /// </summary>
        /// <param name="name">The variable name</param>
        /// <param name="value">The variable initial value</param>
        public ParserVariable(string name, double value)
        {
            this.name = name;
            this.valueArray = new double[1];
            this.valueArray[0] = value;

            // evita que o objeto do array seja movido
            this.ptrValueArray = GCHandle.Alloc(this.valueArray, GCHandleType.Pinned);
        }

        /// <summary>
        /// Class constructor. It creates a variable with multiples values.
        /// </summary>
        /// <param name="name">The variable name</param>
        /// <param name="valueArray">The array with the variable values. The
        /// array's memory will be pinned to avoid the garbage collector to move
        /// it and causing memory problems</param>
        public ParserVariable(string name, double[] valueArray)
        {
            this.name = name;

            /*
             * Verifica se o valor do array é diferente de nulo antes de
             * ajustar a lista de valores.
             */
            if (valueArray != null)
            {
                this.valueArray = valueArray;
            }
            else
            {
                // cria um vetor com uma posição apenas
                this.valueArray = new double[1];
                this.valueArray[0] = 0.0;
            }

            // evita que o objeto do array seja movido
            this.ptrValueArray = GCHandle.Alloc(this.valueArray, GCHandleType.Pinned);
        }

        /// <summary>
        /// Class destructor.
        /// </summary>
        ~ParserVariable()
        {
            // libera o vetor
            this.ptrValueArray.Free();
        }
    }
}
