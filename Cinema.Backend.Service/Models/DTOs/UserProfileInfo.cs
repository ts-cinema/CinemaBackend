using Newtonsoft.Json;

namespace Cinema.Backend.Service.Models.DTOs
{
    public class UserProfileInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("birth_date")]
        public DateTime BirthDate { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; } = string.Empty;

        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;
    }
}
