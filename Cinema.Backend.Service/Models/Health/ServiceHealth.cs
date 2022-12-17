using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Template.Service.Models.Health
{
    /// <summary>
    /// Represents the health of a service.
    /// </summary>
    public class ServiceHealth
    {
        /// <summary>
        /// Gets or sets the uptime of the service.
        /// </summary>
        [JsonProperty(PropertyName = "uptime")]
        public double Uptime { get; set; } = 0.0;

        /// <summary>
        /// Gets or sets the result of the database access test.
        /// </summary>
        [JsonProperty(PropertyName = "databaseAccessResult"), JsonConverter(typeof(StringEnumConverter))]
        public ServiceHealthResult DatabaseAccessResult { get; set; } = ServiceHealthResult.Failed;
    }
}
