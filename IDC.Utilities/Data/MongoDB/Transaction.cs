namespace IDC.Utilities.Data;

public sealed partial class MongoHelper
{
    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    /// <returns>Current <see cref="MongoHelper"/> instance for chaining.</returns>
    /// <remarks>
    /// Creates a new session and starts a transaction if none exists.
    /// Throws an exception if a transaction is already in progress.
    ///
    /// <para>Example:</para>
    /// <code>
    /// var db = new MongoHelper("mongodb://localhost:27017");
    /// db.TransactionBegin()
    ///   .InsertOne("users", new BsonDocument("name", "John Doe"))
    ///   .TransactionCommit();
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when the helper is disposed.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a transaction is already started.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during transaction start.</exception>
    /// <seealso href="https://docs.mongodb.com/manual/core/transactions/">MongoDB Transactions</seealso>
    public MongoHelper TransactionBegin()
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (_session is not null)
                throw new InvalidOperationException(_messages.MSG_TRANSACTION_STARTED);

            _session = _client.StartSession();
            _session.StartTransaction();
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Commits the current database transaction.
    /// </summary>
    /// <returns>Current <see cref="MongoHelper"/> instance for chaining.</returns>
    /// <remarks>
    /// Commits the active transaction and disposes the session.
    /// Throws an exception if no transaction is active.
    ///
    /// <para>Example:</para>
    /// <code>
    /// db.TransactionBegin()
    ///   .InsertOne("users", new BsonDocument("name", "Jane Doe"))
    ///   .TransactionCommit();
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when the helper is disposed.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no transaction is active.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during transaction commit.</exception>
    /// <seealso href="https://docs.mongodb.com/manual/core/transactions/">MongoDB Transactions</seealso>
    public MongoHelper TransactionCommit()
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (_session is null)
                throw new InvalidOperationException(_messages.MSG_TRANSACTION_NOT_STARTED);

            _session.CommitTransaction();
            _session.Dispose();
            _session = null;
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Rolls back the current database transaction.
    /// </summary>
    /// <returns>Current <see cref="MongoHelper"/> instance for chaining.</returns>
    /// <remarks>
    /// Aborts the active transaction and disposes the session.
    /// Throws an exception if no transaction is active.
    ///
    /// <para>Example:</para>
    /// <code>
    /// try
    /// {
    ///     db.TransactionBegin()
    ///       .InsertOne("users", new BsonDocument("name", "Alice"))
    ///       .InsertOne("users", new BsonDocument("name", "Bob"));
    ///     // Some condition that requires rollback
    ///     db.TransactionRollback();
    /// }
    /// catch (Exception ex)
    /// {
    ///     Console.WriteLine($"Error occurred: {ex.Message}");
    /// }
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when the helper is disposed.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no transaction is active.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during transaction rollback.</exception>
    /// <seealso href="https://docs.mongodb.com/manual/core/transactions/">MongoDB Transactions</seealso>
    public MongoHelper TransactionRollback()
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (_session is null)
                throw new InvalidOperationException(_messages.MSG_TRANSACTION_NOT_STARTED);

            _session.AbortTransaction();
            _session.Dispose();
            _session = null;
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }
}
