using Cinema.Backend.Service.Models;
using MongoDB.Driver;
using Template.Service.DAL;
using Template.Service.Models.Organization;

namespace Cinema.Backend.Service.DAL
{
    public class MovieRepository : IMovieRepository
    {
        private readonly MongoContext _context;
        private readonly string _collectionName = "movies";

        private readonly List<Movie> _addList = new List<Movie>();
        private readonly List<Movie> _updateList = new List<Movie>();
        private readonly List<Guid> _removeList = new List<Guid>();

        /// <summary>
        /// Initialize a new instance of the Organizations class.
        /// </summary>
        public MovieRepository(MongoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the organizations collection.
        /// </summary>
        private IMongoCollection<Movie> Movies
        {
            get
            {
                return _context.MongoDatabase.GetCollection<Movie>(this._collectionName);
            }
        }

        public Task AddAsync(Movie movie)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(List<Movie> movies)
        {
            throw new NotImplementedException();
        }

        public Task<List<Movie>> GetAsync(int index = 0, int count = 100, string order = "", int direction = 0)
        {
            throw new NotImplementedException();
        }

        public Task<List<Movie>> GetAsync(string key, string value, int index = 0, int count = 100, string order = "", int direction = 0)
        {
            throw new NotImplementedException();
        }

        public Task<Movie> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<long> GetCountAsync()
        {
            throw new NotImplementedException();
        }

        public Task<long> GetCountAsync(string key, string value)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(List<Guid> ids)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Movie movie)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(List<Movie> movies)
        {
            throw new NotImplementedException();
        }

        internal async Task<int> CommitAsync()
        {
            int count = 0;

            if (_addList.Count > 0)
            {
                // Create the specified organizations in the repository
                count += await CreateAsync(_addList);
                _addList.Clear();
            }

            if (_updateList.Count > 0)
            {
                // Create or update the specified organizations within the repository
                count += await CreateUpdateAsync(_updateList);
                _updateList.Clear();
            }

            if (_removeList.Count > 0)
            {
                // Delete the specified organizations from the repository
                count += await DeleteAsync(_removeList);
                _removeList.Clear();
            }

            return count;
        }

        private async Task<int> CreateAsync(List<Movie> movies)
        {
            if (movies == null)
            {
                throw new ArgumentNullException(nameof(movies));
            }

            int count = movies.Count;

            // Insert the organizations
            await Movies.InsertManyAsync(movies);

            return count;
        }

        private async Task<int> CreateUpdateAsync(List<Movie> movies)
        {
            if (movies == null)
            {
                throw new ArgumentNullException(nameof(movies));
            }

            // Build the request
            var requests = new List<WriteModel<Movie>>();
            foreach (var movie in movies)
            {
                var filter = Builders<Movie>.Filter.Eq("_id", movie.Id);
                var model = new ReplaceOneModel<Movie>(filter, movie) { IsUpsert = true };
                requests.Add(model);
            }

            int count = 0;

            // Create/update the organizations in bulk
            var result = await Movies.BulkWriteAsync(requests);
            if ((result != null) && (result.IsAcknowledged))
            {
                count = (int)result.InsertedCount + (int)result.ModifiedCount + result.Upserts.Count;
            }

            return count;
        }

        private async Task<int> DeleteAsync(List<Guid> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            int count = 0;

            // Delete the organizations
            var filter = Builders<Movie>.Filter.In(x => x.Id, ids);
            var result = await Movies.DeleteManyAsync(filter);
            if ((result != null) && (result.IsAcknowledged))
            {
                count = (int)result.DeletedCount;
            }

            return count;
        }
    }
}
