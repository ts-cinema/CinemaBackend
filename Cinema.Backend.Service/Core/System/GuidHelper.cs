using System.Security.Cryptography;
using System.Text;

namespace Envista.Core.Common.System
{
    /// <summary>
    /// Provides helper methods for interacting with GUIDs.
    /// </summary>
    public static class GuidHelper
    {
        /// <summary>
        /// Derive a GUID from a string.
        /// </summary>
        /// <param name="input">
        /// A string containing an input used to derive a GUID.
        /// </param>
        /// <returns>
        /// An ID that is derived from the seed.
        /// </returns>
        public static Guid DeriveGuid(string input)
        {
            Guid id = Guid.Empty;

            if (!string.IsNullOrEmpty(input))
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
                    id = new Guid(hash);
                }
            }

            return id;
        }
    }
}
