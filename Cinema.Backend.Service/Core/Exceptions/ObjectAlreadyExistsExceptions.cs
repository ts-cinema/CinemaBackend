using System;

namespace Envista.Core.Common.Exceptions
{
    /// <summary>
    /// This exception indicates that an object does not exist.
    /// </summary>
    public class ObjectAlreadyExistsException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the ObjectAlreadyExistsException class.
        /// </summary>
        public ObjectAlreadyExistsException()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the ObjectAlreadyExistsException class.
        /// </summary>
        public ObjectAlreadyExistsException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the ObjectAlreadyExistsException class.
        /// </summary>
        public ObjectAlreadyExistsException(string message, Exception exception)
            : base(message, exception)
        {

        }
    }
}
