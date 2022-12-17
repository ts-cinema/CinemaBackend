using Newtonsoft.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Template.Service.Models.Person
{
    /// <summary>
    /// Describes information about a person.
    /// </summary>
    public class Person
    {
        /// <summary>
        /// Gets or sets the ID of the person.
        /// </summary>
        [BsonId]
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// Gets or sets the person's first name.
        /// </summary>
        [BsonElement("firstName")]
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the person's last name.
        /// </summary>
        [BsonElement("lastName")]
        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the person's age.
        /// </summary>
        [BsonElement("age")]
        [JsonProperty(PropertyName = "age")]
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets the organizations the person is enrolled in.
        /// </summary>
        [BsonElement("organizations")]
        [JsonProperty(PropertyName = "organizations")]
        public List<string> Organizations { get; set; } = new List<string>();

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

            Person person = (Person)obj;

            if ((this.Id.Equals(person.Id)) &&
                ((((this.FirstName == null) && (person.FirstName == null))) || (this.FirstName.Equals(person.FirstName))) &&
                ((((this.LastName == null) && (person.LastName == null))) || (this.LastName.Equals(person.LastName))) &&
                (this.Age.Equals(person.Age)) &&
                (((this.Organizations == null) && (person.Organizations == null)) || (this.Organizations.Equals(person.Organizations))))
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

            if (this.FirstName != null)
            {
                hash ^= this.FirstName.GetHashCode();
            }

            if (this.LastName != null)
            {
                hash ^= this.LastName.GetHashCode();
            }

            hash ^= this.Age.GetHashCode();

            if (this.Organizations != null)
            {
                hash ^= this.Organizations.GetHashCode();
            }

            return hash;
        }
    }
}
