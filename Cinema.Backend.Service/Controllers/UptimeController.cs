using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Template.Service.Controllers
{
    /// <summary>
    /// Represents the endpoint for the uptime check.
    /// </summary>
    [Route("api/v1/template/[controller]")]
    [ApiController]
    public class UptimeController : ControllerBase
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the UptimeController class.
        /// </summary>
        /// <param name="logger">
        /// An object that represents the application's logger.
        /// </param>
        public UptimeController(ILogger<UptimeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// GET: uptime
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(double), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(void))]
        public double Uptime()
        {
            _logger.LogDebug($"{nameof(Uptime)} invoked.");

            double uptime = Program.Uptime.TotalSeconds;
            return Math.Round(uptime, 2);
        }
    }
}
