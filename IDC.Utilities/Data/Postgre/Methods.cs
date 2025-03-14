using System.Data;
using Newtonsoft.Json.Linq;
using Npgsql;

namespace IDC.Utilities.Data;

public sealed partial class PostgreHelper
{
    public bool IsConnected()
    {
        ThrowIfDisposed();
        return _connection.State == ConnectionState.Open;
    }

    public bool IsConnected(out string message) => ConnectionIsValid(message: out message);

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

    public PostgreHelper ExecuteQuery(string query, out DataTable results)
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
