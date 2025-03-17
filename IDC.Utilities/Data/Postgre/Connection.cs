namespace IDC.Utilities.Data;

public sealed partial class PostgreHelper
{
    /// <summary>
    /// Connects to database with optional transaction.
    /// </summary>
    /// <param name="useTransaction">Whether to start transaction.</param>
    /// <returns>Current instance for chaining.</returns>
    /// <remarks>
    /// Opens database connection if not already open.
    /// Optionally starts new transaction.
    /// </remarks>
    /// <example>
    /// <code>
    /// db.Connect(useTransaction: true)
    ///   .ExecuteNonQuery("INSERT INTO Users (Name) VALUES ('John')", out _)
    ///   .TransactionCommit();
    /// </code>
    /// </example>
    /// <exception cref="Exception">Rethrows any exceptions that occur during connection.</exception>
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

    /// <summary>
    /// Reconnects to database with optional transaction.
    /// </summary>
    /// <param name="useTransaction">Whether to start transaction.</param>
    /// <returns>Current instance for chaining.</returns>
    /// <remarks>
    /// Forces connection close then reopens connection.
    /// Optionally starts new transaction.
    /// </remarks>
    /// <example>
    /// <code>
    /// db.Reconnect(useTransaction: true)
    ///   .ExecuteNonQuery("INSERT INTO Users (Name) VALUES ('John')", out _)
    ///   .TransactionCommit();
    /// </code>
    /// </example>
    /// <exception cref="Exception">Rethrows any exceptions that occur during reconnection.</exception>
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

    /// <summary>
    /// Disconnects from database with optional transaction commit.
    /// </summary>
    /// <param name="commitTransaction">Whether to commit transaction before disconnecting.</param>
    /// <returns>Current instance for chaining.</returns>
    /// <remarks>
    /// Handles active transaction before disconnecting.
    /// Closes database connection.
    /// </remarks>
    /// <example>
    /// <code>
    /// db.Connect()
    ///   .ExecuteNonQuery("INSERT INTO Users (Name) VALUES ('John')", out _)
    ///   .Disconnect(commitTransaction: true);
    /// </code>
    /// </example>
    /// <exception cref="Exception">Rethrows any exceptions that occur during disconnection.</exception>
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

    /// <summary>
    /// Changes current database.
    /// </summary>
    /// <param name="database">Name of database to switch to.</param>
    /// <returns>Current instance for chaining.</returns>
    /// <remarks>
    /// Changes active database if different from current.
    /// No effect if same database specified.
    /// </remarks>
    /// <example>
    /// <code>
    /// db.Connect()
    ///   .ChangeDB("new_database")
    ///   .ExecuteNonQuery("INSERT INTO Users (Name) VALUES ('John')", out _);
    /// </code>
    /// </example>
    /// <exception cref="ArgumentException">Thrown when database name is null or empty.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during database change.</exception>
    public PostgreHelper ChangeDB(string database)
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(argument: database);

            if (database == _connection.Database)
                return this;

            _connection.ChangeDatabase(dbName: database);
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Asynchronously connects to database with optional transaction.
    /// </summary>
    /// <param name="useTransaction">Whether to start transaction.</param>
    /// <param name="callback">Optional callback action to execute after connection.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Task representing the asynchronous operation returning current instance for chaining.</returns>
    /// <remarks>
    /// Opens database connection asynchronously if not already open.
    /// Optionally starts new transaction.
    /// Executes callback if provided after successful connection.
    ///
    /// Example:
    /// <code>
    /// await db.ConnectAsync(
    ///     useTransaction: true,
    ///     callback: helper => Console.WriteLine($"Connected to {helper._connection.Database}")
    /// );
    /// </code>
    /// </remarks>
    /// <exception cref="OperationCanceledException">Thrown when operation is canceled via cancellationToken.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during connection.</exception>
    public async Task<PostgreHelper> ConnectAsync(
        bool useTransaction = false,
        Action<PostgreHelper>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var result = await CreateConnectionAsync(
                useTransaction: useTransaction,
                cancellationToken: cancellationToken
            );

            callback?.Invoke(obj: result);
            return result;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Asynchronously reconnects to database with optional transaction.
    /// </summary>
    /// <param name="useTransaction">Whether to start transaction.</param>
    /// <param name="callback">Optional callback action to execute after reconnection.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Task representing the asynchronous operation returning current instance for chaining.</returns>
    /// <remarks>
    /// Forces connection close then reopens connection asynchronously.
    /// Optionally starts new transaction.
    /// Executes callback if provided after successful reconnection.
    ///
    /// Example:
    /// <code>
    /// await db.ReconnectAsync(
    ///     useTransaction: true,
    ///     callback: helper => Console.WriteLine($"Reconnected to {helper._connection.Database}")
    /// );
    /// </code>
    /// </remarks>
    /// <exception cref="OperationCanceledException">Thrown when operation is canceled via cancellationToken.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during reconnection.</exception>
    public async Task<PostgreHelper> ReconnectAsync(
        bool useTransaction = false,
        Action<PostgreHelper>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var result = await CreateConnectionAsync(
                useTransaction: useTransaction,
                disconnectFirst: true,
                cancellationToken: cancellationToken
            );

            callback?.Invoke(obj: result);
            return result;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Asynchronously disconnects from database with optional transaction commit.
    /// </summary>
    /// <param name="commitTransaction">Whether to commit transaction before disconnecting.</param>
    /// <param name="callback">Optional callback action to execute after disconnection.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Task representing the asynchronous operation returning current instance for chaining.</returns>
    /// <remarks>
    /// Handles active transaction before disconnecting asynchronously.
    /// Commits transaction if specified.
    /// Closes database connection.
    /// Executes callback if provided after successful disconnection.
    ///
    /// Example:
    /// <code>
    /// await db.DisconnectAsync(
    ///     commitTransaction: true,
    ///     callback: _ => Console.WriteLine("Disconnected successfully")
    /// );
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when the helper is disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when operation is canceled via cancellationToken.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during disconnection.</exception>
    public async Task<PostgreHelper> DisconnectAsync(
        bool commitTransaction = false,
        Action<PostgreHelper>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ThrowIfDisposed();

            if (_dbTrans is not null)
            {
                if (commitTransaction)
                {
                    await _dbTrans.CommitAsync(cancellationToken: cancellationToken);
                }
                await _dbTrans.DisposeAsync();
                _dbTrans = null;
            }

            if (IsConnected())
                await _connection.CloseAsync();

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
    /// Asynchronously changes current database.
    /// </summary>
    /// <param name="database">Name of database to switch to.</param>
    /// <param name="callback">Optional callback action to execute after database change.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Task representing the asynchronous operation returning current instance for chaining.</returns>
    /// <remarks>
    /// Changes active database asynchronously if different from current.
    /// No effect if same database specified.
    /// Executes callback if provided after successful database change.
    ///
    /// Example:
    /// <code>
    /// await db.ChangeDBAsync(
    ///     database: "new_database",
    ///     callback: helper => Console.WriteLine($"Switched to {helper._connection.Database}")
    /// );
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when database name is null or empty.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the helper is disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when operation is canceled via cancellationToken.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during database change.</exception>
    public async Task<PostgreHelper> ChangeDBAsync(
        string database,
        Action<PostgreHelper>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(argument: database);

            if (database == _connection.Database)
                return this;

            await _connection.ChangeDatabaseAsync(
                databaseName: database,
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
