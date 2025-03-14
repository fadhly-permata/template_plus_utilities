namespace IDC.Utilities.Data;

public sealed partial class MongoHelper
{
    /// <summary>
    /// Connects to MongoDB server.
    /// </summary>
    /// <returns>Current instance for chaining.</returns>
    /// <remarks>
    /// Creates new connection to MongoDB server.
    ///
    /// Example:
    /// <code>
    /// var mongo = new MongoHelper()
    ///     .Connect()
    ///     .ChangeDB("myDatabase");
    /// </code>
    /// </remarks>
    /// <exception cref="Exception">Rethrows any exceptions that occur during connection.</exception>
    public MongoHelper Connect()
    {
        try
        {
            return CreateConnection();
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Connects to MongoDB server asynchronously.
    /// </summary>
    /// <param name="callback">Optional callback action to execute after connection.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Task containing current instance for chaining.</returns>
    /// <remarks>
    /// Creates new connection to MongoDB server.
    /// Executes callback if provided after successful connection.
    ///
    /// Example:
    /// <code>
    /// await mongo.ConnectAsync(
    ///     callback: helper => Console.WriteLine($"Connected to {helper._database.DatabaseNamespace.DatabaseName}"),
    ///     cancellationToken: cts.Token
    /// );
    /// </code>
    /// </remarks>
    /// <exception cref="OperationCanceledException">Thrown when operation is canceled via token.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during connection.</exception>
    public async Task<MongoHelper> ConnectAsync(
        Action<MongoHelper>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var result = await CreateConnectionAsync(cancellationToken: cancellationToken);
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
    /// Reconnects to MongoDB server.
    /// </summary>
    /// <returns>Current instance for chaining.</returns>
    /// <remarks>
    /// Forces disconnection then creates new connection.
    ///
    /// Example:
    /// <code>
    /// var mongo = new MongoHelper()
    ///     .Reconnect()
    ///     .ChangeDB("myDatabase");
    /// </code>
    /// </remarks>
    /// <exception cref="Exception">Rethrows any exceptions that occur during reconnection.</exception>
    public MongoHelper Reconnect()
    {
        try
        {
            return CreateConnection(disconnectFirst: true);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Reconnects to MongoDB server asynchronously.
    /// </summary>
    /// <param name="callback">Optional callback action to execute after reconnection.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Task containing current instance for chaining.</returns>
    /// <remarks>
    /// Forces disconnection then creates new connection.
    /// Executes callback if provided after successful reconnection.
    ///
    /// Example:
    /// <code>
    /// await mongo.ReconnectAsync(
    ///     callback: helper => Console.WriteLine($"Reconnected to {helper._database.DatabaseNamespace.DatabaseName}"),
    ///     cancellationToken: cts.Token
    /// );
    /// </code>
    /// </remarks>
    /// <exception cref="OperationCanceledException">Thrown when operation is canceled via token.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during reconnection.</exception>
    public async Task<MongoHelper> ReconnectAsync(
        Action<MongoHelper>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var result = await CreateConnectionAsync(
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
    /// Disconnects from MongoDB server.
    /// </summary>
    /// <returns>Current instance for chaining.</returns>
    /// <remarks>
    /// Closes active session and cleans up resources.
    ///
    /// Example:
    /// <code>
    /// mongo.Disconnect();
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when accessing disposed instance.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during disconnection.</exception>
    public MongoHelper Disconnect()
    {
        try
        {
            ThrowIfDisposed();

            if (_session is not null)
            {
                CleanupConnection();
                _session = null;
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
    /// Disconnects from MongoDB server asynchronously.
    /// </summary>
    /// <param name="callback">Optional callback action to execute after disconnection.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Task containing current instance for chaining.</returns>
    /// <remarks>
    /// Closes active session and cleans up resources.
    /// Executes callback if provided after successful disconnection.
    ///
    /// Example:
    /// <code>
    /// await mongo.DisconnectAsync(
    ///     callback: _ => Console.WriteLine("Disconnected successfully"),
    ///     cancellationToken: cts.Token
    /// );
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when accessing disposed instance.</exception>
    /// <exception cref="OperationCanceledException">Thrown when operation is canceled via token.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during disconnection.</exception>
    public async Task<MongoHelper> DisconnectAsync(
        Action<MongoHelper>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ThrowIfDisposed();

            if (_session is not null)
            {
                await CleanupConnectionAsync(cancellationToken: cancellationToken);
                _session = null;
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
    /// Changes current database.
    /// </summary>
    /// <param name="database">Name of database to switch to.</param>
    /// <returns>Current instance for chaining.</returns>
    /// <remarks>
    /// Switches to specified database.
    ///
    /// Example:
    /// <code>
    /// mongo.ChangeDB("new_database");
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when database name is null or empty.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when accessing disposed instance.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during database change.</exception>
    public MongoHelper ChangeDB(string database)
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(argument: database);

            _database = _client.GetDatabase(name: database);
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Changes current database asynchronously.
    /// </summary>
    /// <param name="database">Name of database to switch to.</param>
    /// <param name="callback">Optional callback action to execute after database change.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Task containing current instance for chaining.</returns>
    /// <remarks>
    /// Switches to specified database.
    /// Executes callback if provided after successful database change.
    ///
    /// Example:
    /// <code>
    /// await mongo.ChangeDBAsync(
    ///     database: "new_database",
    ///     callback: helper => Console.WriteLine($"Switched to {helper._database.DatabaseNamespace.DatabaseName}"),
    ///     cancellationToken: cts.Token
    /// );
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when database name is null or empty.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when accessing disposed instance.</exception>
    /// <exception cref="OperationCanceledException">Thrown when operation is canceled via token.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during database change.</exception>
    public async Task<MongoHelper> ChangeDBAsync(
        string database,
        Action<MongoHelper>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(argument: database);

            await Task.Run(
                function: () => _database = _client.GetDatabase(name: database),
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
