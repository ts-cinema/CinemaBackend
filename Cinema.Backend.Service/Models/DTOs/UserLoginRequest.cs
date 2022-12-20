using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Cinema.Backend.Service.Models.DTOs
{
    public class UserLoginRequest
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
    }
}
