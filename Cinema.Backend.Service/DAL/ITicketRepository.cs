using Cinema.Backend.Service.Models;

namespace Cinema.Backend.Service.DAL
{
    public interface ITicketRepository
    {
        Task<long> GetCountAsync();

        Task<long> GetCountAsync(string key, string value);

        Task<List<Ticket>> GetAsync(int index = 0, int count = 100, string order = "", int direction = 0);

        Task<List<Ticket>> GetAsync(string key, string value, int index = 0, int count = 100, string order = "", int direction = 0);

        Task<Ticket> GetAsync(Guid id);

        Task AddAsync(Ticket ticket);

        Task AddAsync(List<Ticket> tickets);

        Task UpdateAsync(Ticket ticket);

        Task UpdateAsync(List<Ticket> tickets);

        Task RemoveAsync(Guid id);

        Task RemoveAsync(List<Guid> ids);
    }
}
