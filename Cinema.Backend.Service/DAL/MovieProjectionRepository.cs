using Cinema.Backend.Service.Models;
using DnsClient;
using Envista.Core.Common.System;
using MongoDB.Bson;
using MongoDB.Driver;
using Template.Service.DAL;

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
            if (movieProjection == null)
            {
                throw new ArgumentNullException(nameof(movieProjection));
            }

            _addList.Add(movieProjection);

            return Task.CompletedTask;
        }

        public Task AddAsync(List<MovieProjection> movieProjections)
        {
            if (movieProjections == null)
            {
                throw new ArgumentNullException(nameof(movieProjections));
            }

            _addList.AddRange(movieProjections);

            return Task.CompletedTask;
        }

        public async Task<List<MovieProjection>> GetAsync(int index = 0, int count = 100, string order = "", int direction = 0)
        {
            List<MovieProjection> movieProjections = new List<MovieProjection>();

            if (!string.IsNullOrEmpty(order))
            {
                direction = (direction >= 0) ? 1 : -1;

                var list = await MovieProjections.Find(FilterDefinition<MovieProjection>.Empty).Sort(new BsonDocument(order, direction)).Skip(index).Limit(count).ToListAsync();
                movieProjections = new List<MovieProjection>(list);
            }
            else
            {
                var list = await MovieProjections.Find(FilterDefinition<MovieProjection>.Empty).Skip(index).Limit(count).ToListAsync();
                movieProjections = new List<MovieProjection>(list);
            }

            return movieProjections;
        }

        public async Task<List<MovieProjection>> GetAsync(string key, string value, int index = 0, int count = 100, string order = "", int direction = 0)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"The specified {nameof(key)} parameter is invalid.");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"The specified {nameof(value)} parameter is invalid.");
            }

            var movieProjections = new List<MovieProjection>();

            // Parse the value
            DataParser parser = new DataParser();
            var result = parser.Parse(value);

            // Create the filter
            key = ((key == "id") || (key == "metadata.id")) ? "_id" : key;
            var filter = Builders<MovieProjection>.Filter.Eq(key, result.Item2);

            if (!string.IsNullOrEmpty(order))
            {
                direction = (direction >= 0) ? 1 : -1;

                var list = await this.MovieProjections.Find(filter).Sort(new BsonDocument(order, direction)).Skip(index).Limit(count).ToListAsync();
                movieProjections = new List<MovieProjection>(list);
            }
            else
            {
                var list = await this.MovieProjections.Find(filter).Skip(index).Limit(count).ToListAsync();
                movieProjections = new List<MovieProjection>(list);
            }

            return movieProjections;
        }

        public async Task<MovieProjection> GetAsync(Guid id)
        {
            var filter = Builders<MovieProjection>.Filter.Eq("_id", id);
            var movieProjections = await MovieProjections.FindAsync(filter);
            MovieProjection movieProjection = movieProjections.FirstOrDefault<MovieProjection>();

            return movieProjection;
        }

        public async Task<long> GetCountAsync()
        {
            return await MovieProjections.CountDocumentsAsync(FilterDefinition<MovieProjection>.Empty);
        }

        public async Task<long> GetCountAsync(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"The specified {nameof(key)} parameter is invalid.");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"The specified {nameof(value)} parameter is invalid.");
            }

            // Parse the value
            DataParser parser = new DataParser();
            var result = parser.Parse(value);

            // Build the filter
            var filter = Builders<MovieProjection>.Filter.Eq(key, result.Item2);

            // Get the person count
            long count = await this.MovieProjections.CountDocumentsAsync(filter);

            return count;
        }

        public Task RemoveAsync(Guid id)
        {
            _removeList.Add(id);

            return Task.CompletedTask;
        }

        public Task RemoveAsync(List<Guid> ids)
        {
            _removeList.AddRange(ids);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(MovieProjection movieProjection)
        {
            if (movieProjection == null)
            {
                throw new ArgumentNullException(nameof(movieProjection));
            }

            _updateList.Add(movieProjection);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(List<MovieProjection> movieProjections)
        {
            if (movieProjections == null)
            {
                throw new ArgumentNullException(nameof(movieProjections));
            }

            _updateList.AddRange(movieProjections);

            return Task.CompletedTask;
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
