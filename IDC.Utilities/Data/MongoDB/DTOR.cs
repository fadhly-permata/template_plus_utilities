namespace IDC.Utilities.Data;

public sealed partial class MongoHelper
{
    /// <summary>
    /// Disposes of the MongoHelper instance and cleans up resources.
    /// </summary>
    /// <param name="disposing">True if called from Dispose(), false if called from finalizer.</param>
    /// <remarks>
    /// Performs cleanup by:
    /// - Disconnecting from the database if disposing is true
    /// - Setting the disposed flag
    ///
    /// Example:
    /// <code>
    /// private void Dispose(bool disposing)
    /// {
    ///     if (!_disposed)
    ///     {
    ///         if (disposing)
    ///         {
    ///             Disconnect();
    ///         }
    ///         _disposed = true;
    ///     }
    /// }
    /// </code>
    /// </remarks>
    /// <seealso cref="Disconnect"/>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose">Implementing Dispose</seealso>
    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Disconnect();
            }
            _disposed = true;
        }
    }

    /// <summary>
    /// Disposes of the MongoHelper instance and cleans up resources.
    /// </summary>
    /// <remarks>
    /// Calls the Dispose(bool) method with true and suppresses finalization.
    ///
    /// Example:
    /// <code>
    /// using (var db = new MongoHelper("mongodb://localhost:27017"))
    /// {
    ///     // Database operations
    /// } // Automatically disposes when leaving scope
    /// </code>
    /// </remarks>
    /// <seealso cref="Dispose(bool)"/>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose">Implementing Dispose</seealso>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Asynchronously disposes of the MongoHelper instance and cleans up resources.
    /// </summary>
    /// <returns><see cref="ValueTask"/> representing the asynchronous dispose operation.</returns>
    /// <remarks>
    /// Performs asynchronous cleanup by:
    /// - Disposing the session if it exists
    /// - Setting the disposed flag
    /// - Suppressing finalization
    ///
    /// Example:
    /// <code>
    /// await using (var db = new MongoHelper("mongodb://localhost:27017"))
    /// {
    ///     // Database operations
    /// } // Automatically disposes when leaving scope
    /// </code>
    /// </remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync">Implementing DisposeAsync</seealso>
    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            if (_session is not null)
            {
                _session.Dispose();
                _session = null;
            }
            _disposed = true;
        }

        await ValueTask.CompletedTask;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizer for MongoHelper.
    /// </summary>
    /// <remarks>
    /// Calls Dispose(false) to clean up unmanaged resources.
    /// </remarks>
    /// <seealso cref="Dispose(bool)"/>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/destructors">Finalizers (C# Programming Guide)</seealso>
    ~MongoHelper() => Dispose(disposing: false);
}
