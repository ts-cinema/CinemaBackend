using Envista.Core.Common.System;
using MongoDB.Bson;
using MongoDB.Driver;
using Template.Service.Models.Person;

namespace Template.Service.DAL
{
    /// <summary>
    ///  Represents the base methods and properties for managing a person repository.
    /// </summary>
    public class PersonRepository : IPersonRepository
    {
        private readonly MongoContext _context;
        private readonly string _collectionName = "persons";

        private readonly PersonList _addList = new PersonList();
        private readonly PersonList _updateList = new PersonList();
        private readonly List<Guid> _removeList = new List<Guid>();

        /// <summary>
        /// Initialize a new instance of the Persons class.
        /// </summary>
        public PersonRepository(MongoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the persons collection.
        /// </summary>
        private IMongoCollection<Person> Persons
        {
            get
            {
                return _context.MongoDatabase.GetCollection<Person>(this._collectionName);
            }
        }

        /// <summary>
        /// Get's the number of persons within the repository.
        /// </summary>
        /// <returns>
        /// A count of the number of persons within the repocitory.
        /// </returns>
        public async Task<long> GetCountAsync()
        {
            return await Persons.CountDocumentsAsync(FilterDefinition<Person>.Empty);
        }

        /// <summary>
        /// Get's the number of persons with the specified key and value within the repository.
        /// </summary>
        /// <param name="key">
        /// A string containing the key to examine.
        /// </param>
        /// <param name="value">
        /// A string containing the value to examine.
        /// </param>
        /// <returns>
        /// A count of the number of persons that match the specified key and value within the repository.
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
            var filter = Builders<Person>.Filter.Eq(key, result.Item2);

            // Get the person count
            long count = await this.Persons.CountDocumentsAsync(filter);

            return count;
        }

        /// <summary>
        /// Gets the list of persons within the repository.
        /// </summary>
        /// <param name="index">
        /// The starting index of the persons to include in the list.
        /// </param>
        /// <param name="count">
        /// The maximum number of persons to return.
        /// </param>
        /// <param name="order">
        /// A string containing the name of the attribute to sort on.
        /// </param>
        /// <param name="direction">
        /// An integer specifying the direction of the sort. Positive or zero is ascending; negative is descending.
        /// </param>
        /// <returns>
        /// A list of all persons.
        /// </returns>
        public async Task<PersonList> GetAsync(int index = 0, int count = 100, string order = "", int direction = 0)
        {
            PersonList persons = new PersonList();

            if (!string.IsNullOrEmpty(order))
            {
                direction = (direction >= 0) ? 1 : -1;

                // Gets all persons
                var list = await Persons.Find(FilterDefinition<Person>.Empty).Sort(new BsonDocument(order, direction)).Skip(index).Limit(count).ToListAsync();
                persons = new PersonList(list);
            }
            else
            {
                // Gets all persons
                var list = await Persons.Find(FilterDefinition<Person>.Empty).Skip(index).Limit(count).ToListAsync();
                persons = new PersonList(list);
            }

            return persons;
        }

        /// <summary>
        /// Gets the list of persons that match the specified key and value within the repository.
        /// </summary>
        /// <param name="key">
        /// A string containing the key to examine.
        /// </param>
        /// <param name="value">
        /// A string containing the value to examine.
        /// </param>
        /// <param name="index">
        /// The starting index of the persons to include in the list.
        /// </param>
        /// <param name="count">
        /// The maximum number of persons to return.
        /// </param>
        /// <param name="order">
        /// A string containing the name of the attribute to sort on.
        /// </param>
        /// <param name="direction">
        /// An integer specifying the direction of the sort. Positive or zero is ascending; negative is descending.
        /// </param>
        /// <returns>
        /// A list of all persons that match the specified key and value within the repository.
        /// </returns>
        public async Task<PersonList> GetAsync(string key, string value, int index = 0, int count = 100, string order = "", int direction = 0)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"The specified {nameof(key)} parameter is invalid.");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"The specified {nameof(value)} parameter is invalid.");
            }

            PersonList persons = new PersonList();

            // Parse the value
            DataParser parser = new DataParser();
            var result = parser.Parse(value);

            // Create the filter
            key = ((key == "id") || (key == "metadata.id")) ? "_id" : key;
            var filter = Builders<Person>.Filter.Eq(key, result.Item2);

            if (!string.IsNullOrEmpty(order))
            {
                direction = (direction >= 0) ? 1 : -1;

                var list = await this.Persons.Find(filter).Sort(new BsonDocument(order, direction)).Skip(index).Limit(count).ToListAsync();
                persons = new PersonList(list);
            }
            else
            {
                var list = await this.Persons.Find(filter).Skip(index).Limit(count).ToListAsync();
                persons = new PersonList(list);
            }

            return persons;
        }

        /// <summary>
        /// Gets the service person with the specified ID asynchronously.
        /// </summary>
        /// <param name="id">
        /// An ID identifying the service person to retrieve.
        /// </param>
        /// <returns>
        /// An object representing the service person with the specified ID.
        /// </returns>
        public async Task<Person> GetAsync(Guid id)
        {
            var filter = Builders<Person>.Filter.Eq("_id", id);
            var persons = await Persons.FindAsync(filter);
            Person person = persons.FirstOrDefault<Person>();

            return person;
        }

        /// <summary>
        /// Add a new service person to the repository.
        /// </summary>
        /// <param name="person">
        /// An object representing the service person to add.
        /// </param>
        public Task AddAsync(Person person)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            _addList.Add(person);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Add a range of persons to the repository.
        /// </summary>
        /// <param name="person">
        /// A list of object representing the persons to add.
        /// </param>
        public Task AddAsync(PersonList persons)
        {
            if (persons == null)
            {
                throw new ArgumentNullException(nameof(persons));
            }

            _addList.AddRange(persons);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates an existing service person within the repostory.
        /// </summary>
        /// <param name="person">
        /// An object representing the service person to update.
        /// </param>
        public Task UpdateAsync(Person person)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            _updateList.Add(person);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates a range of persons within the repository.
        /// </summary>
        /// <param name="persons">
        /// A list of object representing the persons to update.
        /// </param>
        public Task UpdateAsync(PersonList persons)
        {
            if (persons == null)
            {
                throw new ArgumentNullException(nameof(persons));
            }

            _updateList.AddRange(persons);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes the service person associated with the specified ID from the repository asynchronously.
        /// </summary>
        /// <param name="id">
        /// An ID identifying the service person to remove.
        /// </param>
        public Task RemoveAsync(Guid id)
        {
            _removeList.Add(id);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes a range of persons from the repository.
        /// </summary>
        /// <param name="ids">
        /// An ID identifying the service person to remove.
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
                // Create the specified persons in the repository
                count += await CreateAsync(_addList);
                _addList.Clear();
            }

            if (_updateList.Count > 0)
            {
                // Create or update the specified persons within the repository
                count += await CreateUpdateAsync(_updateList);
                _updateList.Clear();
            }

            if (_removeList.Count > 0)
            {
                // Delete the specified persons from the repository
                count += await DeleteAsync(_removeList);
                _removeList.Clear();
            }

            return count;
        }

        /// <summary>
        /// Create the specified persons in the repository asynchronously.
        /// </summary>
        /// <param name="persons">
        /// A list of persons to create to the repository.
        /// </param>
        private async Task<int> CreateAsync(PersonList persons)
        {
            if (persons == null)
            {
                throw new ArgumentNullException(nameof(persons));
            }

            int count = persons.Count;

            // Insert the persons
            await Persons.InsertManyAsync(persons);

            return count;
        }

        /// <summary>
        /// Create or update the specified persons within the repository asynchronously.
        /// </summary>
        /// <param name="persons">
        /// A list of persons to create or update within the repository.
        /// </param>
        private async Task<int> CreateUpdateAsync(PersonList persons)
        {
            if (persons == null)
            {
                throw new ArgumentNullException(nameof(persons));
            }

            // Build the request
            var requests = new List<WriteModel<Person>>();
            foreach (var person in persons)
            {
                var filter = Builders<Person>.Filter.Eq("_id", person.Id);
                var model = new ReplaceOneModel<Person>(filter, person) { IsUpsert = true };
                requests.Add(model);
            }

            int count = 0;

            // Create/update the persons in bulk
            var result = await Persons.BulkWriteAsync(requests);
            if ((result != null) && (result.IsAcknowledged))
            {
                count = (int)result.InsertedCount + (int)result.ModifiedCount + result.Upserts.Count;
            }

            return count;
        }

        /// <summary>
        /// Delete the specified persons from the repository asynchronously.
        /// </summary>
        /// <param name="ids">
        /// A list of IDs identifying the persons to delete from the repository.
        /// </param>
        private async Task<int> DeleteAsync(List<Guid> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            int count = 0;

            // Delete the persons
            var filter = Builders<Person>.Filter.In(x => x.Id, ids);
            var result = await Persons.DeleteManyAsync(filter);
            if ((result != null) && (result.IsAcknowledged))
            {
                count = (int)result.DeletedCount;
            }

            return count;
        }
    }
}
