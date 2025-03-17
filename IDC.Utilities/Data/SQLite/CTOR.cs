using IDC.Utilities.Models.Data;

namespace IDC.Utilities.Data;

public sealed partial class SQLiteHelper
{
    /// <summary>
    /// Initializes a new instance using a connection string.
    /// </summary>
    /// <param name="connectionString">SQLite connection string.</param>
    /// <param name="messages">Optional custom messages.</param>
    /// <param name="logging">Optional logging instance.</param>
    /// <exception cref="ArgumentException">Thrown when connectionString is null or empty.</exception>
    /// <remarks>
    /// Creates a new SQLiteHelper instance and immediately opens the database connection.
    /// </remarks>
    /// <example>
    /// <code>
    /// var db = new SQLiteHelper("Data Source=mydb.sqlite");
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/connection-strings">SQLite Connection Strings</seealso>
    public SQLiteHelper(
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
    /// Initializes a new instance using a common connection string.
    /// </summary>
    /// <param name="connectionString">Common connection string instance.</param>
    /// <param name="messages">Optional custom messages.</param>
    /// <param name="logging">Optional logging instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when connectionString is null.</exception>
    /// <remarks>
    /// Creates a new SQLiteHelper instance using a CommonConnectionString object and immediately opens the database connection.
    /// </remarks>
    /// <example>
    /// <code>
    /// var connStr = new CommonConnectionString { Database = "mydb.sqlite" };
    /// var db = new SQLiteHelper(connStr);
    /// </code>
    /// </example>
    /// <seealso cref="CommonConnectionString"/>
    public SQLiteHelper(
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
            _connection = new(connectionString: connectionString.ToSQLite());
            _connection.Open();
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Initializes a new instance using in-memory database.
    /// </summary>
    /// <param name="cached">Whether to use shared cache (default: true).</param>
    /// <param name="messages">Optional custom messages.</param>
    /// <param name="logging">Optional logging instance.</param>
    /// <remarks>
    /// Creates a new SQLiteHelper instance using in-memory database.
    /// Shared cache allows multiple connections to access the same in-memory database.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Private cache (isolated database)
    /// var db1 = new SQLiteHelper(cached: false);
    ///
    /// // Shared cache (shared database)
    /// var db2 = new SQLiteHelper(cached: true);
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/in-memory-databases"/>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/connection-strings#cache"/>
    public SQLiteHelper(
        bool cached = true,
        Messages? messages = null,
        SystemLogging? logging = null
    )
        : this(
            connectionString: new CommonConnectionString().ToSQLiteInMemory(cached: cached),
            messages: messages,
            logging: logging
        ) { }
}
