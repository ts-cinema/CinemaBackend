using MongoDB.Driver;

namespace Template.Service.DAL
{
    /// <summary>
    /// Represents the base methods and properties for an open connection to a mongo collection
    /// </summary>
    public class MongoContext : IDisposable
    {
        protected MongoClient _mongoClient = null;
        protected IMongoDatabase _mongoDatabase = null;

        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the DbMongoContext class.
        /// </summary>
        public MongoContext(string connectionString, string databaseName)
        {
            this.Initialize(connectionString, databaseName);
        }

        /// <summary>
        /// Finalize an instance of the DbMongoContext class.
        /// </summary>
        ~MongoContext()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Releases any unmanaged resources and optionally any managed resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            // Remove the object off the finalization queue and prevent finalization code for this object from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases any unmanaged resources and optionally any managed resources.
        /// </summary>
        /// <param name="disposing">
        /// true indicates whether method has been invoked by user code; otherwise false indicates that the method has been invoked by the run-time.
        /// </param>
        public void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Only dispose of managed resources if invoked by user code
                if (disposing)
                {
                    this._mongoDatabase = null;
                    this._mongoClient = null;
                }

                this._disposed = true;
            }
        }

        /// <summary>
        /// Initializes the internal mongo client
        /// </summary>
        private void Initialize(string connectionString, string databaseName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("The specified database connection string is invalid.");
            }

            if (string.IsNullOrEmpty(databaseName))
            {
                throw new InvalidOperationException("The specified database name is invalid.");
            }

            var mongoClient = new MongoClient(connectionString);
            this._mongoClient = mongoClient ?? throw new InvalidOperationException("Unable to initialize the mongo client.");

            var mongoDatabase = mongoClient.GetDatabase(databaseName);
            this._mongoDatabase = mongoDatabase ?? throw new InvalidOperationException("Unable to open the specified database.");
        }

        /// <summary>
        /// Gets the underlying mongoDb client.
        /// </summary>
        public MongoClient MongoClient
        {
            get
            {
                return this._mongoClient;
            }
        }

        /// <summary>
        /// Gets the underlying mongoDb database.
        /// </summary>
        public IMongoDatabase MongoDatabase
        {
            get
            {
                return this._mongoDatabase;
            }
        }
    }
}
