
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using Template.Service.Configuration;
using Template.Service.Core;
using Template.Service.Models.Core;
using Template.Service.Security;

namespace Envista.Template.Service.Web
{
    /// <summary>
    /// Defines a custom authentication handler for the web request pipeline.
    /// </summary>
    public class CustomAuthenticationHandler : AuthenticationHandler<CustomAuthenticationOptions>
    {
        private readonly IConfiguration _configuration;

        private const char AuthHeaderSeparator = ':';

        /// <summary>
        /// Initialize a new instance of the CustomAuthenticationHandler.
        /// </summary>
        /// <param name="options">
        /// An object used to monitor configuration changes.
        /// </param>
        /// <param name="logger">
        /// An object used to configure the logging system
        /// </param>
        /// <param name="configuration">
        /// An object that represents the service's configuration
        /// </param>
        /// <param name="encoder">
        /// An object that represents a URL character encoding
        /// </param>
        /// <param name="clock">
        /// An object that abstracts the system clock
        /// </param>
        public CustomAuthenticationHandler(IOptionsMonitor<CustomAuthenticationOptions> options, ILoggerFactory logger, IConfiguration configuration, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Authenticates a web request asynchronously
        /// </summary>
        /// <returns>
        /// An object that contains the result of an authentication call.
        /// </returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Get Authorization header value.
            if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out StringValues value))
            {
                return AuthenticateResult.NoResult();
            }

            // Check for valid authorization header
            string? authHeader = value.FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || (!authHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase)))
            {
                return AuthenticateResult.NoResult();
            }

            // Decode the authorization header
            var encoding = authHeader.Substring("Basic ".Length).Trim();
            var content = Encoding.UTF8.GetString(Convert.FromBase64String(encoding));

            // Authenticate the request
            GenericPrincipal? genericPrincipal = await AuthenticateAsync(content);
            if (genericPrincipal == null)
            {
                return AuthenticateResult.Fail("Invalid username or password");
            }

            // Create authentication ticket
            var ticket = new AuthenticationTicket(genericPrincipal, Options.Scheme);

            return AuthenticateResult.Success(ticket);
        }

        /// <summary>
        /// Authenticates a web request asynchronously.
        /// </summary>
        /// <param name="content">
        /// A string containing the authorization header content.
        /// </param>
        /// <returns>
        /// A principal representing the calling user; otherwise null.
        /// </returns>
        private Task<GenericPrincipal?> AuthenticateAsync(string content)
        {
            GenericPrincipal? genericPrincipal = null;

            if (!string.IsNullOrEmpty(content))
            {                
                StringPair stringPairElements = content.SplitOnce(AuthHeaderSeparator);
                if (stringPairElements != null)
                {
                    string username = stringPairElements.First;
                    string password = stringPairElements.Second;

                    // Get the service account credentials
                    string serviceUsername = _configuration.GetServiceUserName();
                    string servicePassword = _configuration.GetServicePassword();

                    if ((string.Compare(username, serviceUsername) == 0) && (string.Compare(password, servicePassword) == 0))
                    {
                        // Create a principal to represent the service user.
                        genericPrincipal = new GenericPrincipal(new WebIdentity(SpecialIds.TemplateUserId, SpecialStrings.TemplateUser, new string[] { SpecialStrings.ViewTemplateRole, SpecialStrings.ManageTemplateRole }, new string[] { }, content, "basic"), new string[] { SpecialStrings.ViewTemplateRole, SpecialStrings.ManageTemplateRole });
                    }
                }
            }

            return Task.FromResult(genericPrincipal);
        }
    }
}
