using System;
using System.Runtime.Serialization;

namespace SestWeb.Domain.Exceptions
{
    public class VersãoNãoEncontradaException : Exception
    {
        public VersãoNãoEncontradaException()
        {
        }

        protected VersãoNãoEncontradaException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public VersãoNãoEncontradaException(string message) : base(message)
        {
        }

        public VersãoNãoEncontradaException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
