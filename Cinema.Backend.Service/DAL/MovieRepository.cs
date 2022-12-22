using Cinema.Backend.Service.Models;
using DnsClient;
using Envista.Core.Common.System;
using MongoDB.Bson;
using MongoDB.Driver;
using Template.Service.DAL;

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
            if (movie == null)
            {
                throw new ArgumentNullException(nameof(movie));
            }

            _addList.Add(movie);

            return Task.CompletedTask;
        }

        public Task AddAsync(List<Movie> movies)
        {
            if (movies == null)
            {
                throw new ArgumentNullException(nameof(movies));
            }

            _addList.AddRange(movies);

            return Task.CompletedTask;
        }

        public async Task<List<Movie>> GetAsync(int index = 0, int count = 100, string order = "", int direction = 0)
        {
            List<Movie> movies = new List<Movie>();

            if (!string.IsNullOrEmpty(order))
            {
                direction = (direction >= 0) ? 1 : -1;

                // Gets all persons
                var list = await Movies.Find(FilterDefinition<Movie>.Empty).Sort(new BsonDocument(order, direction)).Skip(index).Limit(count).ToListAsync();
                movies = new List<Movie>(list);
            }
            else
            {
                // Gets all persons
                var list = await Movies.Find(FilterDefinition<Movie>.Empty).Skip(index).Limit(count).ToListAsync();
                movies = new List<Movie>(list);
            }

            return movies;
        }

        public async Task<List<Movie>> GetAsync(string key, string value, int index = 0, int count = 100, string order = "", int direction = 0)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"The specified {nameof(key)} parameter is invalid.");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"The specified {nameof(value)} parameter is invalid.");
            }

            List<Movie> movies = new List<Movie>();

            // Parse the value
            DataParser parser = new DataParser();
            var result = parser.Parse(value);

            // Create the filter
            key = ((key == "id") || (key == "metadata.id")) ? "_id" : key;
            var filter = Builders<Movie>.Filter.Eq(key, result.Item2);

            if (!string.IsNullOrEmpty(order))
            {
                direction = (direction >= 0) ? 1 : -1;

                var list = await this.Movies.Find(filter).Sort(new BsonDocument(order, direction)).Skip(index).Limit(count).ToListAsync();
                movies = new List<Movie>(list);
            }
            else
            {
                var list = await this.Movies.Find(filter).Skip(index).Limit(count).ToListAsync();
                movies = new List<Movie>(list);
            }

            return movies;
        }

        public async Task<Movie> GetAsync(Guid id)
        {
            var filter = Builders<Movie>.Filter.Eq("_id", id);
            var movies = await Movies.FindAsync(filter);
            Movie movie = movies.FirstOrDefault<Movie>();

            return movie;
        }

        public async Task<long> GetCountAsync()
        {
            return await Movies.CountDocumentsAsync(FilterDefinition<Movie>.Empty);
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
            var filter = Builders<Movie>.Filter.Eq(key, result.Item2);

            // Get the person count
            long count = await this.Movies.CountDocumentsAsync(filter);

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

        public Task UpdateAsync(Movie movie)
        {
            if (movie == null)
            {
                throw new ArgumentNullException(nameof(movie));
            }

            _updateList.Add(movie);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(List<Movie> movies)
        {
            if (movies == null)
            {
                throw new ArgumentNullException(nameof(movies));
            }

            _updateList.AddRange(movies);

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
