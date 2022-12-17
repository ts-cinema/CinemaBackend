using Cinema.Backend.Service.Models;

namespace Cinema.Backend.Service.DAL
{
    public interface IMovieProjectionRepository
    {
        Task<long> GetCountAsync();

        Task<long> GetCountAsync(string key, string value);

        Task<List<MovieProjection>> GetAsync(int index = 0, int count = 100, string order = "", int direction = 0);

        Task<List<MovieProjection>> GetAsync(string key, string value, int index = 0, int count = 100, string order = "", int direction = 0);

        Task<MovieProjection> GetAsync(Guid id);

        Task AddAsync(MovieProjection movieProjection);

        Task AddAsync(List<MovieProjection> movieProjections);

        Task UpdateAsync(MovieProjection movieProjection);

        Task UpdateAsync(List<MovieProjection> movieProjections);

        Task RemoveAsync(Guid id);

        Task RemoveAsync(List<Guid> ids);
    }
}
