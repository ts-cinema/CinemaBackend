using MongoDB.Driver;
using Template.Service.DAL;
using Envista.Core.Common.System;
using Cinema.Backend.Service.Models;

namespace Template.Service.Setup
{
    /// <summary>
    /// Provides helper methods for setting up a MongoDb database.
    /// </summary>
    public class MongoDbSetup
    {
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initialize a new instance of the MongoDbSetup class.
        /// </summary>
        /// <param name="logger">
        /// An object that represents the service's logger.
        /// </param>
        public MongoDbSetup(Serilog.ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Initialize the MongoDb server database as defined in the hosted service's configuration settings.
        /// </summary>
        public async Task InitializeAsync(string connectionString, string databaseName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException($"The specified {nameof(connectionString)} parameter is invalid.");
            }

            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentException($"The specified {nameof(databaseName)} parameter is invalid.");
            }

            _logger.Information($"Checking whether the domain's '{databaseName}' MongoDB database is initialized.");

            try
            {
                // Initialize the MongoDB client
                var client = new MongoClient(connectionString);

                // Check if the domain database exists
                bool exists = (await client.ListDatabasesAsync()).ToList().Any(db => db.GetValue("name").AsString.Equals(databaseName));

                if (!exists)
                {
                    _logger?.Information($"The domain's MongoDB database '{databaseName}' does not exist.");

                    // Create the database
                    IMongoDatabase database = client.GetDatabase(databaseName);
                    _logger.Information($"The domain's '{databaseName}' MongoDB database was successfully created.");

                    #region Create Collections
                    await CreateMoviesCollection(database, databaseName);

                    await CreateMovieProjectionsCollection(database, databaseName);

                    await CreateRatingsCollection(database, databaseName);

                    await CreateTicketsCollection(database, databaseName);

                    #endregion // Create Collections

                    #region Populate Collections

                    #endregion // Populate collections
                }
                else
                {
                    _logger.Information($"The domain's '{databaseName}' MongoDB database already exists.");
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception, $"Unable to initialize the domain's MongoDb '{databaseName}' database.");
                throw;
            }
        }

        private async Task CreateTicketsCollection(IMongoDatabase database, string databaseName)
        {
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }

            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentException($"The specified {nameof(databaseName)} parameter is invalid.");
            }

            // Create the organizations collection
            string collectionName = "tickets";
            IMongoCollection<Ticket> ticketCollection = database.GetCollection<Ticket>(collectionName);
            _logger.Information($"The '{collectionName}' collection within the '{databaseName}' database was successfully created.");

            var ticketIndexKeysDefinition = Builders<Ticket>.IndexKeys;
            var ticketIndexes = new List<CreateIndexModel<Ticket>>();

            // Create an index for the Id
            var ticketIdIndex = new CreateIndexModel<Ticket>(ticketIndexKeysDefinition.Ascending(x => x.Id));
            ticketIndexes.Add(ticketIdIndex);

            // Create the indexes
            await ticketCollection.Indexes.CreateManyAsync(ticketIndexes).ConfigureAwait(false);

            _logger.Information($"The indexes for the '{collectionName}' collection within the '{databaseName}' database were successfully created.");
        }

        private async Task CreateRatingsCollection(IMongoDatabase database, string databaseName)
        {
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }

            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentException($"The specified {nameof(databaseName)} parameter is invalid.");
            }

            // Create the organizations collection
            string collectionName = "ratings";
            IMongoCollection<Rating> ratingsCollection = database.GetCollection<Rating>(collectionName);
            _logger.Information($"The '{collectionName}' collection within the '{databaseName}' database was successfully created.");

            var ratingIndexKeysDefinition = Builders<Rating>.IndexKeys;
            var ratingIndexes = new List<CreateIndexModel<Rating>>();

            // Create an index for the Id
            var ratingIdIndex = new CreateIndexModel<Rating>(ratingIndexKeysDefinition.Ascending(x => x.Id));
            ratingIndexes.Add(ratingIdIndex);

            // Create the indexes
            await ratingsCollection.Indexes.CreateManyAsync(ratingIndexes).ConfigureAwait(false);

            _logger.Information($"The indexes for the '{collectionName}' collection within the '{databaseName}' database were successfully created.");
        }

        private async Task CreateMoviesCollection(IMongoDatabase database, string databaseName)
        {
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }

            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentException($"The specified {nameof(databaseName)} parameter is invalid.");
            }

            // Create the organizations collection
            string collectionName = "movies";
            IMongoCollection<Movie> moviesCollection = database.GetCollection<Movie>(collectionName);
            _logger.Information($"The '{collectionName}' collection within the '{databaseName}' database was successfully created.");

            var movieIndexKeysDefinition = Builders<Movie>.IndexKeys;
            var movieIndexes = new List<CreateIndexModel<Movie>>();

            // Create an index for the Title
            var movieNameIndex = new CreateIndexModel<Movie>(movieIndexKeysDefinition.Ascending(x => x.Title));
            movieIndexes.Add(movieNameIndex);

            // Create an index for the Genre
            var movieCodeIndex = new CreateIndexModel<Movie>(movieIndexKeysDefinition.Ascending(x => x.Genre));
            movieIndexes.Add(movieCodeIndex);

            // Create the indexes
            await moviesCollection.Indexes.CreateManyAsync(movieIndexes).ConfigureAwait(false);

            _logger.Information($"The indexes for the '{collectionName}' collection within the '{databaseName}' database were successfully created.");
        }

        private async Task CreateMovieProjectionsCollection(IMongoDatabase database, string databaseName)
        {
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }

            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentException($"The specified {nameof(databaseName)} parameter is invalid.");
            }

            // Create the organizations collection
            string collectionName = "movie-projections";
            IMongoCollection<MovieProjection> movieProjectionsCollection = database.GetCollection<MovieProjection>(collectionName);
            _logger.Information($"The '{collectionName}' collection within the '{databaseName}' database was successfully created.");

            var movieProjectionIndexKeysDefinition = Builders<MovieProjection>.IndexKeys;
            var movieProjectionIndexes = new List<CreateIndexModel<MovieProjection>>();

            // Create an index for the StartTime
            var movieProjectionStartTimeIndex = new CreateIndexModel<MovieProjection>(movieProjectionIndexKeysDefinition.Ascending(x => x.StartTime));
            movieProjectionIndexes.Add(movieProjectionStartTimeIndex);

            // Create an index for the Id
            var movieProjectionIdIndex = new CreateIndexModel<MovieProjection>(movieProjectionIndexKeysDefinition.Ascending(x => x.Id));
            movieProjectionIndexes.Add(movieProjectionIdIndex);

            // Create the indexes
            await movieProjectionsCollection.Indexes.CreateManyAsync(movieProjectionIndexes).ConfigureAwait(false);

            _logger.Information($"The indexes for the '{collectionName}' collection within the '{databaseName}' database were successfully created.");
        }
    }
}
