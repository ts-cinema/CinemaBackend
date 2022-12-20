namespace Template.Service.Configuration
{
    /// <summary>
    /// Represents extension methods for the IConfiguration interface.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Gets the service's name.
        /// </summary>
        /// <param name="configuration">
        /// Represents a set of key:value configuration properties.
        /// </param>
        /// <returns>
        /// A string containing the service's name.
        /// </returns>
        public static string GetServiceName(this IConfiguration configuration)
        {
            return configuration["ServiceName"] ?? Environment.CurrentDirectory;
        }

        /// <summary>
        /// Gets the application's base folder.
        /// </summary>
        /// <param name="configuration">
        /// Represents a set of key:value configuration properties.
        /// </param>
        /// <returns>
        /// A string containing the application's base folder.
        /// </returns>
        public static string GetBaseFolder(this IConfiguration configuration)
        {
            return configuration["BaseFolder"] ?? Environment.CurrentDirectory;
        }

        /// <summary>
        /// Gets the application's mongo database settings.
        /// </summary>
        /// <param name="configuration">
        /// Represents a set of key:value configuration properties.
        /// </param>
        /// <returns>
        /// A string containing the application's mongo database settings.
        /// </returns>
        public static string GetMongoConnectionString(this IConfiguration configuration)
        {
            return configuration.GetConnectionString("MongoDb");
        }

        /// <summary>
        /// Gets the application's database name.
        /// </summary>
        /// <param name="configuration">
        /// Represents a set of key:value configuration properties.
        /// </param>
        /// <returns>
        /// A string containing the application's database name.
        /// </returns>
        public static string GetMongoDatabaseName(this IConfiguration configuration)
        {
            return configuration["DatabaseName"] ?? "templatedb";
        }

        /// <summary>
        /// Gets whether or not the swagger endpoint should be published.
        /// </summary>
        /// <param name="configuration">
        /// Represents a set of key:value configuration properties.
        /// </param>
        /// <returns>
        /// A bool indicating whether or not the swagger endpoint should be published.
        /// </returns>
        public static bool GetSwaggerPublishEndpoint(this IConfiguration configuration)
        {
            bool publish = false;

            string value = configuration["Swagger:PublishEndpoint"];
            if (!string.IsNullOrEmpty(value))
            {
                publish = Convert.ToBoolean(value);
            }

            return publish;
        }

        /// <summary>
        /// Gets the application's queue platform.
        /// </summary>
        /// <param name="configuration">
        /// Represents a set of key:value configuration properties.
        /// </param>
        /// <returns>
        /// A string containing the application's queue platform.
        /// </returns>
        public static string GetQueuePlatform(this IConfiguration configuration)
        {
            return configuration["Queue:Platform"] ?? string.Empty;
        }

        /// <summary>
        /// Gets the application's Azure queue settings.
        /// </summary>
        /// <param name="configuration">
        /// Represents a set of key:value configuration properties.
        /// </param>
        /// <returns>
        /// A string containing the application's Azure queue settings.
        /// </returns>
        public static string GetAzureQueueSettings(this IConfiguration configuration)
        {
            return configuration["Queue:AzureQueue:Settings"] ?? string.Empty;
        }

        /// <summary>
        /// Gets the message dispatcher service's activity interval.
        /// </summary>
        /// <param name="configuration">
        ///  Represents a set of key:value configuration properties.
        /// </param>
        /// <returns>
        /// The message dispatcher service's activity interval in milliseconds.
        /// </returns>
        public static int GetMessageDispatcherActivityInterval(this IConfiguration configuration)
        {
            int activityInterval = 1000;

            string value = configuration["Services:MessageDispatcher:ActivityInterval"];
            if (!string.IsNullOrEmpty(value))
            {
                if (!Int32.TryParse(value, out activityInterval))
                {
                    activityInterval = 1000;
                }
            }

            // Check minimum and maximum values
            activityInterval = (activityInterval < 100) ? 100 : activityInterval;
            activityInterval = (activityInterval > 5000) ? 5000 : activityInterval;

            return activityInterval;
        }

        /// <summary>
        /// Gets the service's token signing key.
        /// </summary>
        /// <param name="configuration">
        /// Represents a set of key:value configuration properties.
        /// </param>
        /// <returns>
        /// A string containing the service's password.
        /// </returns>
        public static string GetTokenSigningKey(this IConfiguration configuration)
        {
            return configuration["Security:TokenSigningKey"];
        }

        public static string GetAdminEmail(this IConfiguration configuration)
        {
            return configuration["Security:AdminCredentials:Email"];
        }

        public static string GetAdminUserName(this IConfiguration configuration)
        {
            return configuration["Security:AdminCredentials:UserName"];
        }

        public static string GetAdminPassword(this IConfiguration configuration)
        {
            return configuration["Security:AdminCredentials:Password"];
        }
    }
}
