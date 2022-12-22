using Cinema.Backend.Service.Models;
using Envista.Core.Common.System;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using Template.Service.DAL;

namespace Cinema.Backend.Service.DAL
{
    public class RatingRepository : IRatingRepository
    {
        private readonly MongoContext _context;
        private readonly string _collectionName = "ratings";

        private readonly List<Rating> _addList = new List<Rating>();
        private readonly List<Rating> _updateList = new List<Rating>();
        private readonly List<Guid> _removeList = new List<Guid>();

        /// <summary>
        /// Initialize a new instance of the Organizations class.
        /// </summary>
        public RatingRepository(MongoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the organizations collection.
        /// </summary>
        private IMongoCollection<Rating> Ratings
        {
            get
            {
                return _context.MongoDatabase.GetCollection<Rating>(this._collectionName);
            }
        }

        public Task AddAsync(Rating rating)
        {
            if (rating == null)
            {
                throw new ArgumentNullException(nameof(rating));
            }

            _addList.Add(rating);

            return Task.CompletedTask;
        }

        public Task AddAsync(List<Rating> ratings)
        {
            if (ratings == null)
            {
                throw new ArgumentNullException(nameof(ratings));
            }

            _addList.AddRange(ratings);

            return Task.CompletedTask;
        }

        public async Task<List<Rating>> GetAsync(int index = 0, int count = 100, string order = "", int direction = 0)
        {
            List<Rating> ratings = new List<Rating>();

            if (!string.IsNullOrEmpty(order))
            {
                direction = (direction >= 0) ? 1 : -1;

                // Gets all persons
                var list = await Ratings.Find(FilterDefinition<Rating>.Empty).Sort(new BsonDocument(order, direction)).Skip(index).Limit(count).ToListAsync();
                ratings = new List<Rating>(list);
            }
            else
            {
                // Gets all persons
                var list = await Ratings.Find(FilterDefinition<Rating>.Empty).Skip(index).Limit(count).ToListAsync();
                ratings = new List<Rating>(list);
            }

            return ratings;
        }

        public async Task<List<Rating>> GetAsync(string key, string value, int index = 0, int count = 100, string order = "", int direction = 0)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"The specified {nameof(key)} parameter is invalid.");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"The specified {nameof(value)} parameter is invalid.");
            }

            List<Rating> ratings = new List<Rating>();

            // Parse the value
            DataParser parser = new DataParser();
            var result = parser.Parse(value);

            // Create the filter
            key = ((key == "id") || (key == "metadata.id")) ? "_id" : key;
            var filter = Builders<Rating>.Filter.Eq(key, result.Item2);

            if (!string.IsNullOrEmpty(order))
            {
                direction = (direction >= 0) ? 1 : -1;

                var list = await this.Ratings.Find(filter).Sort(new BsonDocument(order, direction)).Skip(index).Limit(count).ToListAsync();
                ratings = new List<Rating>(list);
            }
            else
            {
                var list = await this.Ratings.Find(filter).Skip(index).Limit(count).ToListAsync();
                ratings = new List<Rating>(list);
            }

            return ratings;
        }

        public async Task<Rating> GetAsync(Guid id)
        {
            var filter = Builders<Rating>.Filter.Eq("_id", id);
            var ratings = await Ratings.FindAsync(filter);
            Rating rating = ratings.FirstOrDefault<Rating>();

            return rating;
        }

        public async Task<long> GetCountAsync()
        {
            return await Ratings.CountDocumentsAsync(FilterDefinition<Rating>.Empty);
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
            var filter = Builders<Rating>.Filter.Eq(key, result.Item2);

            // Get the person count
            long count = await this.Ratings.CountDocumentsAsync(filter);

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

        public Task UpdateAsync(Rating rating)
        {
            if (rating == null)
            {
                throw new ArgumentNullException(nameof(rating));
            }

            _updateList.Add(rating);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(List<Rating> ratings)
        {
            if (ratings == null)
            {
                throw new ArgumentNullException(nameof(ratings));
            }

            _updateList.AddRange(ratings);

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

        private async Task<int> CreateAsync(List<Rating> ratings)
        {
            if (ratings == null)
            {
                throw new ArgumentNullException(nameof(ratings));
            }

            int count = ratings.Count;

            // Insert the organizations
            await Ratings.InsertManyAsync(ratings);

            return count;
        }

        private async Task<int> CreateUpdateAsync(List<Rating> ratings)
        {
            if (ratings == null)
            {
                throw new ArgumentNullException(nameof(ratings));
            }

            // Build the request
            var requests = new List<WriteModel<Rating>>();
            foreach (var rating in ratings)
            {
                var filter = Builders<Rating>.Filter.Eq("_id", rating.Id);
                var model = new ReplaceOneModel<Rating>(filter, rating) { IsUpsert = true };
                requests.Add(model);
            }

            int count = 0;

            // Create/update the organizations in bulk
            var result = await Ratings.BulkWriteAsync(requests);
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
            var filter = Builders<Rating>.Filter.In(x => x.Id, ids);
            var result = await Ratings.DeleteManyAsync(filter);
            if ((result != null) && (result.IsAcknowledged))
            {
                count = (int)result.DeletedCount;
            }

            return count;
        }
    }
}
