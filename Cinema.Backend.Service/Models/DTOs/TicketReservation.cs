using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Cinema.Backend.Service.Models.DTOs
{
    public class TicketReservation
    {
        [BsonElement("name")]
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("price")]
        [JsonProperty("price")]
        public double Price { get; set; }

        [BsonElement("quantity")]
        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [BsonElement("movie_projection_id")]
        [JsonProperty("movie_projection_id")]
        public Guid MovieProjectionId { get; set; }

        [BsonElement("user_id")]
        [JsonProperty("user_id")]
        public Guid UserId { get; set; }
    }
}
