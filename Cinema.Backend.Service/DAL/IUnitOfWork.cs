using Cinema.Backend.Service.DAL;

namespace Template.Service.DAL
{
    /// <summary>
    /// Represents a unit of work interface for the service.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IMovieRepository Movies { get; }

        IMovieProjectionRepository MovieProjections { get; }

        IRatingRepository Ratings { get; }

        ITicketRepository Tickets { get; }
    }
}
