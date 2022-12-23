using Cinema.Backend.Service.Models;
using Cinema.Backend.Service.Models.Core;
using Envista.Core.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Template.Service.Configuration;
using Template.Service.DAL;

namespace Cinema.Backend.Service.Controllers
{
    [Route("api/v1/cinema/[controller]")]
    public class RatingsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public RatingsController(ILogger<RatingsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        ///  GET ratings?index={index}&count={count}&order={order}&direction={direction}
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<Rating>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(void))]
        public async Task<ActionResult<IEnumerable<Rating>>> GetAll([FromQuery] int index = 0, [FromQuery] int count = 100, [FromQuery] string order = "", [FromQuery] int direction = 0)
        {
            var list = new List<Rating>();

            try
            {
                _logger.LogDebug($"{nameof(GetAll)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                long totalCount = 0;
                var ratings = new List<Rating>();

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Get the total count of ratings
                    totalCount = await unitOfWork.Ratings.GetCountAsync();

                    // Get the ratings
                    ratings = await unitOfWork.Ratings.GetAsync(index, count, order, direction);
                }

                list = new List<Rating>(ratings);
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
        /// GET: ratings/search/key/value?index={index}&count={count}&order={order}&direction={direction}
        /// </summary>
        [HttpGet("search/{key}/{value}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<Rating>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(void))]
        public async Task<ActionResult<IEnumerable<Rating>>> Search([FromRoute] string key, [FromRoute] string value, [FromQuery] int index = 0, [FromQuery] int count = 100, [FromQuery] string order = "", [FromQuery] int direction = 0)
        {
            var list = new List<Rating>();

            try
            {
                _logger.LogDebug($"{nameof(GetAll)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                long totalCount = 0;
                var ratings = new List<Rating>();

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Get the total count of ratings that match the key and value
                    totalCount = await unitOfWork.Ratings.GetCountAsync(key, value);

                    // Get the organizations that match the key and value
                    ratings = await unitOfWork.Ratings.GetAsync(key, value, index, count, order, direction);
                }

                list = new List<Rating>(ratings);
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
        /// GET: ratings/{id}
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Rating), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(void))]
        public async Task<ActionResult<Rating>> Get([FromRoute] Guid id)
        {
            Rating? rating = null;

            try
            {
                _logger.LogDebug($"{nameof(Get)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Get the rating
                    rating = await unitOfWork.Ratings.GetAsync(id);
                    if (rating == null)
                    {
                        throw new ObjectNotFoundException($"A rating with the specified ID could not be found (ID: {id.ToString("N")}).");
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Web API {nameof(Get)} threw an exception.");
                throw;
            }

            return rating;
        }

        /// <summary>
        /// POST: ratings
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{Roles.REGISTERED_USER}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesErrorResponseType(typeof(Rating))]
        public async Task<ActionResult> Create([FromBody] Rating rating)
        {
            ActionResult result = StatusCode((int)HttpStatusCode.InternalServerError, "The content could not be displayed because an internal server error has occured.");

            try
            {
                _logger.LogDebug($"{nameof(Create)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                Rating? newRating = null;

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Add the specified ticket
                    await unitOfWork.Ratings.AddAsync(rating);

                    // Commit the unit of work
                    await unitOfWork.CommitAsync();

                    // Get the newly created ticket
                    newRating = await unitOfWork.Ratings.GetAsync(rating.Id);
                }

                // Set a successful HTTP status code (201)
                result = StatusCode((int)HttpStatusCode.Created, newRating);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Web API {nameof(Create)} threw an exception.");
                throw;
            }

            return result;
        }

        /// <summary>
        /// PUT: ratings
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{Roles.REGISTERED_USER}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesErrorResponseType(typeof(Rating))]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] Rating rating)
        {
            ActionResult result = StatusCode((int)HttpStatusCode.InternalServerError, "The content could not be displayed because an internal server error has occured.");

            try
            {
                _logger.LogDebug($"{nameof(Create)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                Rating? updatedRating = null;

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Check that ticekt exists
                    var match = await unitOfWork.Ratings.GetAsync(id);
                    if ((match == null) || (match.Id != rating.Id))
                    {
                        throw new ObjectNotFoundException($"The rating with the specified ID could not be found (ID: {id:N}).");
                    }

                    // Update the specified ticket
                    await unitOfWork.Ratings.UpdateAsync(rating);

                    // Commit the unit of work
                    await unitOfWork.CommitAsync();

                    // Get the newly updated organization
                    updatedRating = await unitOfWork.Ratings.GetAsync(rating.Id);
                }

                // Set a successful HTTP status code (200)
                result = StatusCode((int)HttpStatusCode.OK, updatedRating);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Web API {nameof(Create)} threw an exception.");
                throw;
            }

            return result;
        }

        /// <summary>
        /// Delete: ratings/{id}
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{Roles.ADMINISTRATOR}")]
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
                    // Get the ticket
                    Rating rating = await unitOfWork.Ratings.GetAsync(id);
                    if (rating == null)
                    {
                        throw new ObjectNotFoundException($"Rating with the specified ID could not be found (ID: {id.ToString("N")}).");
                    }

                    // Delete the specified ticket
                    await unitOfWork.Tickets.RemoveAsync(id);

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
