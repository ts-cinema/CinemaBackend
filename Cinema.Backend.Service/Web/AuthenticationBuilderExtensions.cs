using Microsoft.AspNetCore.Authentication;

namespace Envista.Template.Service.Web
{
    /// <summary>
    /// Represents extension methods for AuthanticationBuilder class.
    /// </summary>
    public static class AuthenticationBuilderExtensions
    {
        /// <summary>
        /// Adds a custom authentication extension method.
        /// </summary>        
        public static AuthenticationBuilder AddCustomAuthentication(this AuthenticationBuilder builder, Action<CustomAuthenticationOptions> configureOptions)
        {
            /// Add custom authentication scheme with custom options and custom handler.
            return builder.AddScheme<CustomAuthenticationOptions, CustomAuthenticationHandler>(CustomAuthenticationOptions.DefaultScheme, configureOptions);
        }
    }
}
