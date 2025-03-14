namespace IDC.Utilities.Data;

public sealed partial class MongoHelper
{
    /// <summary>
    /// Begins a MongoDB transaction.
    /// </summary>
    /// <remarks>
    /// Initiates a new transaction if one is not already in progress.
    /// If a transaction is already active, a warning will be logged.
    ///
    /// Example:
    /// <code>
    /// var mongo = new MongoHelper()
    ///     .Connect()
    ///     .TransactionBegin()
    ///     .Insert(collection: "users", document: userDoc)
    ///     .TransactionCommit();
    /// </code>
    /// </remarks>
    /// <returns>The current <see cref="MongoHelper"/> instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the helper has been disposed.</exception>
    /// <exception cref="Exception">Thrown when transaction initialization fails.</exception>
    public MongoHelper TransactionBegin()
    {
        try
        {
            ThrowIfDisposed();

            if (_session?.IsInTransaction ?? false)
            {
                _logging?.LogWarning(message: _messages.MSG_TRANSACTION_STARTED);
                return this;
            }

            _session?.StartTransaction();
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Asynchronously begins a MongoDB transaction.
    /// </summary>
    /// <remarks>
    /// Initiates a new transaction asynchronously if one is not already in progress.
    /// If a transaction is already active, a warning will be logged.
    /// Executes an optional callback after transaction initialization.
    ///
    /// Example:
    /// <code>
    /// await mongoHelper.TransactionBeginAsync(
    ///     callback: helper => Console.WriteLine("Transaction started"),
    ///     cancellationToken: cts.Token
    /// );
    /// </code>
    /// </remarks>
    /// <param name="callback">Optional callback action to execute after transaction begins.</param>
    /// <param name="cancellationToken">Optional token to cancel the async operation.</param>
    /// <returns>A task representing the async operation, containing the current <see cref="MongoHelper"/> instance.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the helper has been disposed.</exception>
    /// <exception cref="Exception">Thrown when transaction initialization fails.</exception>
    public async Task<MongoHelper> TransactionBeginAsync(
        Action<MongoHelper>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ThrowIfDisposed();

            if (_session?.IsInTransaction ?? false)
            {
                _logging?.LogWarning(message: _messages.MSG_TRANSACTION_STARTED);
                return this;
            }

            await Task.Run(
                function: () =>
                {
                    _session?.StartTransaction();
                    return Task.CompletedTask;
                },
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
    /// Commits the current MongoDB transaction.
    /// </summary>
    /// <remarks>
    /// Commits the active transaction if one exists.
    /// No action is taken if no transaction is in progress.
    ///
    /// Example:
    /// <code>
    /// mongoHelper
    ///     .Insert(collection: "orders", document: orderDoc)
    ///     .TransactionCommit();
    /// </code>
    /// </remarks>
    /// <returns>The current <see cref="MongoHelper"/> instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the helper has been disposed.</exception>
    /// <exception cref="Exception">Thrown when commit operation fails.</exception>
    public MongoHelper TransactionCommit()
    {
        try
        {
            ThrowIfDisposed();

            if (_session?.IsInTransaction ?? false)
                _session.CommitTransaction();

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Asynchronously commits the current MongoDB transaction.
    /// </summary>
    /// <remarks>
    /// Commits the active transaction asynchronously if one exists.
    /// No action is taken if no transaction is in progress.
    /// Executes an optional callback after commit.
    ///
    /// Example:
    /// <code>
    /// await mongoHelper
    ///     .InsertAsync(collection: "products", document: productDoc)
    ///     .TransactionCommitAsync(
    ///         callback: helper => Console.WriteLine("Transaction committed"),
    ///         cancellationToken: cts.Token
    ///     );
    /// </code>
    /// </remarks>
    /// <param name="callback">Optional callback action to execute after commit.</param>
    /// <param name="cancellationToken">Optional token to cancel the async operation.</param>
    /// <returns>A task representing the async operation, containing the current <see cref="MongoHelper"/> instance.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the helper has been disposed.</exception>
    /// <exception cref="Exception">Thrown when commit operation fails.</exception>
    public async Task<MongoHelper> TransactionCommitAsync(
        Action<MongoHelper>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ThrowIfDisposed();

            if (_session?.IsInTransaction ?? false)
                await _session.CommitTransactionAsync(cancellationToken: cancellationToken);

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
    /// Rolls back the current MongoDB transaction.
    /// </summary>
    /// <remarks>
    /// Aborts and rolls back the active transaction if one exists.
    /// No action is taken if no transaction is in progress.
    ///
    /// Example:
    /// <code>
    /// try
    /// {
    ///     mongoHelper
    ///         .Insert(collection: "payments", document: paymentDoc)
    ///         .TransactionCommit();
    /// }
    /// catch
    /// {
    ///     mongoHelper.TransactionRollback();
    ///     throw;
    /// }
    /// </code>
    /// </remarks>
    /// <returns>The current <see cref="MongoHelper"/> instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the helper has been disposed.</exception>
    /// <exception cref="Exception">Thrown when rollback operation fails.</exception>
    public MongoHelper TransactionRollback()
    {
        try
        {
            ThrowIfDisposed();

            if (_session?.IsInTransaction ?? false)
                _session.AbortTransaction();

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Asynchronously rolls back the current MongoDB transaction.
    /// </summary>
    /// <remarks>
    /// Aborts and rolls back the active transaction asynchronously if one exists.
    /// No action is taken if no transaction is in progress.
    /// Executes an optional callback after rollback.
    ///
    /// Example:
    /// <code>
    /// try
    /// {
    ///     await mongoHelper
    ///         .InsertAsync(collection: "transfers", document: transferDoc)
    ///         .TransactionCommitAsync();
    /// }
    /// catch
    /// {
    ///     await mongoHelper.TransactionRollbackAsync(
    ///         callback: helper => Console.WriteLine("Transaction rolled back"),
    ///         cancellationToken: cts.Token
    ///     );
    ///     throw;
    /// }
    /// </code>
    /// </remarks>
    /// <param name="callback">Optional callback action to execute after rollback.</param>
    /// <param name="cancellationToken">Optional token to cancel the async operation.</param>
    /// <returns>A task representing the async operation, containing the current <see cref="MongoHelper"/> instance.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the helper has been disposed.</exception>
    /// <exception cref="Exception">Thrown when rollback operation fails.</exception>
    public async Task<MongoHelper> TransactionRollbackAsync(
        Action<MongoHelper>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ThrowIfDisposed();

            if (_session?.IsInTransaction ?? false)
                await _session.AbortTransactionAsync(cancellationToken: cancellationToken);

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
