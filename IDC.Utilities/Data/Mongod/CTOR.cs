using IDC.Utilities.Models.Data;
using MongoDB.Driver;

namespace IDC.Utilities.Data;

public sealed partial class MongoHelper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MongoHelper"/> class using a MongoDB connection string.
    /// </summary>
    /// <param name="connectionString">The MongoDB connection string.</param>
    /// <param name="databaseSettings">Optional settings for the MongoDB database.</param>
    /// <param name="messages">Optional custom messages handler.</param>
    /// <param name="logging">Optional system logging handler.</param>
    /// <returns>A new instance of <see cref="MongoHelper"/>.</returns>
    /// <remarks>
    /// Creates a MongoDB client and connects to the specified database.
    /// If no database is specified in credentials, defaults to "admin".
    ///
    /// Example:
    /// <code>
    /// var mongo = new MongoHelper(
    ///     connectionString: "mongodb://localhost:27017",
    ///     databaseSettings: new MongoDatabaseSettings(),
    ///     messages: new Messages(),
    ///     logging: new SystemLogging()
    /// );
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when <paramref name="connectionString"/> is null or whitespace.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during initialization.</exception>
    /// <see href="https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/connection/connect/#std-label-connect-to-mongodb"/>
    public MongoHelper(
        string connectionString,
        MongoDatabaseSettings? databaseSettings = null,
        Messages? messages = null,
        SystemLogging? logging = null
    )
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(argument: connectionString);

            _messages = messages ?? new Messages();
            _logging = logging;
            _client = new MongoClient(connectionString: connectionString);
            _database = _client.GetDatabase(
                name: _client.Settings.Credential?.Source ?? "admin",
                settings: databaseSettings
            );
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoHelper"/> class using a common connection string.
    /// </summary>
    /// <param name="connectionString">The common connection string object.</param>
    /// <param name="messages">Optional custom messages handler.</param>
    /// <param name="logging">Optional system logging handler.</param>
    /// <returns>A new instance of <see cref="MongoHelper"/>.</returns>
    /// <remarks>
    /// Creates a MongoDB client using a common connection string format.
    /// If no database is specified, defaults to "admin".
    ///
    /// Example:
    /// <code>
    /// var commonConn = new CommonConnectionString {
    ///     Host = "localhost",
    ///     Port = 27017,
    ///     Database = "mydb"
    /// };
    /// var mongo = new MongoHelper(
    ///     connectionString: commonConn,
    ///     messages: new Messages(),
    ///     logging: new SystemLogging()
    /// );
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="connectionString"/> is null.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during initialization.</exception>
    /// <see href="https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/connection/connect/#std-label-connect-to-mongodb"/>
    public MongoHelper(
        CommonConnectionString connectionString,
        Messages? messages = null,
        SystemLogging? logging = null
    )
    {
        try
        {
            ArgumentNullException.ThrowIfNull(argument: connectionString);

            _messages = messages ?? new Messages();
            _logging = logging;
            _client = new MongoClient(connectionString: connectionString.ToMongoDB());
            _database = _client.GetDatabase(connectionString.Database ?? "admin");
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }
}
