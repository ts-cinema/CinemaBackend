using Cinema.Backend.Service.Models;

namespace Cinema.Backend.Service.DAL
{
    public interface IRatingRepository
    {
        Task<long> GetCountAsync();

        Task<long> GetCountAsync(string key, string value);

        Task<List<Rating>> GetAsync(int index = 0, int count = 100, string order = "", int direction = 0);

        Task<List<Rating>> GetAsync(string key, string value, int index = 0, int count = 100, string order = "", int direction = 0);

        Task<Rating> GetAsync(Guid id);

        Task AddAsync(Rating rating);

        Task AddAsync(List<Rating> ratings);

        Task UpdateAsync(Rating rating);

        Task UpdateAsync(List<Rating> ratings);

        Task RemoveAsync(Guid id);

        Task RemoveAsync(List<Guid> ids);
    }
}
