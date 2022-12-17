using Cinema.Backend.Service.Models;
using MongoDB.Driver;
using Template.Service.DAL;
using Template.Service.Models.Organization;

namespace Cinema.Backend.Service.DAL
{
    public class MovieProjectionRepository : IMovieProjectionRepository
    {
        private readonly MongoContext _context;
        private readonly string _collectionName = "movie-projections";

        private readonly List<MovieProjection> _addList = new List<MovieProjection>();
        private readonly List<MovieProjection> _updateList = new List<MovieProjection>();
        private readonly List<Guid> _removeList = new List<Guid>();

        /// <summary>
        /// Initialize a new instance of the Organizations class.
        /// </summary>
        public MovieProjectionRepository(MongoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the organizations collection.
        /// </summary>
        private IMongoCollection<MovieProjection> MovieProjections
        {
            get
            {
                return _context.MongoDatabase.GetCollection<MovieProjection>(this._collectionName);
            }
        }

        public Task AddAsync(MovieProjection movieProjection)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(List<MovieProjection> movieProjection)
        {
            throw new NotImplementedException();
        }

        public Task<List<MovieProjection>> GetAsync(int index = 0, int count = 100, string order = "", int direction = 0)
        {
            throw new NotImplementedException();
        }

        public Task<List<MovieProjection>> GetAsync(string key, string value, int index = 0, int count = 100, string order = "", int direction = 0)
        {
            throw new NotImplementedException();
        }

        public Task<MovieProjection> GetAsync(Guid id)
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

        public Task UpdateAsync(MovieProjection movieProjection)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(List<MovieProjection> movieProjections)
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

        private async Task<int> CreateAsync(List<MovieProjection> movieProjections)
        {
            if (movieProjections == null)
            {
                throw new ArgumentNullException(nameof(movieProjections));
            }

            int count = movieProjections.Count;

            // Insert the organizations
            await MovieProjections.InsertManyAsync(movieProjections);

            return count;
        }

        private async Task<int> CreateUpdateAsync(List<MovieProjection> movieProjections)
        {
            if (movieProjections == null)
            {
                throw new ArgumentNullException(nameof(movieProjections));
            }

            // Build the request
            var requests = new List<WriteModel<MovieProjection>>();
            foreach (var movieProjection in movieProjections)
            {
                var filter = Builders<MovieProjection>.Filter.Eq("_id", movieProjection.Id);
                var model = new ReplaceOneModel<MovieProjection>(filter, movieProjection) { IsUpsert = true };
                requests.Add(model);
            }

            int count = 0;

            // Create/update the organizations in bulk
            var result = await MovieProjections.BulkWriteAsync(requests);
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
            var filter = Builders<MovieProjection>.Filter.In(x => x.Id, ids);
            var result = await MovieProjections.DeleteManyAsync(filter);
            if ((result != null) && (result.IsAcknowledged))
            {
                count = (int)result.DeletedCount;
            }

            return count;
        }
    }
}
