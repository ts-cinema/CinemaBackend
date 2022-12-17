using Newtonsoft.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Template.Service.Models.Organization
{
    /// <summary>
    /// Describes information about a organization.
    /// </summary>
    public class Organization
    {
        /// <summary>
        /// Gets or sets the ID of the organization.
        /// </summary>
        [BsonId]
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// Gets or sets the organization's name.
        /// </summary>
        [BsonElement("name")]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the organization's code.
        /// </summary>
        [BsonElement("code")]
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">
        /// The pair to compare with the current object.
        /// </param>
        /// <returns>
        /// true if the specified pair is equal to the current object; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if ((obj == null) || (this.GetType() != obj.GetType()))
            {
                return false;
            }

            bool equals = false;

            Organization organization = (Organization)obj;

            if ((this.Id.Equals(organization.Id)) &&
                ((((this.Name == null) && (organization.Name == null))) || (this.Name.Equals(organization.Name))) &&
                ((((this.Code == null) && (organization.Code == null))) || (this.Code.Equals(organization.Code))))
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
            int hash = 0;

            hash ^= this.Id.GetHashCode();

            if (this.Name != null)
            {
                hash ^= this.Name.GetHashCode();
            }

            if (this.Code != null)
            {
                hash ^= this.Code.GetHashCode();
            }

            return hash;
        }
    }
}
