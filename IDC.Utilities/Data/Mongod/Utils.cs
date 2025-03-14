namespace IDC.Utilities.Data;

public sealed partial class MongoHelper
{
    /// <summary>
    /// Throws an <see cref="ObjectDisposedException"/> if the MongoHelper instance has been disposed.
    /// </summary>
    /// <remarks>
    /// Internal utility method used to check the disposal state before performing operations.
    ///
    /// Example:
    /// <code>
    /// private void SomeOperation()
    /// {
    ///     ThrowIfDisposed();
    ///     // Proceed with operation
    /// }
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when the helper instance has been disposed.</exception>
    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(condition: _disposed, instance: nameof(MongoHelper));
    }

    /// <summary>
    /// Cleans up the MongoDB session by aborting any active transaction and disposing the session.
    /// </summary>
    /// <remarks>
    /// Internal utility method that handles session cleanup. If a transaction is in progress, it will be aborted before disposal.
    ///
    /// Example:
    /// <code>
    /// private void Dispose()
    /// {
    ///     CleanupConnection();
    ///     _disposed = true;
    /// }
    /// </code>
    /// </remarks>
    /// <exception cref="Exception">Rethrows any exceptions that occur during cleanup after logging.</exception>
    private void CleanupConnection()
    {
        try
        {
            if (_session is not null)
            {
                if (_session.IsInTransaction)
                    _session.AbortTransaction();

                _session.Dispose();
            }
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Asynchronously cleans up the MongoDB session by aborting any active transaction and disposing the session.
    /// </summary>
    /// <remarks>
    /// Internal utility method that handles asynchronous session cleanup. If a transaction is in progress, it will be aborted before disposal.
    ///
    /// Example:
    /// <code>
    /// private async Task DisposeAsync()
    /// {
    ///     await CleanupConnectionAsync(cancellationToken: default);
    ///     _disposed = true;
    /// }
    /// </code>
    /// </remarks>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Rethrows any exceptions that occur during cleanup after logging.</exception>
    private async Task CleanupConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_session is not null)
            {
                if (_session.IsInTransaction)
                    await _session.AbortTransactionAsync(cancellationToken: cancellationToken);

                _session.Dispose();
            }
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }
}
