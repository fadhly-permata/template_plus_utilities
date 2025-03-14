namespace IDC.Utilities.Data;

public sealed partial class PostgreHelper
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

    /// <summary>
    /// Asynchronously begins a new database transaction.
    /// </summary>
    /// <param name="callback">Optional callback action to execute after transaction begins.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Task representing the asynchronous operation returning current instance for chaining.</returns>
    /// <remarks>
    /// Creates new transaction if none exists.
    /// Logs warning if transaction already exists.
    /// Executes callback if provided after successful transaction start.
    ///
    /// Example:
    /// <code>
    /// await db.Connect()
    ///   .TransactionBeginAsync(
    ///     callback: helper => Console.WriteLine($"Transaction started on {helper._connection.Database}"),
    ///     cancellationToken: cts.Token
    ///   );
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when the helper is disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when operation is canceled via cancellationToken.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during transaction start.</exception>
    /// <see href="https://www.npgsql.org/doc/transactions.html">Npgsql Transactions Documentation</see>
    public async Task<PostgreHelper> TransactionBeginAsync(
        Action<PostgreHelper>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ThrowIfDisposed();

            if (_dbTrans is not null)
            {
                _logging?.LogWarning(message: _messages.MSG_TRANSACTION_STARTED);
                return this;
            }

            _dbTrans = await _connection.BeginTransactionAsync(
                cancellationToken: cancellationToken
            );

            callback?.Invoke(obj: this);
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

    /// <summary>
    /// Asynchronously commits current transaction with optional restart.
    /// </summary>
    /// <param name="reinitTransactions">Whether to start new transaction after commit.</param>
    /// <param name="callback">Optional callback action to execute after commit.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Task representing the asynchronous operation returning current instance for chaining.</returns>
    /// <remarks>
    /// Commits active transaction asynchronously.
    /// Optionally starts new transaction after commit.
    /// Executes callback if provided after successful commit.
    /// Logs warning if no transaction exists.
    ///
    /// Example:
    /// <code>
    /// await db.Connect()
    ///   .TransactionBeginAsync()
    ///   .ExecuteNonQueryAsync("INSERT INTO Users (Name) VALUES ('John')")
    ///   .TransactionCommitAsync(
    ///     reinitTransactions: true,
    ///     callback: helper => Console.WriteLine($"Transaction committed on {helper._connection.Database}"),
    ///     cancellationToken: cts.Token
    ///   );
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when the helper is disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when operation is canceled via cancellationToken.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during commit.</exception>
    /// <see href="https://www.npgsql.org/doc/transactions.html">Npgsql Transactions Documentation</see>
    public async Task<PostgreHelper> TransactionCommitAsync(
        bool reinitTransactions = false,
        Action<PostgreHelper>? callback = null,
        CancellationToken cancellationToken = default
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

            await _dbTrans.CommitAsync(cancellationToken: cancellationToken);

            if (!reinitTransactions)
            {
                await _dbTrans.DisposeAsync();
                _dbTrans = null;
            }
            else
            {
                _dbTrans = await _connection.BeginTransactionAsync(
                    cancellationToken: cancellationToken
                );
            }

            callback?.Invoke(obj: this);
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

    /// <summary>
    /// Asynchronously rolls back current transaction with optional restart.
    /// </summary>
    /// <param name="reinitTransactions">Whether to start new transaction after rollback.</param>
    /// <param name="callback">Optional callback action to execute after rollback.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Task representing the asynchronous operation returning current instance for chaining.</returns>
    /// <remarks>
    /// Rolls back active transaction asynchronously.
    /// Optionally starts new transaction after rollback.
    /// Executes callback if provided after successful rollback.
    /// Logs warning if no transaction exists.
    ///
    /// Example:
    /// <code>
    /// await db.Connect()
    ///   .TransactionBeginAsync()
    ///   .ExecuteNonQueryAsync("INSERT INTO Users (Name) VALUES ('John')")
    ///   .TransactionRollbackAsync(
    ///     reinitTransactions: true,
    ///     callback: helper => Console.WriteLine($"Transaction rolled back on {helper._connection.Database}"),
    ///     cancellationToken: cts.Token
    ///   );
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when the helper is disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when operation is canceled via cancellationToken.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during rollback.</exception>
    /// <see href="https://www.npgsql.org/doc/transactions.html">Npgsql Transactions Documentation</see>
    public async Task<PostgreHelper> TransactionRollbackAsync(
        bool reinitTransactions = false,
        Action<PostgreHelper>? callback = null,
        CancellationToken cancellationToken = default
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

            await _dbTrans.RollbackAsync(cancellationToken: cancellationToken);

            if (!reinitTransactions)
            {
                await _dbTrans.DisposeAsync();
                _dbTrans = null;
            }
            else
            {
                _dbTrans = await _connection.BeginTransactionAsync(
                    cancellationToken: cancellationToken
                );
            }

            callback?.Invoke(obj: this);
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
    /// <exception cref="Exception">Rethrows any exceptions that occur during removal.</exception>
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

    /// <summary>
    /// Asynchronously removes current transaction with optional commit.
    /// </summary>
    /// <param name="commitTransaction">Whether to commit before removing.</param>
    /// <param name="reinitTransactions">Whether to start new transaction after removal.</param>
    /// <param name="callback">Optional callback action to execute after removal.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Task representing the asynchronous operation returning current instance for chaining.</returns>
    /// <remarks>
    /// Optionally commits active transaction asynchronously.
    /// Removes transaction from context.
    /// Optionally starts new transaction.
    /// Executes callback if provided after successful removal.
    /// Logs warning if no transaction exists.
    ///
    /// Example:
    /// <code>
    /// await db.Connect()
    ///   .TransactionBeginAsync()
    ///   .ExecuteNonQueryAsync("INSERT INTO Users (Name) VALUES ('John')")
    ///   .TransactionRemoveAsync(
    ///     commitTransaction: true,
    ///     reinitTransactions: true,
    ///     callback: helper => Console.WriteLine($"Transaction removed on {helper._connection.Database}"),
    ///     cancellationToken: cts.Token
    ///   );
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when the helper is disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when operation is canceled via cancellationToken.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during removal.</exception>
    /// <see href="https://www.npgsql.org/doc/transactions.html">Npgsql Transactions Documentation</see>
    public async Task<PostgreHelper> TransactionRemoveAsync(
        bool commitTransaction = false,
        bool reinitTransactions = false,
        Action<PostgreHelper>? callback = null,
        CancellationToken cancellationToken = default
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
                await _dbTrans.CommitAsync(cancellationToken: cancellationToken);
            else
                await _dbTrans.RollbackAsync(cancellationToken: cancellationToken);

            await _dbTrans.DisposeAsync();
            _dbTrans = null;

            if (reinitTransactions)
                _dbTrans = await _connection.BeginTransactionAsync(
                    cancellationToken: cancellationToken
                );

            callback?.Invoke(obj: this);
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }
}
