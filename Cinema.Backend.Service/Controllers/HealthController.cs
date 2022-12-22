using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Service.Configuration;
using Template.Service.Models.Health;
using Template.Service.DAL;

namespace Template.Service.Controllers
{
    /// <summary>
    /// Represents the endpoint for the health check.
    /// </summary>
    [Route("api/v1/template/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the HealthController class.
        /// </summary>
        /// <param name="logger">
        /// An object that represents the application's logger.
        /// </param>
        /// <param name="configuration">
        /// An object that represents the application's configuration.
        /// </param>
        public HealthController(ILogger<HealthController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// GET: health
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ServiceHealth), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [ProducesErrorResponseType(typeof(void))]
        public async Task<ActionResult> GetHealth()
        {
            ActionResult result;
            ServiceHealth serviceHealth = new ServiceHealth();

            try
            {
                _logger.LogDebug($"{nameof(GetHealth)} invoked.");

                // Get the service uptime
                serviceHealth.Uptime = Math.Round(Program.Uptime.TotalSeconds, 2);

                // Run the database access test
                var connectionString = _configuration.GetMongoConnectionString();
                serviceHealth.DatabaseAccessResult = await TestDatabaseAccessAsync(connectionString, "templatedb") ? ServiceHealthResult.Passed : ServiceHealthResult.Failed;
                

                HttpStatusCode code = HttpStatusCode.ServiceUnavailable;
                if (serviceHealth.DatabaseAccessResult == ServiceHealthResult.Passed)
                {
                    code = HttpStatusCode.OK;
                }

                // Set the status code
                result = StatusCode((int)code, serviceHealth);
            }
            catch
            {
                result = StatusCode((int)HttpStatusCode.ServiceUnavailable, serviceHealth);
            }

            return result;
        }

        #region Health checks

        /// <summary>
        /// Tests access to the database.
        /// </summary>
        private async Task<bool> TestDatabaseAccessAsync(string connectionString, string databaseName)
        {
            bool result = true;

            try
            {
                // Perform a simple idempotent action against the database
                using (var unitOfWork = new UnitOfWork(connectionString, databaseName))
                {
                    await unitOfWork.Movies.GetCountAsync();
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        #endregion // Health checks
    }
}
