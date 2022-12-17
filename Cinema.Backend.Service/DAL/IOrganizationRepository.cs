using Template.Service.Models.Organization;

namespace Template.Service.DAL
{
    /// <summary>
    /// Represents the base methods and properties for managing a organization repository.
    /// </summary>
    public interface IOrganizationRepository
    {
        /// <summary>
        /// Get's the number of organizations within the repository.
        /// </summary>
        /// <returns>
        /// A count of the number of organizations within the repository.
        /// </returns>
        Task<long> GetCountAsync();

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
        Task<long> GetCountAsync(string key, string value);

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
        Task<OrganizationList> GetAsync(int index = 0, int count = 100, string order = "", int direction = 0);

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
        Task<OrganizationList> GetAsync(string key, string value, int index = 0, int count = 100, string order = "", int direction = 0);

        /// <summary>
        /// Gets the organization with the specified ID asynchronously.
        /// </summary>
        /// <param name="id">
        /// An ID identifying the organization to retrieve.
        /// </param>
        /// <returns>
        /// An object representing the organization with the specified ID.
        /// </returns>
        Task<Organization> GetAsync(Guid id);

        /// <summary>
        /// Add a new organization to the repository.
        /// </summary>
        /// <param name="organization">
        /// An object representing the organization to add.
        /// </param>
        Task AddAsync(Organization organization);

        /// <summary>
        /// Add a range of organizations to the repository.
        /// </summary>
        /// <param name="organizations">
        /// A list of object representing the organizations to add.
        /// </param>
        Task AddAsync(OrganizationList organizations);

        /// <summary>
        /// Updates an existing organization within the repository.
        /// </summary>
        /// <param name="organization">
        /// An object representing the organization to update.
        /// </param>
        Task UpdateAsync(Organization organization);

        /// <summary>
        /// Updates a range of organizations within the repository.
        /// </summary>
        /// <param name="organizations">
        /// A list of object representing the organizations to update.
        /// </param>
        Task UpdateAsync(OrganizationList organizations);

        /// <summary>
        /// Removes the organization from the repository.
        /// </summary>
        /// <param name="id">
        /// An ID identifying the organization to remove.
        /// </param>
        Task RemoveAsync(Guid id);

        /// <summary>
        /// Removes a range of organizations from the repository asynchronously.
        /// </summary>
        /// <param name="ids">
        /// An ID identifying the organization to remove.
        /// </param>
        Task RemoveAsync(List<Guid> ids);
    }
}
