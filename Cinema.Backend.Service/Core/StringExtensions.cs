using Template.Service.Models.Core;

namespace Template.Service.Core
{
    /// <summary>
    /// Defines string extension methods
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// String extension method that splits string into two substrings.
        /// </summary>
        /// <param name="input">
        /// A string parameter containing the value that requires split into two substrings.
        /// </param>
        /// <param name="separator">
        /// A character containing the separator that delimits the substrings.
        /// </param>
        /// <returns>
        /// Custom StringPair model containing the substrings as model properties.
        /// </returns>
        public static StringPair SplitOnce(this string input, char separator)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));

            if (!input.Contains(separator))
                throw new ArgumentException("Specified separator not found in input string", nameof(separator));

            string[] stringElements = input.Split(separator);

            StringPair stringPair = new StringPair()
            {
                First = stringElements[0],
                Second = stringElements[1],
            };          

            return stringPair;
        }
    }    
}
