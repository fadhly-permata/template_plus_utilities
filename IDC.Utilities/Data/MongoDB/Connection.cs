namespace IDC.Utilities.Data;

public sealed partial class MongoHelper
{
    /// <summary>
    /// Establishes a connection to the MongoDB database.
    /// </summary>
    /// <returns>The current <see cref="MongoHelper"/> instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the object has been disposed.</exception>
    /// <exception cref="Exception">Rethrows any exception that occurs during the connection process.</exception>
    /// <remarks>
    /// This method attempts to establish a connection to the MongoDB database.
    /// If successful, it logs an information message.
    ///
    /// Example:
    /// <code>
    /// var mongo = new MongoHelper(connectionString);
    /// mongo.Connect();
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
    /// Disconnects from the MongoDB database and disposes of the current session.
    /// </summary>
    /// <returns>The current <see cref="MongoHelper"/> instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the object has been disposed.</exception>
    /// <exception cref="Exception">Rethrows any exception that occurs during the disconnection process.</exception>
    /// <remarks>
    /// This method closes the current MongoDB session if one exists.
    /// It's safe to call even if no session is currently active.
    ///
    /// Example:
    /// <code>
    /// var mongo = new MongoHelper(connectionString);
    /// mongo.Connect().Disconnect();
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
    /// Reconnects to the MongoDB database by disconnecting and then connecting again.
    /// </summary>
    /// <returns>The current <see cref="MongoHelper"/> instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the object has been disposed.</exception>
    /// <exception cref="Exception">Rethrows any exception that occurs during the reconnection process.</exception>
    /// <remarks>
    /// This method is useful for resetting the connection state.
    /// It first calls <see cref="Disconnect"/> and then <see cref="Connect"/>.
    ///
    /// Example:
    /// <code>
    /// var mongo = new MongoHelper(connectionString);
    /// mongo.Connect().Reconnect();
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
