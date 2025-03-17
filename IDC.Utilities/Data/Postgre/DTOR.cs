namespace IDC.Utilities.Data;

public sealed partial class PostgreHelper
{
    /// <summary>
    /// Disposes of the PostgreHelper instance and cleans up resources.
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
    /// using (var db = new PostgreHelper("Host=localhost;Database=mydb"))
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

    /// <summary>
    /// Asynchronously disposes of the PostgreHelper instance and cleans up resources.
    /// </summary>
    /// <remarks>
    /// Performs asynchronous cleanup by:
    /// - Closing database connection
    /// - Clearing connection pool
    /// - Disposing connection object
    /// - Setting disposed flag
    ///
    /// Example:
    /// <code>
    /// await using (var db = new PostgreHelper("Host=localhost;Database=mydb"))
    /// {
    ///     // Database operations
    /// } // Automatically disposes when leaving scope
    /// </code>
    /// </remarks>
    /// <returns><see cref="ValueTask"/> representing the asynchronous dispose operation.</returns>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync">Implementing DisposeAsync</seealso>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            _logging?.LogWarning(message: _messages.MSG_ALREADY_DISPOSED);
            return;
        }

        await CleanupConnectionAsync();

        _disposed = true;
        await ValueTask.CompletedTask;
    }
}
