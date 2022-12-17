using System.Security.Cryptography;
using System.Text;

namespace Envista.Core.Common.Cryptographics
{
    /// <summary>
    /// Provides helper methods for hashing.
    /// </summary>
    public class HashHelper
    {
        /// <summary>
        /// Generates SHA256 hash code from string
        /// </summary>
        /// <param name="input">
        /// A string containing an input used to generate SHA256 hashcode.
        /// </param>
        /// <returns>
        /// SHA256 hashed value of the string
        /// </returns>
        public static string GetSha256HashCode(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");

            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Convert byte array to a string   
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    stringBuilder.Append(bytes[i].ToString("x2"));
                }
                return stringBuilder.ToString();
            }
        }
    }
}
