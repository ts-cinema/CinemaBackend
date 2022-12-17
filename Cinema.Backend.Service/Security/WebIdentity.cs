using System.Security.Principal;

namespace Template.Service.Security
{
    /// <summary>
    /// Represents a web identity.
    /// </summary>
    public class WebIdentity : GenericIdentity 
    {
        /// <summary>
        /// Creates an instance of the WebIdentity class.
        /// </summary>
        /// <param name="identityId">
        /// the ID of the identity associated with the resquest.
        /// </param>
        /// <param name="identityName">
        /// A string containing the name of the identity associated with the request.
        /// </param>
        /// <param name="roles">
        /// An array of strings containing the names of the roles associated with the identity.
        /// </param>
        /// <param name="groups">
        /// An array of strings containing the names of the groups associated with the identity.
        /// </param>
        /// <param name="token">
        /// A string containing the security token associated with the identity.
        /// </param>
        /// <param name="type">
        /// A string containing the type of authentication used to identify the identity.
        /// </param>
        public WebIdentity(Guid identityId, string identityName, string[] roles, string[] groups, string token, string type)
            : base(identityName, type)
        {
            this.IdentityId = identityId;
            this.IdentityName = identityName;
            this.Roles = roles;
            this.Groups = groups;
            this.Token = token;
        }

        /// <summary>
        /// Gets the ID associated with the identity.
        /// </summary>
        public Guid IdentityId { get; } = Guid.Empty;
        
        /// <summary>
        /// Gets the name associated with the identity.
        /// </summary>
        public string IdentityName { get; } = string.Empty;
        
        /// <summary>
        /// Gets the roles associated with the identity.
        /// </summary>
        public string[] Roles { get; } = null;
        
        /// <summary>
        /// Gets the groups associated with the identity.
        /// </summary>
        public string[] Groups { get; } = null;
        
        /// <summary>
        /// Gets the token associated with the identity.
        /// </summary>
        public string Token { get; } = string.Empty;
    }
}
