using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;

namespace Envista.Template.Service.Web
{
    /// <summary>
    /// Defines custom authentication options used by the authentication handler.
    /// </summary>
    public class CustomAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "basic";

        /// <summary>
        /// Get the authentication scheme.
        /// </summary>
        public string Scheme => DefaultScheme;

        /// <summary>
        /// Gets the authentication token.
        /// </summary>
        public StringValues Token { get; set; }
    }
}
