using System;
using System.Runtime.Serialization;
using SestWeb.Domain.Exceptions;

namespace SestWeb.Infra.Exceptions
{
    public class DatabaseOffLineException : InfrastructureException
    {
        public DatabaseOffLineException() : base("Banco de dados está offline ou não respondendo.")
        {
        }

        protected DatabaseOffLineException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public DatabaseOffLineException(string message) : base($"Banco de dados está offline ou não respondendo - {message}")
        {
        }

        public DatabaseOffLineException(string message, Exception innerException) : base($"Banco de dados está offline ou não respondendo - {message}", innerException)
        {
        }
    }
}
