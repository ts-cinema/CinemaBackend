namespace Template.Service.Models.Health
{
    /// <summary>
    /// Defines the health result for a service component.
    /// </summary>
    public enum ServiceHealthResult
    {
        /// <summary>
        /// The service health check failed.
        /// </summary>
        Failed = 0,

        /// <summary>
        /// The service health check passed.
        /// </summary>
        Passed = 1
    }
}
