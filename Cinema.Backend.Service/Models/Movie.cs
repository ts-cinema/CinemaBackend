using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Cinema.Backend.Service.Models
{
    public class Movie
    {
        [BsonId]
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [BsonElement("title")]
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("description")]
        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("genre")]
        [JsonProperty("genre")]
        public string Genre { get; set; } = string.Empty;

        [BsonElement("release_date")]
        [JsonProperty("release_date")]
        public DateTime ReleaseDate { get; set; }

        [BsonElement("cover_url")]
        [JsonProperty("cover_url")]
        public string CoverUrl { get; set; } = string.Empty;
    }
}
