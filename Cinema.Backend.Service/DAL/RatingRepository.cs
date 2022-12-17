using Cinema.Backend.Service.Models;
using MongoDB.Driver;
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
            throw new NotImplementedException();
        }

        public Task AddAsync(List<Rating> ratings)
        {
            throw new NotImplementedException();
        }

        public Task<List<Rating>> GetAsync(int index = 0, int count = 100, string order = "", int direction = 0)
        {
            throw new NotImplementedException();
        }

        public Task<List<Rating>> GetAsync(string key, string value, int index = 0, int count = 100, string order = "", int direction = 0)
        {
            throw new NotImplementedException();
        }

        public Task<Rating> GetAsync(Guid id)
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

        public Task UpdateAsync(Rating rating)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(List<Rating> ratings)
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
