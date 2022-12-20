using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Cinema.Backend.Service.Models.DTOs
{
    public class UserChangeUserNameRequest
    {
        [Required]
        [JsonProperty("username")]
        public string UserName { get; set; }
    }
}
