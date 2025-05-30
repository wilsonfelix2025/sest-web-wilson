using System;

namespace SestWeb.Application.Helpers
{
    public abstract class UseCaseOutput<T> where T : Enum
    {
        public T Status { get; protected set; }
        public string Mensagem { get; protected set; } = null;
    }
}