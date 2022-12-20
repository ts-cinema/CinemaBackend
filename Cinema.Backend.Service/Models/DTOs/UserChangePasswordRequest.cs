using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Cinema.Backend.Service.Models.DTOs
{
    public class UserChangePasswordRequest
    {
        [Required]
        [JsonProperty("current_password")]
        public string CurrentPassword { get; set; }

        [Required]
        [JsonProperty("new_password")]
        public string NewPassword { get; set; }
    }
}
