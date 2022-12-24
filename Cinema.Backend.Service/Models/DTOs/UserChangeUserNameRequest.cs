using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Cinema.Backend.Service.Models.DTOs
{
    public class UserChangeEmailRequest
    {
        [Required]
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
