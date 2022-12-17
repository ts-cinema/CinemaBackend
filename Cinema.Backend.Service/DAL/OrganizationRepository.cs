using MongoDB.Bson;
using MongoDB.Driver;
using Template.Service.Models.Organization;
using Envista.Core.Common.System;

namespace Template.Service.DAL
{
    /// <summary>
    ///  Represents the base methods and properties for managing a organization repository.
    /// </summary>
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly MongoContext _context;
        private readonly string _collectionName = "organizations";

        private readonly OrganizationList _addList = new OrganizationList();
        private readonly OrganizationList _updateList = new OrganizationList();
        private readonly List<Guid> _removeList = new List<Guid>();

        /// <summary>
        /// Initialize a new instance of the Organizations class.
        /// </summary>
        public OrganizationRepository(MongoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the organizations collection.
        /// </summary>
        private IMongoCollection<Organization> Organizations
        {
            get
            {
                return _context.MongoDatabase.GetCollection<Organization>(this._collectionName);
            }
        }

        /// <summary>
        /// Get's the number of organizations within the repository.
        /// </summary>
        /// <returns>
        /// A count of the number of organizations within the repocitory.
        /// </returns>
        public async Task<long> GetCountAsync()
        {
            return await Organizations.CountDocumentsAsync(FilterDefinition<Organization>.Empty);
        }

        /// <summary>
        /// Get's the number of organizations with the specified key and value within the repository.
        /// </summary>
        /// <param name="key">
        /// A string containing the key to examine.
        /// </param>
        /// <param name="value">
        /// A string containing the value to examine.
        /// </param>
        /// <returns>
        /// A count of the number of organizations that match the specified key and value within the repository.
        /// </returns>
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
            var filter = Builders<Organization>.Filter.Eq(key, result.Item2);

            // Get the organization count
            long count = await this.Organizations.CountDocumentsAsync(filter);

            return count;
        }

        /// <summary>
        /// Gets the list of organizations within the repository.
        /// </summary>
        /// <param name="index">
        /// The starting index of the organizations to include in the list.
        /// </param>
        /// <param name="count">
        /// The maximum number of organizations to return.
        /// </param>
        /// <param name="order">
        /// A string containing the name of the attribute to sort on.
        /// </param>
        /// <param name="direction">
        /// An integer specifying the direction of the sort. Positive or zero is ascending; negative is descending.
        /// </param>
        /// <returns>
        /// A list of all organizations.
        /// </returns>
        public async Task<OrganizationList> GetAsync(int index = 0, int count = 100, string order = "", int direction = 0)
        {
            OrganizationList organizations = new OrganizationList();

            if (!string.IsNullOrEmpty(order))
            {
                direction = (direction >= 0) ? 1 : -1;

                // Gets all organizations
                var list = await Organizations.Find(FilterDefinition<Organization>.Empty).Sort(new BsonDocument(order, direction)).Skip(index).Limit(count).ToListAsync();
                organizations = new OrganizationList(list);
            }
            else
            {
                // Gets all organizations
                var list = await Organizations.Find(FilterDefinition<Organization>.Empty).Skip(index).Limit(count).ToListAsync();
                organizations = new OrganizationList(list);
            }

            return organizations;
        }

        /// <summary>
        /// Gets the list of organizations that match the specified key and value within the repository.
        /// </summary>
        /// <param name="key">
        /// A string containing the key to examine.
        /// </param>
        /// <param name="value">
        /// A string containing the value to examine.
        /// </param>
        /// <param name="index">
        /// The starting index of the organizations to include in the list.
        /// </param>
        /// <param name="count">
        /// The maximum number of organizations to return.
        /// </param>
        /// <param name="order">
        /// A string containing the name of the attribute to sort on.
        /// </param>
        /// <param name="direction">
        /// An integer specifying the direction of the sort. Positive or zero is ascending; negative is descending.
        /// </param>
        /// <returns>
        /// A list of all organizations that match the specified key and value within the repository.
        /// </returns>
        public async Task<OrganizationList> GetAsync(string key, string value, int index = 0, int count = 100, string order = "", int direction = 0)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"The specified {nameof(key)} parameter is invalid.");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"The specified {nameof(value)} parameter is invalid.");
            }

            OrganizationList organizations = new OrganizationList();

            // Parse the value
            DataParser parser = new DataParser();
            var result = parser.Parse(value);

            // Create the filter
            key = ((key == "id") || (key == "metadata.id")) ? "_id" : key;
            var filter = Builders<Organization>.Filter.Eq(key, result.Item2);

            if (!string.IsNullOrEmpty(order))
            {
                direction = (direction >= 0) ? 1 : -1;

                var list = await this.Organizations.Find(filter).Sort(new BsonDocument(order, direction)).Skip(index).Limit(count).ToListAsync();
                organizations = new OrganizationList(list);
            }
            else
            {
                var list = await this.Organizations.Find(filter).Skip(index).Limit(count).ToListAsync();
                organizations = new OrganizationList(list);
            }

            return organizations;
        }

        /// <summary>
        /// Gets the service organization with the specified ID asynchronously.
        /// </summary>
        /// <param name="id">
        /// An ID identifying the service organization to retrieve.
        /// </param>
        /// <returns>
        /// An object representing the service organization with the specified ID.
        /// </returns>
        public async Task<Organization> GetAsync(Guid id)
        {
            var filter = Builders<Organization>.Filter.Eq("_id", id);
            var organizations = await Organizations.FindAsync(filter);
            Organization organization = organizations.FirstOrDefault<Organization>();

            return organization;
        }

        /// <summary>
        /// Add a new service organization to the repository.
        /// </summary>
        /// <param name="organization">
        /// An object representing the service organization to add.
        /// </param>
        public Task AddAsync(Organization organization)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }

            _addList.Add(organization);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Add a range of organizations to the repository.
        /// </summary>
        /// <param name="organization">
        /// A list of object representing the organizations to add.
        /// </param>
        public Task AddAsync(OrganizationList organizations)
        {
            if (organizations == null)
            {
                throw new ArgumentNullException(nameof(organizations));
            }

            _addList.AddRange(organizations);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates an existing service organization within the repostory.
        /// </summary>
        /// <param name="organization">
        /// An object representing the service organization to update.
        /// </param>
        public Task UpdateAsync(Organization organization)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }

            _updateList.Add(organization);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates a range of organizations within the repository.
        /// </summary>
        /// <param name="organizations">
        /// A list of object representing the organizations to update.
        /// </param>
        public Task UpdateAsync(OrganizationList organizations)
        {
            if (organizations == null)
            {
                throw new ArgumentNullException(nameof(organizations));
            }

            _updateList.AddRange(organizations);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes the service organization associated with the specified ID from the repository asynchronously.
        /// </summary>
        /// <param name="id">
        /// An ID identifying the service organization to remove.
        /// </param>
        public Task RemoveAsync(Guid id)
        {
            _removeList.Add(id);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes a range of organizations from the repository.
        /// </summary>
        /// <param name="ids">
        /// An ID identifying the service organization to remove.
        /// </param>
        public Task RemoveAsync(List<Guid> ids)
        {
            _removeList.AddRange(ids);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Commit all changes in the repository.
        /// </summary>
        /// <returns>
        /// The number of changes included within the repository
        /// </returns>
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

        /// <summary>
        /// Create the specified organizations in the repository asynchronously.
        /// </summary>
        /// <param name="organizations">
        /// A list of organizations to create to the repository.
        /// </param>
        private async Task<int> CreateAsync(OrganizationList organizations)
        {
            if (organizations == null)
            {
                throw new ArgumentNullException(nameof(organizations));
            }

            int count = organizations.Count;

            // Insert the organizations
            await Organizations.InsertManyAsync(organizations);

            return count;
        }

        /// <summary>
        /// Create or update the specified organizations within the repository asynchronously.
        /// </summary>
        /// <param name="organizations">
        /// A list of organizations to create or update within the repository.
        /// </param>
        private async Task<int> CreateUpdateAsync(OrganizationList organizations)
        {
            if (organizations == null)
            {
                throw new ArgumentNullException(nameof(organizations));
            }

            // Build the request
            var requests = new List<WriteModel<Organization>>();
            foreach (var organization in organizations)
            {
                var filter = Builders<Organization>.Filter.Eq("_id", organization.Id);
                var model = new ReplaceOneModel<Organization>(filter, organization) { IsUpsert = true };
                requests.Add(model);
            }

            int count = 0;

            // Create/update the organizations in bulk
            var result = await Organizations.BulkWriteAsync(requests);
            if ((result != null) && (result.IsAcknowledged))
            {
                count = (int)result.InsertedCount + (int)result.ModifiedCount + result.Upserts.Count;
            }

            return count;
        }

        /// <summary>
        /// Delete the specified organizations from the repository asynchronously.
        /// </summary>
        /// <param name="ids">
        /// A list of IDs identifying the organizations to delete from the repository.
        /// </param>
        private async Task<int> DeleteAsync(List<Guid> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            int count = 0;

            // Delete the organizations
            var filter = Builders<Organization>.Filter.In(x => x.Id, ids);
            var result = await Organizations.DeleteManyAsync(filter);
            if ((result != null) && (result.IsAcknowledged))
            {
                count = (int)result.DeletedCount;
            }

            return count;
        }
    }
}
