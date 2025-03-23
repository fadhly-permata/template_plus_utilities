namespace IDC.Utilities.Data;

public sealed partial class MongoHelper
{
    /// <summary>
    /// Establishes a connection to the MongoDB database.
    /// </summary>
    /// <returns>The current <see cref="MongoHelper"/> instance.</returns>
    /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
    /// <exception cref="MongoDB.Driver.MongoConnectionException">Failed to establish database connection.</exception>
    /// <exception cref="MongoDB.Driver.MongoAuthenticationException">Invalid authentication credentials.</exception>
    /// <remarks>
    /// Establishes and validates a connection to the MongoDB database using the configured connection string.
    ///
    /// > [!IMPORTANT]
    /// > Ensure proper connection string configuration before calling this method.
    ///
    /// > [!NOTE]
    /// > This method is thread-safe and can be called multiple times safely.
    ///
    /// Example:
    /// <code>
    /// var mongo = new MongoHelper(connectionString: "mongodb://localhost:27017")
    ///     .Connect();
    /// </code>
    /// </remarks>
    public MongoHelper Connect()
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _logging?.LogInformation(message: _messages.MSG_CONNECTION_ESTABLISHED);
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Disconnects from the MongoDB database and releases the current session.
    /// </summary>
    /// <returns>The current <see cref="MongoHelper"/> instance.</returns>
    /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
    /// <exception cref="MongoDB.Driver.MongoException">Error occurred while disconnecting from database.</exception>
    /// <remarks>
    /// Safely closes the current MongoDB session and releases associated resources.
    ///
    /// > [!NOTE]
    /// > Safe to call even without an active connection.
    ///
    /// > [!TIP]
    /// > Always call this method in a finally block or use a using statement.
    ///
    /// Example:
    /// <code>
    /// using var mongo = new MongoHelper(connectionString: "mongodb://localhost:27017");
    /// try
    /// {
    ///     mongo.Connect()
    ///         .ExecuteCommand()
    ///         .Disconnect();
    /// }
    /// finally
    /// {
    ///     mongo.Disconnect();
    /// }
    /// </code>
    /// </remarks>
    public MongoHelper Disconnect()
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_session is not null)
            {
                _session.Dispose();
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
    /// Reestablishes the MongoDB database connection.
    /// </summary>
    /// <returns>The current <see cref="MongoHelper"/> instance.</returns>
    /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
    /// <exception cref="MongoDB.Driver.MongoConnectionException">Failed to reestablish database connection.</exception>
    /// <exception cref="MongoDB.Driver.MongoAuthenticationException">Invalid authentication credentials.</exception>
    /// <remarks>
    /// Performs a clean disconnect and reconnect sequence to reset the connection state.
    ///
    /// > [!CAUTION]
    /// > This operation will terminate all active queries and transactions.
    ///
    /// > [!TIP]
    /// > Useful for handling connection timeouts or stale connections.
    ///
    /// Example:
    /// <code>
    /// var mongo = new MongoHelper(connectionString: "mongodb://localhost:27017");
    /// try
    /// {
    ///     await mongo.Connect()
    ///         .ExecuteCommand()
    ///         .Reconnect()
    ///         .ExecuteAnotherCommand();
    /// }
    /// catch (MongoConnectionException ex)
    /// {
    ///     // Handle connection errors
    /// }
    /// </code>
    /// </remarks>
    public MongoHelper Reconnect()
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return Disconnect().Connect();
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }
}
