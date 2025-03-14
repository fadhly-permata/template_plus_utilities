namespace IDC.Utilities.Data;

public sealed partial class SQLiteHelper
{
    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    /// <returns>Current instance for chaining.</returns>
    /// <remarks>
    /// Creates new transaction if none exists.
    /// Logs warning if transaction already exists.
    /// </remarks>
    /// <example>
    /// <code>
    /// db.Connect()
    ///   .TransactionBegin()
    ///   .ExecuteNonQuery("INSERT INTO Users (Name) VALUES ('John')", out _)
    ///   .TransactionCommit();
    /// </code>
    /// </example>
    /// <exception cref="Exception">Rethrows any exceptions that occur during transaction start.</exception>
    public SQLiteHelper TransactionBegin()
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

    /// <summary>
    /// Commits current transaction with optional restart.
    /// </summary>
    /// <param name="reinitTransactions">Whether to start new transaction after commit.</param>
    /// <returns>Current instance for chaining.</returns>
    /// <remarks>
    /// Commits active transaction.
    /// Optionally starts new transaction after commit.
    /// Logs warning if no transaction exists.
    /// </remarks>
    /// <example>
    /// <code>
    /// db.Connect()
    ///   .TransactionBegin()
    ///   .ExecuteNonQuery("INSERT INTO Users (Name) VALUES ('John')", out _)
    ///   .TransactionCommit(reinitTransactions: true)
    ///   .ExecuteNonQuery("INSERT INTO Users (Name) VALUES ('Jane')", out _)
    ///   .TransactionCommit();
    /// </code>
    /// </example>
    /// <exception cref="Exception">Rethrows any exceptions that occur during commit.</exception>
    public SQLiteHelper TransactionCommit(bool reinitTransactions = false)
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

    /// <summary>
    /// Rolls back current transaction with optional restart.
    /// </summary>
    /// <param name="reinitTransactions">Whether to start new transaction after rollback.</param>
    /// <returns>Current instance for chaining.</returns>
    /// <remarks>
    /// Rolls back active transaction.
    /// Optionally starts new transaction after rollback.
    /// Logs warning if no transaction exists.
    /// </remarks>
    /// <example>
    /// <code>
    /// db.Connect()
    ///   .TransactionBegin()
    ///   .ExecuteNonQuery("INSERT INTO Users (Name) VALUES ('John')", out _)
    ///   .TransactionRollback(reinitTransactions: true)
    ///   .ExecuteNonQuery("INSERT INTO Users (Name) VALUES ('Jane')", out _)
    ///   .TransactionCommit();
    /// </code>
    /// </example>
    /// <exception cref="Exception">Rethrows any exceptions that occur during rollback.</exception>
    public SQLiteHelper TransactionRollback(bool reinitTransactions = false)
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

    /// <summary>
    /// Removes current transaction with optional commit.
    /// </summary>
    /// <param name="commitTransaction">Whether to commit before removing.</param>
    /// <param name="reinitTransactions">Whether to start new transaction after removal.</param>
    /// <returns>Current instance for chaining.</returns>
    /// <remarks>
    /// Optionally commits active transaction.
    /// Removes transaction from context.
    /// Optionally starts new transaction.
    /// Logs warning if no transaction exists.
    /// </remarks>
    /// <example>
    /// <code>
    /// db.Connect()
    ///   .TransactionBegin()
    ///   .ExecuteNonQuery("INSERT INTO Users (Name) VALUES ('John')", out _)
    ///   .TransactionRemove(commitTransaction: true, reinitTransactions: true)
    ///   .ExecuteNonQuery("INSERT INTO Users (Name) VALUES ('Jane')", out _)
    ///   .TransactionCommit();
    /// </code>
    /// </example>
    /// <exception cref="Exception">Rethrows any exceptions that occur during transaction removal.</exception>
    public SQLiteHelper TransactionRemove(
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
