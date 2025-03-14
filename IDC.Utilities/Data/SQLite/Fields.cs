using Microsoft.Data.Sqlite;

namespace IDC.Utilities.Data;

public sealed partial class SQLiteHelper
{
    /// <summary>
    /// Optional logging instance for error and diagnostic messages.
    /// </summary>
    private readonly SystemLogging? _logging;

    /// <summary>
    /// SQLite database connection instance.
    /// </summary>
    private readonly SqliteConnection _connection;

    /// <summary>
    /// Current transaction instance, if any.
    /// </summary>
    private SqliteTransaction? _dbTrans;

    /// <summary>
    /// Customizable message strings for operations.
    /// </summary>
    private Messages _messages = new();

    /// <summary>
    /// Flag indicating if instance has been disposed.
    /// </summary>
    private bool _disposed;
}
