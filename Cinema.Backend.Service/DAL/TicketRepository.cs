using Cinema.Backend.Service.Models;
using DnsClient;
using Envista.Core.Common.System;
using MongoDB.Bson;
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
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }

            _addList.Add(ticket);

            return Task.CompletedTask;
        }

        public Task AddAsync(List<Ticket> tickets)
        {
            if (tickets == null)
            {
                throw new ArgumentNullException(nameof(tickets));
            }

            _addList.AddRange(tickets);

            return Task.CompletedTask;
        }

        public async Task<List<Ticket>> GetAsync(int index = 0, int count = 100, string order = "", int direction = 0)
        {
            List<Ticket> tickets = new List<Ticket>();

            if (!string.IsNullOrEmpty(order))
            {
                direction = (direction >= 0) ? 1 : -1;

                // Gets all persons
                var list = await Tickets.Find(FilterDefinition<Ticket>.Empty).Sort(new BsonDocument(order, direction)).Skip(index).Limit(count).ToListAsync();
                tickets = new List<Ticket>(list);
            }
            else
            {
                // Gets all persons
                var list = await Tickets.Find(FilterDefinition<Ticket>.Empty).Skip(index).Limit(count).ToListAsync();
                tickets = new List<Ticket>(list);
            }

            return tickets;
        }

        public async Task<List<Ticket>> GetAsync(string key, string value, int index = 0, int count = 100, string order = "", int direction = 0)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"The specified {nameof(key)} parameter is invalid.");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"The specified {nameof(value)} parameter is invalid.");
            }

            List<Ticket> tickets = new List<Ticket>();

            // Parse the value
            DataParser parser = new DataParser();
            var result = parser.Parse(value);

            // Create the filter
            key = ((key == "id") || (key == "metadata.id")) ? "_id" : key;
            var filter = Builders<Ticket>.Filter.Eq(key, result.Item2);

            if (!string.IsNullOrEmpty(order))
            {
                direction = (direction >= 0) ? 1 : -1;

                var list = await this.Tickets.Find(filter).Sort(new BsonDocument(order, direction)).Skip(index).Limit(count).ToListAsync();
                tickets = new List<Ticket>(list);
            }
            else
            {
                var list = await this.Tickets.Find(filter).Skip(index).Limit(count).ToListAsync();
                tickets = new List<Ticket>(list);
            }

            return tickets;
        }

        public async Task<Ticket> GetAsync(Guid id)
        {
            var filter = Builders<Ticket>.Filter.Eq("_id", id);
            var tickets = await Tickets.FindAsync(filter);
            Ticket ticket = tickets.FirstOrDefault<Ticket>();

            return ticket;
        }

        public async Task<long> GetCountAsync()
        {
            return await Tickets.CountDocumentsAsync(FilterDefinition<Ticket>.Empty);
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
            var filter = Builders<Ticket>.Filter.Eq(key, result.Item2);

            // Get the person count
            long count = await this.Tickets.CountDocumentsAsync(filter);

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

        public Task UpdateAsync(Ticket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }

            _updateList.Add(ticket);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(List<Ticket> tickets)
        {
            if (tickets == null)
            {
                throw new ArgumentNullException(nameof(tickets));
            }

            _updateList.AddRange(tickets);

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
