namespace IDC.Utilities.Data;

public sealed partial class SQLiteHelper
{
    /// <summary>
    /// Disposes of the SQLiteHelper instance and cleans up resources.
    /// </summary>
    /// <remarks>
    /// Performs cleanup by:
    /// - Closing database connection
    /// - Clearing connection pool
    /// - Disposing connection object
    /// - Setting disposed flag
    /// </remarks>
    /// <example>
    /// <code>
    /// using (var db = new SQLiteHelper("Data Source=mydb.sqlite"))
    /// {
    ///     // Database operations
    /// } // Automatically disposes when leaving scope
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose">Implementing Dispose</seealso>
    public void Dispose()
    {
        if (_disposed)
        {
            _logging?.LogWarning(message: _messages.MSG_ALREADY_DISPOSED);
            return;
        }

        CleanupConnection();

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
