using Cinema.Backend.Service.Models;
using Cinema.Backend.Service.Models.Core;
using Cinema.Backend.Service.Models.DTOs;
using Envista.Core.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Template.Service.Configuration;
using Template.Service.DAL;

namespace Cinema.Backend.Service.Controllers
{
    [Route("api/v1/cinema/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public MoviesController(ILogger<MoviesController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        ///  GET movies?index={index}&count={count}&order={order}&direction={direction}
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<MovieWithRating>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(void))]
        public async Task<ActionResult<IEnumerable<MovieWithRating>>> GetAll([FromQuery] int index = 0, [FromQuery] int count = 100, [FromQuery] string order = "", [FromQuery] int direction = 0)
        {
            var list = new List<MovieWithRating>();

            try
            {
                _logger.LogDebug($"{nameof(GetAll)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                long totalCount = 0;
                var movies = new List<Movie>();
                var ratings = new List<Rating>();
                var projections = new List<MovieProjection>();

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Get the total count of tickets
                    totalCount = await unitOfWork.Movies.GetCountAsync();

                    // Get the movies
                    movies = await unitOfWork.Movies.GetAsync(index, count, order, direction);

                    ratings = await unitOfWork.Ratings.GetAsync(index, count, order, direction);

                    projections = await unitOfWork.MovieProjections.GetAsync(index, count, order, direction);
                }

                list = CalculateRatingForMovies(movies, ratings);
                AddMovieProjections(list, projections);
                
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
        /// GET: movies/search/key/value?index={index}&count={count}&order={order}&direction={direction}
        /// </summary>
        [HttpGet("search/{key}/{value}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<MovieWithRating>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(void))]
        public async Task<ActionResult<IEnumerable<MovieWithRating>>> Search([FromRoute] string key, [FromRoute] string value, [FromQuery] int index = 0, [FromQuery] int count = 100, [FromQuery] string order = "", [FromQuery] int direction = 0)
        {
            var list = new List<MovieWithRating>();

            try
            {
                _logger.LogDebug($"{nameof(GetAll)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                long totalCount = 0;
                var movies = new List<Movie>();
                var ratings = new List<Rating>();
                var projections = new List<MovieProjection>();

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Get the total count of movies that match the key and value
                    totalCount = await unitOfWork.Tickets.GetCountAsync(key, value);

                    // Get the movies that match the key and value
                    movies = await unitOfWork.Movies.GetAsync(key, value, index, count, order, direction);

                    ratings = await unitOfWork.Ratings.GetAsync(index, count, order, direction);

                    projections = await unitOfWork.MovieProjections.GetAsync(index, count, order, direction);
                }

                list = CalculateRatingForMovies(movies, ratings);
                AddMovieProjections(list, projections);
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
        /// GET: movies/{id}
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(MovieWithRating), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(void))]
        public async Task<ActionResult<MovieWithRating>> Get([FromRoute] Guid id)
        {
            Movie? movie = null;
            var movieWithRating = new MovieWithRating();
            var ratings = new List<Rating>();
            var projections = new List<MovieProjection>();

            try
            {
                _logger.LogDebug($"{nameof(Get)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Get the organization
                    movie = await unitOfWork.Movies.GetAsync(id);
                    if (movie == null)
                    {
                        throw new ObjectNotFoundException($"A movie with the specified ID could not be found (ID: {id.ToString("N")}).");
                    }

                    // calculate rating for movie
                    ratings = await unitOfWork.Ratings.GetAsync();
                    double rating = ratings
                        .Where(x => x.MovieId == movie.Id)
                        .Select(x => x.Value)
                        .Average();
                    movieWithRating.Id = movie.Id;
                    movieWithRating.Title = movie.Title;
                    movieWithRating.Description = movie.Description;
                    movieWithRating.ReleaseDate = movie.ReleaseDate;
                    movieWithRating.CoverUrl = movie.CoverUrl;
                    movieWithRating.Rating = rating;

                    // add movie projections
                    projections = await unitOfWork.MovieProjections.GetAsync();
                    movieWithRating.Projections = projections.Where(x => x.MovieId == movieWithRating.Id).ToList();
                }
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Web API {nameof(Get)} threw an exception.");
                throw;
            }

            return movieWithRating;
        }

        /// <summary>
        /// POST: movies
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{Roles.ADMINISTRATOR}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesErrorResponseType(typeof(Movie))]
        public async Task<ActionResult> Create([FromBody] Movie movie)
        {
            ActionResult result = StatusCode((int)HttpStatusCode.InternalServerError, "The content could not be displayed because an internal server error has occured.");

            try
            {
                _logger.LogDebug($"{nameof(Create)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                Movie? newMovie = null;

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Add the specified movie
                    await unitOfWork.Movies.AddAsync(movie);

                    // Commit the unit of work
                    await unitOfWork.CommitAsync();

                    // Get the newly created ticket
                    newMovie = await unitOfWork.Movies.GetAsync(movie.Id);
                }

                // Set a successful HTTP status code (201)
                result = StatusCode((int)HttpStatusCode.Created, newMovie);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Web API {nameof(Create)} threw an exception.");
                throw;
            }

            return result;
        }

        /// <summary>
        /// PUT: movies
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{Roles.ADMINISTRATOR}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesErrorResponseType(typeof(Movie))]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] Movie movie)
        {
            ActionResult result = StatusCode((int)HttpStatusCode.InternalServerError, "The content could not be displayed because an internal server error has occured.");

            try
            {
                _logger.LogDebug($"{nameof(Create)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                Movie? updatedMovie = null;

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Check that movie exists
                    var match = await unitOfWork.Movies.GetAsync(id);
                    if ((match == null) || (match.Id != movie.Id))
                    {
                        throw new ObjectNotFoundException($"The movie with the specified ID could not be found (ID: {id:N}).");
                    }

                    // Update the specified ticket
                    await unitOfWork.Movies.UpdateAsync(movie);

                    // Commit the unit of work
                    await unitOfWork.CommitAsync();

                    // Get the newly updated movie
                    updatedMovie = await unitOfWork.Movies.GetAsync(movie.Id);
                }

                // Set a successful HTTP status code (200)
                result = StatusCode((int)HttpStatusCode.OK, updatedMovie);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Web API {nameof(Create)} threw an exception.");
                throw;
            }

            return result;
        }

        /// <summary>
        /// Delete: movies/{id}
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
                    // Get the movie
                    Movie movie = await unitOfWork.Movies.GetAsync(id);
                    if (movie == null)
                    {
                        throw new ObjectNotFoundException($"Movie with the specified ID could not be found (ID: {id.ToString("N")}).");
                    }

                    // Delete the specified movie
                    await unitOfWork.Movies.RemoveAsync(id);

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

        private void AddMovieProjections(List<MovieWithRating> movies, List<MovieProjection> projections)
        {
            foreach (var movie in movies)
            {
                var projectionsForMovie = projections.Where(x => x.MovieId == movie.Id).ToList();
                movie.Projections = projectionsForMovie;
            }
        }

        private List<MovieWithRating> CalculateRatingForMovies(List<Movie> movies, List<Rating> ratings)
        {
            var moviesWithRating = new List<MovieWithRating>();
            foreach (var movie in movies)
            {
                double rating = ratings
                    .Where(x => x.MovieId == movie.Id)
                    .Select(x => x.Value)
                    .Average();
                moviesWithRating.Add(new MovieWithRating
                {
                    Id = movie.Id,
                    Rating = rating,
                    Title = movie.Title,
                    Description = movie.Description,
                    Genre = movie.Genre,
                    ReleaseDate = movie.ReleaseDate,
                    CoverUrl = movie.CoverUrl
            });
            }
            return moviesWithRating;
        }
    }
}
