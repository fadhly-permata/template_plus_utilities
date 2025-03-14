namespace IDC.Utilities.Data;

public sealed partial class PostgreHelper
{
    public PostgreHelper TransactionBegin()
    {
        try
        {
            ThrowIfDisposed();

            if (_dbTrans is not null)
            {
                _logging?.LogWarning(message: _messages.MSG_TRANSACTION_STARTED);
                return this;
            }

            _dbTrans = _connection.BeginTransaction();
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public PostgreHelper TransactionCommit(bool reinitTransactions = false)
    {
        try
        {
            ThrowIfDisposed();

            if (_dbTrans is null)
            {
                _logging?.LogWarning(message: _messages.MSG_TRANSACTION_NOT_STARTED);
                return this;
            }

            _dbTrans.Commit();

            if (!reinitTransactions)
            {
                _dbTrans.Dispose();
                _dbTrans = null;
            }
            else
            {
                _dbTrans = _connection.BeginTransaction();
            }

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public PostgreHelper TransactionRollback(bool reinitTransactions = false)
    {
        try
        {
            ThrowIfDisposed();

            if (_dbTrans is null)
            {
                _logging?.LogWarning(message: _messages.MSG_TRANSACTION_NOT_STARTED);
                return this;
            }

            _dbTrans.Rollback();

            if (!reinitTransactions)
            {
                _dbTrans.Dispose();
                _dbTrans = null;
            }
            else
            {
                _dbTrans = _connection.BeginTransaction();
            }

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public PostgreHelper TransactionRemove(
        bool commitTransaction = false,
        bool reinitTransactions = false
    )
    {
        try
        {
            ThrowIfDisposed();

            if (_dbTrans is null)
            {
                _logging?.LogWarning(message: _messages.MSG_TRANSACTION_NOT_STARTED);
                return this;
            }

            if (commitTransaction)
                _dbTrans.Commit();
            else
                _dbTrans.Rollback();

            _dbTrans.Dispose();
            _dbTrans = null;

            if (reinitTransactions)
                _dbTrans = _connection.BeginTransaction();

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }
}
