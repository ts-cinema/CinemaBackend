using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Cinema.Backend.Service.Models
{
    public class Rating
    {
        [BsonId]
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [BsonElement("movie_id")]
        [JsonProperty("movie_id")]
        public Guid MovieId { get; set; }

        [BsonElement("value")]
        [JsonProperty("value")]
        public double Value { get; set; }

        [BsonElement("user_id")]
        [JsonProperty("user_id")]
        public Guid UserId { get; set; }
    }
}
