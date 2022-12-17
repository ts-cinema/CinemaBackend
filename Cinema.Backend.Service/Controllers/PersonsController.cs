using System.Net;
using Envista.Core.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Service.Configuration;
using Template.Service.DAL;
using Template.Service.Models.Person;

namespace Template.Service.Controllers
{
    /// <summary>
    /// Represents the endpoint for managing persons.
    /// </summary>
    [Route("api/v1/template/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the PersonsController class.
        /// </summary>
        /// <param name="logger">
        /// An object that represents the application's logger.
        /// </param>
        /// <param name="configuration">
        /// An object that represents the application's configuration.
        /// </param>
        public PersonsController(ILogger<PersonsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        ///  GET persons?index={index}&count={count}&order={order}&direction={direction}
        /// </summary>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Person>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(void))]
        public async Task<ActionResult<IEnumerable<Person>>> GetAll([FromQuery] int index = 0, [FromQuery] int count = 100, [FromQuery] string order = "", [FromQuery] int direction = 0)
        {
            List<Person> list = new List<Person>();

            try
            {
                _logger.LogDebug($"{nameof(GetAll)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                long totalCount = 0;
                PersonList persons = new PersonList();

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Get the total count of persons
                    totalCount = await unitOfWork.Persons.GetCountAsync();

                    // Get the persons
                    persons = await unitOfWork.Persons.GetAsync(index, count, order, direction);
                }

                list = new List<Person>(persons);
                Request.HttpContext.Response.Headers.Add("x-total-count", totalCount.ToString());
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Web API {nameof(GetAll)} threw an exception.");
                throw;
            }

            return list;
        }

        /// <summary>
        /// GET: persons/search/key/value?index={index}&count={count}&order={order}&direction={direction}
        /// </summary>
        [HttpGet("search/{key}/{value}")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Person>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(void))]
        public async Task<ActionResult<IEnumerable<Person>>> Search([FromRoute] string key, [FromRoute] string value, [FromQuery] int index = 0, [FromQuery] int count = 100, [FromQuery] string order = "", [FromQuery] int direction = 0)
        {
            List<Person> list = new List<Person>();

            try
            {
                _logger.LogDebug($"{nameof(GetAll)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                long totalCount = 0;
                PersonList persons = new PersonList();

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Get the total count of persons that match the key and value
                    totalCount = await unitOfWork.Persons.GetCountAsync(key, value);

                    // Get the persons that match the key and value
                    persons = await unitOfWork.Persons.GetAsync(key, value, index, count, order, direction);
                }

                list = new List<Person>(persons);
                Request.HttpContext.Response.Headers.Add("x-total-count", totalCount.ToString());
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Web API {nameof(GetAll)} threw an exception.");
                throw;
            }

            return list;
        }

        /// <summary>
        /// GET: persons/{id}
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Person), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(void))]
        public async Task<ActionResult<Person>> Get([FromRoute] Guid id)
        {
            Person? person = null;

            try
            {
                _logger.LogDebug($"{nameof(Get)} invoked.");

                // Get the connection string
                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Get the person
                    person = await unitOfWork.Persons.GetAsync(id);
                    if (person == null)
                    {
                        throw new ObjectNotFoundException($"A person with the specified ID could not be found (ID: {id}).");
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Web API {nameof(Get)} threw an exception.");
                throw;
            }

            return person;
        }

        /// <summary>
        /// POST: persons
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesErrorResponseType(typeof(Person))]
        public async Task<ActionResult> Create([FromBody] Person person)
        {
            ActionResult result = StatusCode((int)HttpStatusCode.InternalServerError, "The content could not be displayed because an internal server error has occured.");

            try
            {
                _logger.LogDebug($"{nameof(Create)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                Person? newPerson = null;

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {

                    // Check if user with this ID exists
                    var existingUser = await unitOfWork.Persons.GetAsync(person.Id);

                    if (existingUser != null)
                    {
                        throw new ObjectAlreadyExistsException($"A person with the specified ID already exists in the database (ID: {person.Id}).");
                    }

                    // Add the specified person
                    await unitOfWork.Persons.AddAsync(person);

                    // Commit the unit of work
                    await unitOfWork.CommitAsync();

                    // Get the newly created person
                    newPerson = await unitOfWork.Persons.GetAsync(person.Id);
                }

                // Set a successful HTTP status code (201)
                result = StatusCode((int)HttpStatusCode.Created, newPerson);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Web API {nameof(Create)} threw an exception.");
                throw;
            }

            return result;
        }

        /// <summary>
        /// PUT: persons
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesErrorResponseType(typeof(Person))]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] Person person)
        {
            ActionResult result = StatusCode((int)HttpStatusCode.InternalServerError, "The content could not be displayed because an internal server error has occured.");

            try
            {
                _logger.LogDebug($"{nameof(Create)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                Person? updatedPerson = null;

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Check that person exists
                    var match = await unitOfWork.Persons.GetAsync(id);
                    if ((match == null) || (match.Id != person.Id))
                    {
                        throw new ObjectNotFoundException($"The person with the specified ID could not be found (ID: {id}).");
                    }

                    // Update the specified person
                    await unitOfWork.Persons.UpdateAsync(person);

                    // Commit the unit of work
                    await unitOfWork.CommitAsync();

                    // Get the newly updated person
                    updatedPerson = await unitOfWork.Persons.GetAsync(id);
                }

                // Set a successful HTTP status code (200)
                result = StatusCode((int)HttpStatusCode.OK, updatedPerson);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Web API {nameof(Create)} threw an exception.");
                throw;
            }

            return result;
        }

        /// <summary>
        /// Delete: persons/{id}
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(void))]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            ActionResult result = StatusCode((int)HttpStatusCode.InternalServerError, "The content could not be displayed because an internal server error has occured.");

            try
            {
                _logger.LogDebug($"{nameof(Delete)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Get the person
                    Person person = await unitOfWork.Persons.GetAsync(id);
                    if (person == null)
                    {
                        throw new ObjectNotFoundException($"A person with the specified ID could not be found (ID: {id}).");
                    }

                    // Delete the specified persons
                    await unitOfWork.Persons.RemoveAsync(id);

                    // Commit the unit of work
                    await unitOfWork.CommitAsync();
                }

                // Set a successful HTTP status code (204)
                result = NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Web API {nameof(Delete)} threw an exception.");
                throw;
            }

            return result;
        }
    }
}
