using Cinema.Backend.Service.Models;
using Envista.Core.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Sockets;
using Template.Service.Configuration;
using Template.Service.DAL;

namespace Cinema.Backend.Service.Controllers
{
    [Route("api/v1/cinema/[controller]")]
    public class MovieProjectionsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public MovieProjectionsController(ILogger<MovieProjectionsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        ///  GET movieprojections?index={index}&count={count}&order={order}&direction={direction}
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MovieProjection>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(void))]
        public async Task<ActionResult<IEnumerable<MovieProjection>>> GetAll([FromQuery] int index = 0, [FromQuery] int count = 100, [FromQuery] string order = "", [FromQuery] int direction = 0)
        {
            var list = new List<MovieProjection>();

            try
            {
                _logger.LogDebug($"{nameof(GetAll)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                long totalCount = 0;
                var movieProjections = new List<MovieProjection>();

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Get the total count of tickets
                    totalCount = await unitOfWork.MovieProjections.GetCountAsync();

                    // Get the tickets
                    movieProjections = await unitOfWork.MovieProjections.GetAsync(index, count, order, direction);
                }

                list = new List<MovieProjection>(movieProjections);
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
        /// GET: movieprojections/search/key/value?index={index}&count={count}&order={order}&direction={direction}
        /// </summary>
        [HttpGet("search/{key}/{value}")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<MovieProjection>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(void))]
        public async Task<ActionResult<IEnumerable<MovieProjection>>> Search([FromRoute] string key, [FromRoute] string value, [FromQuery] int index = 0, [FromQuery] int count = 100, [FromQuery] string order = "", [FromQuery] int direction = 0)
        {
            var list = new List<MovieProjection>();

            try
            {
                _logger.LogDebug($"{nameof(GetAll)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                long totalCount = 0;
                var movieProjections = new List<MovieProjection>();

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Get the total count of organizations that match the key and value
                    totalCount = await unitOfWork.MovieProjections.GetCountAsync(key, value);

                    // Get the organizations that match the key and value
                    movieProjections = await unitOfWork.MovieProjections.GetAsync(key, value, index, count, order, direction);
                }

                list = new List<MovieProjection>(movieProjections);
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
        /// GET: movieprojections/{id}
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(MovieProjection), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(void))]
        public async Task<ActionResult<MovieProjection>> Get([FromRoute] Guid id)
        {
            MovieProjection? movieProjection = null;

            try
            {
                _logger.LogDebug($"{nameof(Get)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Get the organization
                    movieProjection = await unitOfWork.MovieProjections.GetAsync(id);
                    if (movieProjection == null)
                    {
                        throw new ObjectNotFoundException($"A movie projection with the specified ID could not be found (ID: {id.ToString("N")}).");
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Web API {nameof(Get)} threw an exception.");
                throw;
            }

            return movieProjection;
        }

        /// <summary>
        /// POST: movieprojections
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesErrorResponseType(typeof(MovieProjection))]
        public async Task<ActionResult> Create([FromBody] Ticket movieProjection)
        {
            ActionResult result = StatusCode((int)HttpStatusCode.InternalServerError, "The content could not be displayed because an internal server error has occured.");

            try
            {
                _logger.LogDebug($"{nameof(Create)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                MovieProjection? newMovieProjection = null;

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Add the specified movie projection
                    await unitOfWork.Tickets.AddAsync(movieProjection);

                    // Commit the unit of work
                    await unitOfWork.CommitAsync();

                    // Get the newly created movie projection
                    newMovieProjection = await unitOfWork.MovieProjections.GetAsync(movieProjection.Id);
                }

                // Set a successful HTTP status code (201)
                result = StatusCode((int)HttpStatusCode.Created, newMovieProjection);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Web API {nameof(Create)} threw an exception.");
                throw;
            }

            return result;
        }

        /// <summary>
        /// PUT: tickets
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesErrorResponseType(typeof(MovieProjection))]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] MovieProjection movieProjection)
        {
            ActionResult result = StatusCode((int)HttpStatusCode.InternalServerError, "The content could not be displayed because an internal server error has occured.");

            try
            {
                _logger.LogDebug($"{nameof(Create)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                MovieProjection? updatedMovieProjection = null;

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Check that movie projection exists
                    var match = await unitOfWork.MovieProjections.GetAsync(id);
                    if ((match == null) || (match.Id != movieProjection.Id))
                    {
                        throw new ObjectNotFoundException($"The movie projection with the specified ID could not be found (ID: {id:N}).");
                    }

                    // Update the specified ticket
                    await unitOfWork.MovieProjections.UpdateAsync(movieProjection);

                    // Commit the unit of work
                    await unitOfWork.CommitAsync();

                    // Get the newly updated organization
                    updatedMovieProjection = await unitOfWork.MovieProjections.GetAsync(movieProjection.Id);
                }

                // Set a successful HTTP status code (200)
                result = StatusCode((int)HttpStatusCode.OK, updatedMovieProjection);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Web API {nameof(Create)} threw an exception.");
                throw;
            }

            return result;
        }

        /// <summary>
        /// Delete: movieprojections/{id}
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
                    // Get the ticket
                    MovieProjection movieProjection = await unitOfWork.MovieProjections.GetAsync(id);
                    if (movieProjection == null)
                    {
                        throw new ObjectNotFoundException($"Movie projection with the specified ID could not be found (ID: {id.ToString("N")}).");
                    }

                    // Delete the specified movie projection
                    await unitOfWork.MovieProjections.RemoveAsync(id);

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
