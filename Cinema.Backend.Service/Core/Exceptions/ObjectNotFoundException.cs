using System;

namespace Envista.Core.Common.Exceptions
{
    /// <summary>
    /// This exception indicates that an object does not exist.
    /// </summary>
    public class ObjectNotFoundException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the ObjectNotFoundException class.
        /// </summary>
        public ObjectNotFoundException()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the ObjectNotFoundException class.
        /// </summary>
        public ObjectNotFoundException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the ObjectNotFoundException class.
        /// </summary>
        public ObjectNotFoundException(string message, Exception exception)
            : base(message, exception)
        {

        }
    }
}
