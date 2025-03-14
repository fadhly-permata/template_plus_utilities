using Microsoft.Data.Sqlite;

namespace IDC.Utilities.Data;

public sealed partial class SQLiteHelper
{
    /// <summary>
    /// Throws ObjectDisposedException if the helper has been disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when accessing disposed instance.</exception>
    /// <remarks>
    /// Internal validation method to prevent operations on disposed instances.
    /// </remarks>
    private void ThrowIfDisposed()
    {
        _logging?.LogError(message: _messages.MSG_ALREADY_DISPOSED);
        ObjectDisposedException.ThrowIf(_disposed, nameof(SQLiteHelper));
    }

    /// <summary>
    /// Cleans up database connection resources.
    /// </summary>
    /// <remarks>
    /// Performs connection cleanup by:
    /// - Closing active connection
    /// - Clearing connection pool
    /// - Disposing connection object
    /// </remarks>
    /// <exception cref="Exception">Rethrows any exceptions that occur during cleanup.</exception>
    private void CleanupConnection()
    {
        try
        {
            if (IsConnected())
            {
                _connection.Close();
                SqliteConnection.ClearPool(connection: _connection);
                _connection?.Dispose();
            }
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Validates database connection status.
    /// </summary>
    /// <param name="message">Status message output.</param>
    /// <returns>True if connection valid, false otherwise.</returns>
    /// <remarks>
    /// Checks if:
    /// - Instance not disposed
    /// - Connection is established
    /// </remarks>
    /// <exception cref="Exception">Rethrows any exceptions that occur during validation.</exception>
    private bool ConnectionIsValid(out string message)
    {
        try
        {
            ThrowIfDisposed();

            if (!IsConnected())
            {
                message = _messages.MSG_CONNECTION_NOT_ESTABLISHED;
                return false;
            }

            message = _messages.MSG_CONNECTION_ESTABLISHED;
            return true;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }
}
