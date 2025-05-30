using System;

namespace SestWeb.Application
{
    public class ApplicationException : Exception
    {
        internal ApplicationException(string businessMessage) : base(businessMessage)
        {
        }
    }
}
