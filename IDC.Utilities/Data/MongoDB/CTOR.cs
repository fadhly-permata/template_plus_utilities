using IDC.Utilities.Models.Data;
using MongoDB.Driver;

namespace IDC.Utilities.Data;

public sealed partial class MongoHelper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MongoHelper"/> class using a connection string.
    /// </summary>
    /// <param name="connectionString">The MongoDB connection string in standard URI format.</param>
    /// <param name="database">The name of the database to connect to.</param>
    /// <param name="messages">Optional custom messages for logging and exceptions.</param>
    /// <param name="logging">Optional logging provider for diagnostic information.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="connectionString"/> or <paramref name="database"/> is null, empty, or consists only of white-space characters.
    /// </exception>
    /// <exception cref="MongoConfigurationException">Invalid connection string format.</exception>
    /// <exception cref="MongoClientException">Failed to initialize MongoDB client.</exception>
    /// <remarks>
    /// Initializes MongoDB client with the specified connection string and database name.
    ///
    /// > [!IMPORTANT]
    /// > Connection string must follow MongoDB URI format specification.
    ///
    /// > [!NOTE]
    /// > The connection is not established until <see cref="Connect"/> is called.
    ///
    /// Supported connection string formats:
    /// - Standard URI: mongodb://[username:password@]host[:port][/database][?options]
    /// - DNS SRV: mongodb+srv://[username:password@]host[/database][?options]
    ///
    /// Example:
    /// <code>
    /// // Basic connection
    /// var db = new MongoHelper(
    ///     connectionString: "mongodb://localhost:27017",
    ///     database: "mydb"
    /// );
    ///
    /// // With authentication
    /// var db = new MongoHelper(
    ///     connectionString: "mongodb://user:pass@localhost:27017/?authSource=admin",
    ///     database: "mydb",
    ///     logging: new SystemLogging()
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/connection-string/">MongoDB Connection String URI Format</seealso>
    public MongoHelper(
        string connectionString,
        string database,
        Messages? messages = null,
        SystemLogging? logging = null
    )
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
            ArgumentException.ThrowIfNullOrWhiteSpace(database);

            _messages = messages ?? new();
            _logging = logging;
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(database);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoHelper"/> class using a common connection string format.
    /// </summary>
    /// <param name="connectionString">The common connection string configuration.</param>
    /// <param name="messages">Optional custom messages for logging and exceptions.</param>
    /// <param name="logging">Optional logging provider for diagnostic information.</param>
    /// <exception cref="ArgumentNullException"><paramref name="connectionString"/> is null.</exception>
    /// <exception cref="MongoConfigurationException">Invalid connection string configuration.</exception>
    /// <exception cref="MongoClientException">Failed to initialize MongoDB client.</exception>
    /// <remarks>
    /// Creates a MongoDB client using the standardized <see cref="CommonConnectionString"/> format.
    ///
    /// > [!TIP]
    /// > Use this constructor when working with multiple database types in your application.
    ///
    /// > [!NOTE]
    /// > The connection is not established until <see cref="Connect"/> is called.
    ///
    /// Example:
    /// <code>
    /// var connStr = new CommonConnectionString
    /// {
    ///     Server = "localhost",
    ///     Port = 27017,
    ///     Database = "mydb",
    ///     Username = "admin",
    ///     Password = "secret",
    ///     ConnectionTimeout = 30
    /// };
    ///
    /// var db = new MongoHelper(
    ///     connectionString: connStr,
    ///     logging: new SystemLogging()
    /// );
    /// </code>
    /// </remarks>
    /// <seealso cref="CommonConnectionString"/>
    /// <seealso cref="CommonConnectionString.ToMongoDB"/>
    public MongoHelper(
        CommonConnectionString connectionString,
        Messages? messages = null,
        SystemLogging? logging = null
    )
    {
        try
        {
            ArgumentNullException.ThrowIfNull(argument: connectionString);

            _messages = messages ?? new();
            _logging = logging;
            _client = new MongoClient(connectionString.ToMongoDB());
            _database = _client.GetDatabase(connectionString.Database);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }
}
