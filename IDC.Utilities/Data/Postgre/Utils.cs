using Npgsql;

namespace IDC.Utilities.Data;

public sealed partial class PostgreHelper
{
    private void ThrowIfDisposed()
    {
        _logging?.LogError(message: _messages.MSG_ALREADY_DISPOSED);
        ObjectDisposedException.ThrowIf(_disposed, nameof(PostgreHelper));
    }

    private void CleanupConnection()
    {
        try
        {
            if (IsConnected())
            {
                _connection.Close();
                NpgsqlConnection.ClearPool(connection: _connection);
                _connection?.Dispose();
            }
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

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
