using System;

namespace Envista.Core.Common.Exceptions
{
    /// <summary>
    /// This exception indicates the calling users has insufficient permissions to fulfill the request.
    /// </summary>
    public class SecurityException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the SecurityException class.
        /// </summary>
        public SecurityException()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the SecurityException class.
        /// </summary>
        public SecurityException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the SecurityException class.
        /// </summary>
        public SecurityException(string message, Exception exception)
            : base(message, exception)
        {

        }
    }
}
