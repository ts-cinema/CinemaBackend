using Cinema.Backend.Service.Models;
using MongoDB.Driver;
using Template.Service.DAL;

namespace Cinema.Backend.Service.DAL
{
    public class TicketRepository : ITicketRepository
    {
        private readonly MongoContext _context;
        private readonly string _collectionName = "tickets";

        private readonly List<Ticket> _addList = new List<Ticket>();
        private readonly List<Ticket> _updateList = new List<Ticket>();
        private readonly List<Guid> _removeList = new List<Guid>();

        /// <summary>
        /// Initialize a new instance of the Organizations class.
        /// </summary>
        public TicketRepository(MongoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the organizations collection.
        /// </summary>
        private IMongoCollection<Ticket> Tickets
        {
            get
            {
                return _context.MongoDatabase.GetCollection<Ticket>(this._collectionName);
            }
        }

        public Task AddAsync(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(List<Ticket> tickets)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetAsync(int index = 0, int count = 100, string order = "", int direction = 0)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetAsync(string key, string value, int index = 0, int count = 100, string order = "", int direction = 0)
        {
            throw new NotImplementedException();
        }

        public Task<Ticket> GetAsync(Guid id)
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

        public Task UpdateAsync(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(List<Ticket> tickets)
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

        private async Task<int> CreateAsync(List<Ticket> tickets)
        {
            if (tickets == null)
            {
                throw new ArgumentNullException(nameof(tickets));
            }

            int count = tickets.Count;

            // Insert the organizations
            await Tickets.InsertManyAsync(tickets);

            return count;
        }

        private async Task<int> CreateUpdateAsync(List<Ticket> tickets)
        {
            if (tickets == null)
            {
                throw new ArgumentNullException(nameof(tickets));
            }

            // Build the request
            var requests = new List<WriteModel<Ticket>>();
            foreach (var ticket in tickets)
            {
                var filter = Builders<Ticket>.Filter.Eq("_id", ticket.Id);
                var model = new ReplaceOneModel<Ticket>(filter, ticket) { IsUpsert = true };
                requests.Add(model);
            }

            int count = 0;

            // Create/update the organizations in bulk
            var result = await Tickets.BulkWriteAsync(requests);
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
            var filter = Builders<Ticket>.Filter.In(x => x.Id, ids);
            var result = await Tickets.DeleteManyAsync(filter);
            if ((result != null) && (result.IsAcknowledged))
            {
                count = (int)result.DeletedCount;
            }

            return count;
        }
    }
}
