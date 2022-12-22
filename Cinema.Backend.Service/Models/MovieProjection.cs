using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Cinema.Backend.Service.Models
{
    public class MovieProjection
    {
        [BsonId]
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [BsonElement("start_time")]
        [JsonProperty("start_time")]
        public DateTime StartTime { get; set; }

        [BsonElement("total_seats")]
        [JsonProperty("total_seats")]
        public int TotalSeats { get; set; }

        [BsonElement("available_seats")]
        [JsonProperty("available_seats")]
        public int AvailableSeats { get; set; }

        [BsonElement("movie_id")]
        [JsonProperty("movie_id")]
        public Guid MovieId { get; set; }
    }
}
