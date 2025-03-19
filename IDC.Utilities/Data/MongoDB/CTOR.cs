using IDC.Utilities.Models.Data;
using MongoDB.Driver;

namespace IDC.Utilities.Data;

public sealed partial class MongoHelper
{
    /// <summary>
    /// Initializes MongoHelper with connection string and database name.
    /// </summary>
    /// <param name="connectionString">MongoDB connection string.</param>
    /// <param name="database">Database name.</param>
    /// <param name="messages">Optional custom messages.</param>
    /// <param name="logging">Optional logging instance.</param>
    /// <remarks>
    /// Creates MongoDB client and connects to specified database.
    /// Optionally configures logging.
    /// </remarks>
    /// <example>
    /// <code>
    /// var db = new MongoHelper("mongodb://localhost:27017", "mydb");
    /// </code>
    /// </example>
    /// <exception cref="ArgumentException">Thrown when connection string or database name is null or empty.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during initialization.</exception>
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
    /// Initializes MongoHelper with common connection string.
    /// </summary>
    /// <param name="connectionString">Common connection string instance.</param>
    /// <param name="messages">Optional custom messages.</param>
    /// <param name="logging">Optional logging instance.</param>
    /// <remarks>
    /// Creates MongoDB client using common format.
    /// Optionally configures logging.
    /// </remarks>
    /// <example>
    /// <code>
    /// var connStr = new CommonConnectionString {
    ///     Host = "localhost",
    ///     Database = "mydb"
    /// };
    /// var db = new MongoHelper(connStr);
    /// </code>
    /// </example>
    /// <exception cref="ArgumentNullException">Thrown when connection string is null.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during initialization.</exception>
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
