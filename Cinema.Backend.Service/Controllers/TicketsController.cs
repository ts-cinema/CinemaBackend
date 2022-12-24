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
    public class TicketsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public TicketsController(ILogger<TicketsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        ///  GET tickets?index={index}&count={count}&order={order}&direction={direction}
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{Roles.ADMINISTRATOR}")]
        [ProducesResponseType(typeof(IEnumerable<Ticket>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(void))]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetAll([FromQuery] int index = 0, [FromQuery] int count = 100, [FromQuery] string order = "", [FromQuery] int direction = 0)
        {
            var list = new List<Ticket>();

            try
            {
                _logger.LogDebug($"{nameof(GetAll)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                long totalCount = 0;
                var tickets = new List<Ticket>();

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Get the total count of tickets
                    totalCount = await unitOfWork.Tickets.GetCountAsync();

                    // Get the tickets
                    tickets = await unitOfWork.Tickets.GetAsync(index, count, order, direction);
                }

                list = new List<Ticket>(tickets);
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
        /// GET: tickets/search/key/value?index={index}&count={count}&order={order}&direction={direction}
        /// </summary>
        [HttpGet("search/{key}/{value}")]
        [Authorize(Roles = $"{Roles.ADMINISTRATOR}")]
        [ProducesResponseType(typeof(IEnumerable<Ticket>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(void))]
        public async Task<ActionResult<IEnumerable<Ticket>>> Search([FromRoute] string key, [FromRoute] string value, [FromQuery] int index = 0, [FromQuery] int count = 100, [FromQuery] string order = "", [FromQuery] int direction = 0)
        {
            var list = new List<Ticket>();

            try
            {
                _logger.LogDebug($"{nameof(GetAll)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                long totalCount = 0;
                var tickets = new List<Ticket>();

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Get the total count of organizations that match the key and value
                    totalCount = await unitOfWork.Tickets.GetCountAsync(key, value);

                    // Get the organizations that match the key and value
                    tickets = await unitOfWork.Tickets.GetAsync(key, value, index, count, order, direction);
                }

                list = new List<Ticket>(tickets);
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
        /// GET: tickets/{id}
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{Roles.ADMINISTRATOR}")]
        [ProducesResponseType(typeof(Ticket), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(void))]
        public async Task<ActionResult<Ticket>> Get([FromRoute] Guid id)
        {
            Ticket? ticket = null;

            try
            {
                _logger.LogDebug($"{nameof(Get)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Get the organization
                    ticket = await unitOfWork.Tickets.GetAsync(id);
                    if (ticket == null)
                    {
                        throw new ObjectNotFoundException($"A ticket with the specified ID could not be found (ID: {id.ToString("N")}).");
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Web API {nameof(Get)} threw an exception.");
                throw;
            }

            return ticket;
        }

        /// <summary>
        /// GET: tickets/user/{id}
        /// </summary>
        [HttpGet("user/{id}")]
        [Authorize(Roles = $"{Roles.REGISTERED_USER}")]
        [ProducesResponseType(typeof(List<Ticket>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(void))]
        public async Task<ActionResult<List<Ticket>>> GetTicketsForUser([FromRoute] Guid id)
        {
            var tickets = new List<Ticket>();

            try
            {
                _logger.LogDebug($"{nameof(Get)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Get the organization
                    tickets = await unitOfWork.Tickets.GetAsync("user_id", id.ToString());
                    if (tickets == null)
                    {
                        throw new ObjectNotFoundException($"Tickets with the specified user ID could not be found (ID: {id.ToString("N")}).");
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Web API {nameof(Get)} threw an exception.");
                throw;
            }

            return tickets;
        }

        /// <summary>
        /// POST: tickets
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{Roles.REGISTERED_USER}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesErrorResponseType(typeof(IEnumerable<TicketReservation>))]
        public async Task<ActionResult> Create([FromBody] TicketReservation ticketReservation)
        {
            ActionResult result = StatusCode((int)HttpStatusCode.InternalServerError, "The content could not be displayed because an internal server error has occured.");

            try
            {
                _logger.LogDebug($"{nameof(Create)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                Ticket? newTicket = null;
                var createdTickets = new List<Ticket>();

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // check for seats availability
                    var movieProjection = await unitOfWork.MovieProjections.GetAsync(ticketReservation.MovieProjectionId);

                    if (movieProjection.AvailableSeats - ticketReservation.Quantity < 0)
                    {
                        throw new ArgumentOutOfRangeException("Amount of requested tickets is not available");
                    }

                    movieProjection.AvailableSeats -= ticketReservation.Quantity;
                    await unitOfWork.MovieProjections.UpdateAsync(movieProjection);

                    var createdIds = new List<Guid>();

                    for (int i = 0; i < ticketReservation.Quantity; i++)
                    {
                        // initialize ticket object
                        newTicket = new Ticket
                        {
                            Name = ticketReservation.Name,
                            Price = ticketReservation.Price,
                            MovieProjectionId = ticketReservation.MovieProjectionId,
                            UserId = ticketReservation.UserId
                        };

                        // Add the specified ticket
                        await unitOfWork.Tickets.AddAsync(newTicket);

                        // Commit the unit of work
                        await unitOfWork.CommitAsync();

                        createdIds.Add(newTicket.Id);
                    }

                    foreach (var id in createdIds)
                    {
                        createdTickets.Add(
                            await unitOfWork.Tickets.GetAsync(id)
                        );
                    }
                }

                // Set a successful HTTP status code (201)
                result = StatusCode((int)HttpStatusCode.Created, createdTickets);
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
        [Authorize(Roles = $"{Roles.ADMINISTRATOR}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesErrorResponseType(typeof(Ticket))]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] Ticket ticket)
        {
            ActionResult result = StatusCode((int)HttpStatusCode.InternalServerError, "The content could not be displayed because an internal server error has occured.");

            try
            {
                _logger.LogDebug($"{nameof(Create)} invoked.");

                // Get the connection string
                var connectionString = _configuration.GetMongoConnectionString();
                var databaseName = _configuration.GetMongoDatabaseName();

                Ticket? updatedTicket = null;

                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    // Check that ticekt exists
                    var match = await unitOfWork.Tickets.GetAsync(id);
                    if ((match == null) || (match.Id != ticket.Id))
                    {
                        throw new ObjectNotFoundException($"The ticket with the specified ID could not be found (ID: {id:N}).");
                    }

                    // Update the specified ticket
                    await unitOfWork.Tickets.UpdateAsync(ticket);

                    // Commit the unit of work
                    await unitOfWork.CommitAsync();

                    // Get the newly updated organization
                    updatedTicket = await unitOfWork.Tickets.GetAsync(ticket.Id);
                }

                // Set a successful HTTP status code (200)
                result = StatusCode((int)HttpStatusCode.OK, updatedTicket);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Web API {nameof(Create)} threw an exception.");
                throw;
            }

            return result;
        }

        /// <summary>
        /// Delete: tickets/{id}
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
                    Ticket ticket = await unitOfWork.Tickets.GetAsync(id);
                    if (ticket == null)
                    {
                        throw new ObjectNotFoundException($"Ticket with the specified ID could not be found (ID: {id.ToString("N")}).");
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
