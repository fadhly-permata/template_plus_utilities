using IDC.Utilities.Models.Data;

namespace IDC.Utilities.Data;

public sealed partial class PostgreHelper
{
    /// <summary>
    /// Initializes PostgreHelper with connection string.
    /// </summary>
    /// <param name="connectionString">PostgreSQL connection string.</param>
    /// <param name="messages">Optional custom messages.</param>
    /// <param name="logging">Optional logging instance.</param>
    /// <remarks>
    /// Creates and opens database connection.
    /// Optionally configures logging.
    /// </remarks>
    /// <example>
    /// <code>
    /// var db = new PostgreHelper("Host=localhost;Database=mydb;Username=user;Password=pass");
    /// </code>
    /// </example>
    /// <exception cref="ArgumentException">Thrown when connection string is null or empty.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during initialization.</exception>
    public PostgreHelper(
        string connectionString,
        Messages? messages = null,
        SystemLogging? logging = null
    )
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            _messages = messages ?? new();
            _logging = logging;
            _connection = new(connectionString: connectionString);
            _connection.Open();
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Initializes PostgreHelper with common connection string.
    /// </summary>
    /// <param name="connectionString">Common connection string instance.</param>
    /// <param name="messages">Optional custom messages.</param>
    /// <param name="logging">Optional logging instance.</param>
    /// <remarks>
    /// Creates and opens database connection using common format.
    /// Optionally configures logging.
    /// </remarks>
    /// <example>
    /// <code>
    /// var connStr = new CommonConnectionString {
    ///     Host = "localhost",
    ///     Database = "mydb"
    /// };
    /// var db = new PostgreHelper(connStr);
    /// </code>
    /// </example>
    /// <exception cref="ArgumentNullException">Thrown when connection string is null.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during initialization.</exception>
    public PostgreHelper(
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
            _connection = new(connectionString: connectionString.ToPostgreSQL());
            _connection.Open();
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }
}
