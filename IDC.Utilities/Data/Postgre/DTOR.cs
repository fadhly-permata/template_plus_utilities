namespace IDC.Utilities.Data;

public sealed partial class PostgreHelper
{
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
