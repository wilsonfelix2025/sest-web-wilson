using System;
using System.Runtime.Serialization;

namespace SestWeb.Domain.Entities.PontosEntity.InternalCollections
{
    /// <summary>
    /// Exception class: LurchTableCorruptionException
    /// The LurchTable internal datastructure appears to be corrupted.
    /// </summary>
    public class LurchTableCorruptionException : ApplicationException
    {
        /// <summary>Serialization constructor</summary>
        protected LurchTableCorruptionException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }

        /// <summary>
        /// Used to create this exception from an hresult and message bypassing the message formatting
        /// </summary>
        internal static Exception Create(int hResult, string message)
        {
            // ISSUE: object of a compiler-generated type is created
            return (Exception)new LurchTableCorruptionException((Exception)null, hResult, message);
        }

        /// <summary>
        /// Constructs the exception from an hresult and message bypassing the message formatting
        /// </summary>
        protected LurchTableCorruptionException(Exception innerException, int hResult, string message)
          : base(message, innerException)
        {
            this.HResult = hResult;
            // ISSUE: reference to a compiler-generated method
            this.HelpLink = "The LurchTable internal datastructure appears to be corrupted.";
        }

        /// <summary>
        /// The LurchTable internal datastructure appears to be corrupted.
        /// </summary>
        public LurchTableCorruptionException()
          : this((Exception)null, -1, "The LurchTable internal datastructure appears to be corrupted.")
        {
        }

        /// <summary>
        /// The LurchTable internal datastructure appears to be corrupted.
        /// </summary>
        public LurchTableCorruptionException(Exception innerException)
          : this(innerException, -1, "The LurchTable internal datastructure appears to be corrupted.")
        {
        }

        /// <summary>
        /// if(condition == false) throws The LurchTable internal datastructure appears to be corrupted.
        /// </summary>
        public static void Assert(bool condition)
        {
            if (!condition)
            {
                // ISSUE: object of a compiler-generated type is created
                throw new LurchTableCorruptionException();
            }
        }
    }
}
