using Cinema.Backend.Service.DAL;

namespace Template.Service.DAL
{
    /// <summary>
    /// Represents the unit of work for the service.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        readonly MongoContext _context;
        private readonly MovieRepository _movies;
        private readonly MovieProjectionRepository _movieProjections;
        private readonly TicketRepository _tickets;
        private readonly RatingRepository _ratings;
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the UnitOfWork class.
        /// </summary>
        public UnitOfWork(string connectionString, string databaseName)
        {
            _context = new MongoContext(connectionString, databaseName);
            _movies = new MovieRepository(_context);
            _movieProjections = new MovieProjectionRepository(_context);
            _tickets = new TicketRepository(_context);
            _ratings = new RatingRepository(_context);
        }

        /// <summary>
        /// Releases any unmanaged resources and optionally any managed resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            // Remove the object off the finalization queue and prevent finalization code for this object from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases any unmanaged resources and optionally any managed resources.
        /// </summary>
        /// <param name="disposing">
        /// true indicates whether method has been invoked by user code; otherwise false indicates that the method has been invoked by the run-time.
        /// </param>
        public void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Only dispose of managed resources if invoked by user code
                if (disposing)
                {
                    _context?.Dispose();
                }

                this._disposed = true;
            }
        }

        public IMovieRepository Movies
        {
            get
            {
                return _movies as IMovieRepository;
            }
        }

        public IMovieProjectionRepository MovieProjections
        {
            get
            {
                return _movieProjections as IMovieProjectionRepository;
            }
        }

        public IRatingRepository Ratings
        {
            get
            {
                return _ratings as IRatingRepository;
            }
        }

        public ITicketRepository Tickets
        {
            get
            {
                return _tickets as ITicketRepository;
            }
        }

        /// <summary>
        /// Commits the current unit of work.
        /// </summary>
        /// <returns>
        /// Returns the number of changes committed within the unit of work.
        /// </returns>
        public async Task<int> CommitAsync()
        {
            int count = 0;

            count += await _movieProjections.CommitAsync();
            count += await _movies.CommitAsync();
            count += await _ratings.CommitAsync();
            count += await _tickets.CommitAsync();

            return count;
        }
    }
}
