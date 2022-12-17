using Newtonsoft.Json;

namespace Envista.Core.Common.Security
{
    /// <summary>
    /// Provides the methods and properties for application credentials.
    /// </summary>
    public class Credential
    {
        /// <summary>
        /// Initializes a new instance of the Credential class.
        /// </summary>
        public Credential()
        {
            this.Username = string.Empty;
            this.Password = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the Credential class.
        /// </summary>
        /// <param name="username">
        /// A string containing the username associated with the credentials.
        /// </param>
        /// <param name="password">
        ///  A string containing the password associated with the credentials.
        /// </param>
        public Credential(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        /// <summary>
        /// Gets or set the username associated with the credentials.
        /// </summary>
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or set the password associated with the credentials.
        /// </summary>
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the minimum credential information is valid.
        /// </summary>
        [JsonIgnore]
        public bool IsValid 
        {
            get
            {
                bool valid = false;

                if (!string.IsNullOrEmpty(this.Username))
                {
                    valid = true;
                }

                return valid;
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">
        /// The object to compare with the current object.
        /// </param>
        /// <returns>
        /// true if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if ((obj == null) || (this.GetType() != obj.GetType()))
            {
                return false;
            }

            bool equals = false;

            Credential item = (Credential)obj;

            if ((((this.Username == null) && (item.Username == null)) || (this.Username.Equals(item.Username))) &&
                (((this.Password == null) && (item.Password == null)) || (this.Password.Equals(item.Password))))
            {
                equals = true;
            }

            return equals;
        }

        /// <summary>
        /// Gets the hash code for the current object.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            int hash = base.GetHashCode();

            if (this.Username != null)
            {
                hash ^= this.Username.GetHashCode();
            }

            if (this.Password != null)
            {
                hash ^= this.Password.GetHashCode();
            }

            return hash;
        }
    }
}