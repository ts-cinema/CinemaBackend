using System;

namespace Envista.Core.Common.System
{
    /// <summary>
    /// Provides helper methods for converting a string representation of a value to it's native type.
    /// </summary>
    public class DataParser
    {
        /// <summary>
        /// Attempts to convert a string representation of a value to it's native type.
        /// </summary>
        /// <param name="value">
        /// A string containing the string representation to examine.
        /// </param>
        /// <returns>
        /// A tuple showing the string representation's native type and value.
        /// </returns>
        public Tuple<Type, object> Parse(string value)
        {
            Type type = typeof(string);
            object obj = null;

            try
            {
                if (obj == null)
                {
                    // Check for a date time
                    DateTime data = DateTime.MinValue;
                    if (DateTime.TryParse(value, out data))
                    {
                        type = data.GetType();
                        obj = data;
                    }
                }

                if (obj == null)
                {
                    // Check for a GUID
                    Guid data = Guid.Empty;
                    if (Guid.TryParse(value, out data))
                    {
                        type = data.GetType();
                        obj = data;
                    }
                }

                if (obj == null)
                {
                    // Check for a boolean
                    bool data = false;
                    if (Boolean.TryParse(value, out data))
                    {
                        type = data.GetType();
                        obj = data;
                    }
                }

                if (obj == null)
                {
                    // Check for an int32
                    Int32 data = -0;
                    if (Int32.TryParse(value, out data))
                    {
                        type = data.GetType();
                        obj = data;
                    }
                }

                if (obj == null)
                {
                    // Check for an int64
                    Int64 data = 0;
                    if (Int64.TryParse(value, out data))
                    {
                        type = data.GetType();
                        obj = data;
                    }
                }
            }
            catch
            {
                // Do nothing
            }

            if (obj == null)
            {
                // Assume it's a string
                type = value.GetType();
                obj = value;
            }

            return Tuple.Create<Type, object>(type, obj);
        }
    }
}
