using Template.Service.Models.Person;

namespace Template.Service.DAL
{
    /// <summary>
    /// Represents the base methods and properties for managing a person repository.
    /// </summary>
    public interface IPersonRepository
    {
        /// <summary>
        /// Get's the number of persons within the repository.
        /// </summary>
        /// <returns>
        /// A count of the number of persons within the repository.
        /// </returns>
        Task<long> GetCountAsync();

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
        Task<long> GetCountAsync(string key, string value);

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
        Task<PersonList> GetAsync(int index = 0, int count = 100, string order = "", int direction = 0);

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
        Task<PersonList> GetAsync(string key, string value, int index = 0, int count = 100, string order = "", int direction = 0);

        /// <summary>
        /// Gets the person with the specified ID asynchronously.
        /// </summary>
        /// <param name="id">
        /// An ID identifying the person to retrieve.
        /// </param>
        /// <returns>
        /// An object representing the person with the specified ID.
        /// </returns>
        Task<Person> GetAsync(Guid id);

        /// <summary>
        /// Add a new person to the repository.
        /// </summary>
        /// <param name="person">
        /// An object representing the person to add.
        /// </param>
        Task AddAsync(Person person);

        /// <summary>
        /// Add a range of persons to the repository.
        /// </summary>
        /// <param name="persons">
        /// A list of object representing the persons to add.
        /// </param>
        Task AddAsync(PersonList persons);

        /// <summary>
        /// Updates an existing person within the repository.
        /// </summary>
        /// <param name="person">
        /// An object representing the person to update.
        /// </param>
        Task UpdateAsync(Person person);

        /// <summary>
        /// Updates a range of persons within the repository.
        /// </summary>
        /// <param name="persons">
        /// A list of object representing the persons to update.
        /// </param>
        Task UpdateAsync(PersonList persons);

        /// <summary>
        /// Removes the person from the repository.
        /// </summary>
        /// <param name="id">
        /// An ID identifying the person to remove.
        /// </param>
        Task RemoveAsync(Guid id);

        /// <summary>
        /// Removes a range of persons from the repository asynchronously.
        /// </summary>
        /// <param name="ids">
        /// An ID identifying the person to remove.
        /// </param>
        Task RemoveAsync(List<Guid> ids);
    }
}
