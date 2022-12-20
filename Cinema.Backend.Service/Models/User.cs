using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Cinema.Backend.Service.Models
{
    [CollectionName("users")]
    public class User : MongoIdentityUser<Guid>
    {
        public string Name { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        public string Address { get; set; } = string.Empty;
    }
}
