namespace IDC.Utilities.Data;

public sealed partial class PostgreHelper
{
    public PostgreHelper Connect(bool useTransaction = false)
    {
        try
        {
            return CreateConnection(useTransaction: useTransaction);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public PostgreHelper Reconnect(bool useTransaction = false)
    {
        try
        {
            return CreateConnection(useTransaction: useTransaction, disconnectFirst: true);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public PostgreHelper Disconnect(bool commitTransaction = false)
    {
        try
        {
            ThrowIfDisposed();

            if (_dbTrans is not null)
            {
                if (commitTransaction)
                {
                    _dbTrans.Commit();
                }
                _dbTrans.Dispose();
                _dbTrans = null;
            }

            if (IsConnected())
                _connection.Close();

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public PostgreHelper ChangeDB(string database)
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(database);

            if (database == _connection.Database)
                return this;

            _connection.ChangeDatabase(database);
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }
}
