using System.Data;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;

namespace IDC.Utilities.Data;

public sealed partial class SQLiteHelper
{
    /// <summary>
    /// Checks if database connection is open.
    /// </summary>
    /// <returns>True if connected, false otherwise.</returns>
    /// <remarks>
    /// Validates connection state without additional status message.
    /// </remarks>
    /// <example>
    /// <code>
    /// if (db.IsConnected())
    ///     db.ExecuteNonQuery("INSERT INTO Users (Name) VALUES ('John')", out _);
    /// </code>
    /// </example>
    /// <exception cref="ObjectDisposedException">Thrown when accessing disposed instance.</exception>
    public bool IsConnected()
    {
        ThrowIfDisposed();
        return _connection.State == ConnectionState.Open;
    }

    /// <summary>
    /// Checks connection status and returns status message.
    /// </summary>
    /// <param name="message">Status message output.</param>
    /// <returns>True if connected, false otherwise.</returns>
    /// <remarks>
    /// Validates connection state and provides status message.
    /// </remarks>
    /// <example>
    /// <code>
    /// if (db.IsConnected(out string status))
    ///     Console.WriteLine($"Connected: {status}");
    /// </code>
    /// </example>
    /// <exception cref="Exception">Rethrows any exceptions that occur during validation.</exception>
    public bool IsConnected(out string message) => ConnectionIsValid(message: out message);

    /// <summary>
    /// Creates or reuses database connection with optional transaction.
    /// </summary>
    /// <param name="useTransaction">Whether to start transaction.</param>
    /// <param name="disconnectFirst">Whether to disconnect before connecting.</param>
    /// <returns>Current instance for chaining.</returns>
    /// <remarks>
    /// Internal method for connection management.
    /// Handles connection state and optional transaction.
    /// </remarks>
    /// <exception cref="Exception">Rethrows any exceptions that occur during connection.</exception>
    private SQLiteHelper CreateConnection(bool useTransaction = false, bool disconnectFirst = false)
    {
        try
        {
            ThrowIfDisposed();

            if (disconnectFirst)
                _connection.Close();

            if (!_connection.State.HasFlag(flag: ConnectionState.Open))
                _connection.Open();

            if (useTransaction)
                TransactionBegin();

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Creates command object for query execution.
    /// </summary>
    /// <param name="query">SQL query string.</param>
    /// <param name="commandType">Type of command.</param>
    /// <returns>Configured SqliteCommand object.</returns>
    /// <remarks>
    /// Internal method for command creation.
    /// Validates connection and query string.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when query is null or empty.</exception>
    /// <exception cref="DataException">Thrown when connection is invalid.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during command creation.</exception>
    private SqliteCommand CreateCommand(string query, CommandType commandType = CommandType.Text)
    {
        try
        {
            ThrowIfDisposed();

            if (!ConnectionIsValid(message: out var errorMessage))
                throw new DataException(s: errorMessage);

            ArgumentException.ThrowIfNullOrWhiteSpace(query);

            var command = _connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = commandType;

            return command;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Executes non-query SQL command.
    /// </summary>
    /// <param name="query">SQL command text.</param>
    /// <param name="affectedRows">Number of affected rows.</param>
    /// <returns>Current instance for chaining.</returns>
    /// <remarks>
    /// Used for INSERT, UPDATE, DELETE operations.
    /// Returns number of affected rows.
    /// </remarks>
    /// <example>
    /// <code>
    /// db.Connect()
    ///   .ExecuteNonQuery("INSERT INTO Users (Name) VALUES ('John')", out int rows)
    ///   .Disconnect();
    /// </code>
    /// </example>
    /// <exception cref="ArgumentException">Thrown when query is null or empty.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during execution.</exception>
    public SQLiteHelper ExecuteNonQuery(string query, out int affectedRows)
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(query);

            using var command = CreateCommand(query: query);
            affectedRows = command.ExecuteNonQuery();

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Executes SQL query returning scalar value.
    /// </summary>
    /// <param name="query">SQL query text.</param>
    /// <param name="result">Query result value.</param>
    /// <returns>Current instance for chaining.</returns>
    /// <remarks>
    /// Used for single value queries (COUNT, SUM, etc).
    /// Returns first column of first row.
    /// </remarks>
    /// <example>
    /// <code>
    /// db.Connect()
    ///   .ExecuteScalar("SELECT COUNT(*) FROM Users", out object count)
    ///   .Disconnect();
    /// </code>
    /// </example>
    /// <exception cref="ArgumentException">Thrown when query is null or empty.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during execution.</exception>
    public SQLiteHelper ExecuteScalar(string query, out object? result)
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(query);

            using var command = CreateCommand(query: query);
            result = command.ExecuteScalar();

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Executes SQL query returning data reader.
    /// </summary>
    /// <param name="query">SQL query text.</param>
    /// <param name="reader">Query result reader.</param>
    /// <returns>Current instance for chaining.</returns>
    /// <remarks>
    /// Used for row-by-row data access.
    /// Returns SqliteDataReader for manual iteration.
    /// </remarks>
    /// <example>
    /// <code>
    /// db.Connect()
    ///   .ExecuteReader("SELECT * FROM Users", out var reader);
    /// while (reader.Read())
    ///     Console.WriteLine(reader["Name"]);
    /// </code>
    /// </example>
    /// <exception cref="ArgumentException">Thrown when query is null or empty.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during execution.</exception>
    public SQLiteHelper ExecuteReader(string query, out SqliteDataReader reader)
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(query);

            using var command = CreateCommand(query: query);
            reader = command.ExecuteReader();

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Executes SQL query returning results as JObject list.
    /// </summary>
    /// <param name="query">SQL query text.</param>
    /// <param name="results">Query results as JObject list.</param>
    /// <returns>Current instance for chaining.</returns>
    /// <remarks>
    /// Used for JSON-formatted results.
    /// Returns list of JObject for each row.
    /// Handles null values appropriately.
    /// </remarks>
    /// <example>
    /// <code>
    /// db.Connect()
    ///   .ExecuteQuery("SELECT * FROM Users", out List&lt;JObject&gt; users)
    ///   .Disconnect();
    /// foreach (var user in users)
    ///     Console.WriteLine(user["Name"]);
    /// </code>
    /// </example>
    /// <exception cref="ArgumentException">Thrown when query is null or empty.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during execution.</exception>
    public SQLiteHelper ExecuteQuery(string query, out List<JObject> results)
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(query);

            results = [];
            using var command = CreateCommand(query: query);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var row = new JObject();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    row[name] = value == null ? null : JToken.FromObject(value);
                }
                results.Add(row);
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
    /// Executes SQL query returning results as DataTable.
    /// </summary>
    /// <param name="query">SQL query text.</param>
    /// <param name="results">Query results as DataTable.</param>
    /// <returns>Current instance for chaining.</returns>
    /// <remarks>
    /// Used for table-formatted results.
    /// Returns DataTable containing all rows.
    /// </remarks>
    /// <example>
    /// <code>
    /// db.Connect()
    ///   .ExecuteQuery("SELECT * FROM Users", out DataTable users)
    ///   .Disconnect();
    /// foreach (DataRow row in users.Rows)
    ///     Console.WriteLine(row["Name"]);
    /// </code>
    /// </example>
    /// <exception cref="ArgumentException">Thrown when query is null or empty.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during execution.</exception>
    public SQLiteHelper ExecuteQuery(string query, out DataTable results)
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(query);

            results = new();
            using var command = CreateCommand(query: query);
            using var reader = command.ExecuteReader();
            results.Load(reader);

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }
}
