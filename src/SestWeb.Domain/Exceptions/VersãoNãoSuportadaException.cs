using System;
using System.Runtime.Serialization;

namespace SestWeb.Domain.Exceptions
{
    public class VersãoNãoSuportadaException : Exception
    {
        public VersãoNãoSuportadaException()
        {
        }

        protected VersãoNãoSuportadaException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public VersãoNãoSuportadaException(string message) : base(message)
        {
        }

        public VersãoNãoSuportadaException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
