using System.Data;
using Newtonsoft.Json.Linq;
using Npgsql;

namespace IDC.Utilities.Data;

public sealed partial class PostgreHelper
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
    /// Opens connection if closed and optionally starts transaction.
    /// </remarks>
    /// <exception cref="Exception">Rethrows any exceptions that occur during connection.</exception>
    private PostgreHelper CreateConnection(
        bool useTransaction = false,
        bool disconnectFirst = false
    )
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
    /// Creates or reuses database connection asynchronously with optional transaction.
    /// </summary>
    /// <param name="useTransaction">Whether to start transaction.</param>
    /// <param name="disconnectFirst">Whether to disconnect before connecting.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Task containing current instance for chaining.</returns>
    /// <remarks>
    /// Internal method for asynchronous connection management.
    /// Opens connection if closed and optionally starts transaction.
    ///
    /// Example:
    /// <code>
    /// await db.CreateConnectionAsync(
    ///     useTransaction: true,
    ///     disconnectFirst: false,
    ///     cancellationToken: token
    /// );
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when accessing disposed instance.</exception>
    /// <exception cref="OperationCanceledException">Thrown when operation is canceled via token.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during connection.</exception>
    /// <seealso cref="NpgsqlConnection.OpenAsync"/>
    /// <seealso cref="NpgsqlConnection.CloseAsync"/>
    private async Task<PostgreHelper> CreateConnectionAsync(
        bool useTransaction = false,
        bool disconnectFirst = false,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ThrowIfDisposed();

            if (disconnectFirst)
                await _connection.CloseAsync();

            if (!_connection.State.HasFlag(flag: ConnectionState.Open))
                await _connection.OpenAsync(cancellationToken: cancellationToken);

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
    /// Creates command object for executing queries.
    /// </summary>
    /// <param name="query">SQL query string.</param>
    /// <param name="commandType">Type of command.</param>
    /// <returns>Configured NpgsqlCommand object.</returns>
    /// <remarks>
    /// Internal method for command creation.
    /// Validates connection and query string.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when query is null or empty.</exception>
    /// <exception cref="DataException">Thrown when connection is invalid.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during command creation.</exception>
    private NpgsqlCommand CreateCommand(string query, CommandType commandType = CommandType.Text)
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
    public PostgreHelper ExecuteNonQuery(string query, out int affectedRows)
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
    /// Executes non-query SQL command asynchronously.
    /// </summary>
    /// <param name="query">SQL command text.</param>
    /// <param name="callback">Optional callback action to process affected rows count.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Tuple containing current instance and number of affected rows.</returns>
    /// <remarks>
    /// Used for asynchronous INSERT, UPDATE, DELETE operations.
    /// Returns number of affected rows.
    ///
    /// Example:
    /// <code>
    /// var (db, rows) = await db.Connect()
    ///   .ExecuteNonQueryAsync(
    ///     query: "INSERT INTO Users (Name) VALUES ('John')",
    ///     callback: count => Console.WriteLine($"Affected rows: {count}"),
    ///     cancellationToken: token
    ///   );
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when query is null or empty.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when accessing disposed instance.</exception>
    /// <exception cref="OperationCanceledException">Thrown when operation is canceled via token.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during execution.</exception>
    /// <seealso cref="NpgsqlCommand.ExecuteNonQueryAsync"/>
    public async Task<(PostgreHelper helper, int affectedRows)> ExecuteNonQueryAsync(
        string query,
        Action<int>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(query);

            using var command = CreateCommand(query: query);
            int affectedRows = await command.ExecuteNonQueryAsync(
                cancellationToken: cancellationToken
            );

            callback?.Invoke(obj: affectedRows);

            return (helper: this, affectedRows);
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
    public PostgreHelper ExecuteScalar(string query, out object? result)
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
    /// Executes SQL query returning scalar value asynchronously.
    /// </summary>
    /// <param name="query">SQL query text.</param>
    /// <param name="callback">Optional callback action to process the scalar result.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Tuple containing current instance and scalar result value.</returns>
    /// <remarks>
    /// Used for asynchronous single value queries (COUNT, SUM, etc).
    /// Returns first column of first row.
    ///
    /// Example:
    /// <code>
    /// var (db, count) = await db.Connect()
    ///   .ExecuteScalarAsync(
    ///     query: "SELECT COUNT(*) FROM Users",
    ///     callback: result => Console.WriteLine($"Count: {result}"),
    ///     cancellationToken: token
    ///   );
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when query is null or empty.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when accessing disposed instance.</exception>
    /// <exception cref="OperationCanceledException">Thrown when operation is canceled via token.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during execution.</exception>
    /// <seealso cref="NpgsqlCommand.ExecuteScalarAsync"/>
    public async Task<(PostgreHelper helper, object? result)> ExecuteScalarAsync(
        string query,
        Action<object?>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(query);

            using var command = CreateCommand(query: query);
            var result = await command.ExecuteScalarAsync(cancellationToken: cancellationToken);

            callback?.Invoke(obj: result);

            return (helper: this, result);
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
    /// Returns NpgsqlDataReader for manual iteration.
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
    public PostgreHelper ExecuteReader(string query, out NpgsqlDataReader reader)
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
    /// Executes SQL query returning data reader asynchronously.
    /// </summary>
    /// <param name="query">SQL query text.</param>
    /// <param name="callback">Optional callback action to process the data reader.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Tuple containing current instance and data reader.</returns>
    /// <remarks>
    /// Used for asynchronous row-by-row data access.
    /// Returns NpgsqlDataReader for manual iteration.
    ///
    /// Example:
    /// <code>
    /// var (db, reader) = await db.Connect()
    ///   .ExecuteReaderAsync(
    ///     query: "SELECT * FROM Users",
    ///     callback: reader => { while(reader.Read()) Console.WriteLine(reader["Name"]); },
    ///     cancellationToken: token
    ///   );
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when query is null or empty.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when accessing disposed instance.</exception>
    /// <exception cref="OperationCanceledException">Thrown when operation is canceled via token.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during execution.</exception>
    /// <seealso cref="NpgsqlCommand.ExecuteReaderAsync"/>
    public async Task<(PostgreHelper helper, NpgsqlDataReader reader)> ExecuteReaderAsync(
        string query,
        Action<NpgsqlDataReader>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(query);

            using var command = CreateCommand(query: query);
            var reader = await command.ExecuteReaderAsync(cancellationToken: cancellationToken);

            callback?.Invoke(obj: reader);

            return (helper: this, reader);
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
    public PostgreHelper ExecuteQuery(string query, out List<JObject> results)
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
                    var name = reader.GetName(ordinal: i);
                    var value = reader.IsDBNull(ordinal: i) ? null : reader.GetValue(ordinal: i);
                    row[name] = value == null ? null : JToken.FromObject(o: value);
                }
                results.Add(item: row);
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
    /// Executes SQL query returning results as JObject list asynchronously.
    /// </summary>
    /// <param name="query">SQL query text.</param>
    /// <param name="callback">Optional callback action to process the results.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Tuple containing current instance and results as JObject list.</returns>
    /// <remarks>
    /// Used for asynchronous JSON-formatted results.
    /// Returns list of JObject for each row.
    /// Handles null values appropriately.
    ///
    /// Example:
    /// <code>
    /// var (db, users) = await db.Connect()
    ///   .ExecuteQueryAsync(
    ///     query: "SELECT * FROM Users",
    ///     callback: results => results.ForEach(user => Console.WriteLine(user["Name"])),
    ///     cancellationToken: token
    ///   );
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when query is null or empty.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when accessing disposed instance.</exception>
    /// <exception cref="OperationCanceledException">Thrown when operation is canceled via token.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during execution.</exception>
    /// <seealso cref="NpgsqlCommand.ExecuteReaderAsync"/>
    /// <seealso cref="NpgsqlDataReader.ReadAsync"/>
    /// <seealso cref="NpgsqlDataReader.IsDBNullAsync"/>
    public async Task<(PostgreHelper helper, List<JObject> results)> ExecuteQueryAsync(
        string query,
        Action<List<JObject>>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(query);

            var results = new List<JObject>();
            using var command = CreateCommand(query: query);
            using var reader = await command.ExecuteReaderAsync(
                cancellationToken: cancellationToken
            );

            while (await reader.ReadAsync(cancellationToken: cancellationToken))
            {
                var row = new JObject();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(ordinal: i);
                    var value = await reader.IsDBNullAsync(
                        ordinal: i,
                        cancellationToken: cancellationToken
                    )
                        ? null
                        : reader.GetValue(ordinal: i);
                    row[name] = value == null ? null : JToken.FromObject(o: value);
                }
                results.Add(item: row);
            }

            callback?.Invoke(obj: results);

            return (this, results);
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
    public PostgreHelper ExecuteQuery(string query, out DataTable results)
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(query);

            results = new();
            using var command = CreateCommand(query: query);
            using var reader = command.ExecuteReader();
            results.Load(reader: reader);

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Executes SQL query returning results as DataTable asynchronously.
    /// </summary>
    /// <param name="query">SQL query text.</param>
    /// <param name="callback">Optional callback action to process the results.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Tuple containing current instance and results as DataTable.</returns>
    /// <remarks>
    /// Used for asynchronous table-formatted results.
    /// Returns DataTable containing all rows.
    ///
    /// Example:
    /// <code>
    /// var (db, users) = await db.Connect()
    ///   .ExecuteQueryAsync(
    ///     query: "SELECT * FROM Users",
    ///     callback: table => table.Rows.Cast&lt;DataRow&gt;().ToList().ForEach(row => Console.WriteLine(row["Name"])),
    ///     cancellationToken: token
    ///   );
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when query is null or empty.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when accessing disposed instance.</exception>
    /// <exception cref="OperationCanceledException">Thrown when operation is canceled via token.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during execution.</exception>
    /// <seealso cref="NpgsqlCommand.ExecuteReaderAsync"/>
    /// <seealso cref="DataTable.Load"/>
    public async Task<(PostgreHelper helper, DataTable results)> ExecuteQueryAsync(
        string query,
        Action<DataTable>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(argument: query);

            var results = new DataTable();
            using var command = CreateCommand(query: query);
            using var reader = await command.ExecuteReaderAsync(
                cancellationToken: cancellationToken
            );
            results.Load(reader: reader);

            callback?.Invoke(obj: results);

            return (this, results);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }
}
