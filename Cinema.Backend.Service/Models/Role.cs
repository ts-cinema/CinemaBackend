using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Cinema.Backend.Service.Models
{
    [CollectionName("roles")]
    public class Role : MongoIdentityRole<Guid>
    {

    }
}
