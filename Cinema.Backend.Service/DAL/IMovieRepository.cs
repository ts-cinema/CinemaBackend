using Cinema.Backend.Service.Models;

namespace Cinema.Backend.Service.DAL
{
    public interface IMovieRepository
    {
        Task<long> GetCountAsync();

        Task<long> GetCountAsync(string key, string value);

        Task<List<Movie>> GetAsync(int index = 0, int count = 100, string order = "", int direction = 0);

        Task<List<Movie>> GetAsync(string key, string value, int index = 0, int count = 100, string order = "", int direction = 0);

        Task<Movie> GetAsync(Guid id);

        Task AddAsync(Movie movie);

        Task AddAsync(List<Movie> movies);

        Task UpdateAsync(Movie movie);

        Task UpdateAsync(List<Movie> movies);

        Task RemoveAsync(Guid id);

        Task RemoveAsync(List<Guid> ids);
    }
}
