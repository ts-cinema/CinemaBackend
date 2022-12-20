using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Cinema.Backend.Service.Models.DTOs
{
    public class UserRegistrationRequest
    {
        [Required]
        [StringLength(50)]
        [EmailAddress]
        [JsonProperty("email")]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 8)]
        [JsonProperty("password")]
        public string Password { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 8)]
        [JsonProperty("confirm_password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Required]
        [JsonProperty("birth_date")]
        public DateTime BirthDate { get; set; }

        [Required]
        [JsonProperty("address")]
        public string Address { get; set; }
    }
}
