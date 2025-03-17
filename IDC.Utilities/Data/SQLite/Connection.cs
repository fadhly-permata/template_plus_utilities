namespace IDC.Utilities.Data;

public sealed partial class SQLiteHelper
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
    public SQLiteHelper Connect(bool useTransaction = false)
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
    public SQLiteHelper Reconnect(bool useTransaction = false)
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
    public SQLiteHelper Disconnect(bool commitTransaction = false)
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
                CleanupConnection();

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }
}
