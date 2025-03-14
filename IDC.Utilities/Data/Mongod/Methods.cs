using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace IDC.Utilities.Data;

public sealed partial class MongoHelper
{
    /// <summary>
    /// Checks if the MongoDB cluster is in a connected state.
    /// </summary>
    /// <returns>True if the cluster state is connected, false otherwise.</returns>
    /// <remarks>
    /// This method checks the internal state of the MongoDB cluster connection.
    ///
    /// Example:
    /// <code>
    /// var mongo = new MongoHelper("mongodb://localhost:27017");
    /// bool isConnected = mongo.IsConnected();
    /// Console.WriteLine($"Connection status: {isConnected}");
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when the MongoHelper instance has been disposed.</exception>
    /// <seealso cref="MongoDB.Driver.Core.Clusters.ClusterState"/>
    public bool IsConnected()
    {
        ThrowIfDisposed();
        return _client.Cluster.Description.State
            == MongoDB.Driver.Core.Clusters.ClusterState.Connected;
    }

    /// <summary>
    /// Checks if the MongoDB database is accessible by executing a ping command.
    /// </summary>
    /// <param name="message">Output parameter containing success or error message.</param>
    /// <returns>True if ping successful, false otherwise.</returns>
    /// <remarks>
    /// This method attempts to ping the database and returns detailed status information.
    ///
    /// Example:
    /// <code>
    /// var mongo = new MongoHelper("mongodb://localhost:27017");
    /// if (mongo.IsConnected(out string message))
    /// {
    ///     Console.WriteLine($"Connected: {message}");
    /// }
    /// else
    /// {
    ///     Console.WriteLine($"Connection failed: {message}");
    /// }
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when the MongoHelper instance has been disposed.</exception>
    /// <seealso cref="MongoDB.Driver.IMongoDatabase.RunCommand{TResult}"/>
    public bool IsConnected(out string message)
    {
        try
        {
            ThrowIfDisposed();
            _ = _database.RunCommand<BsonDocument>(
                command: new BsonDocument(name: "ping", value: 1)
            );
            message = _messages.MSG_CONNECTION_ESTABLISHED;
            return true;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return false;
        }
    }

    /// <summary>
    /// Asynchronously checks if the MongoDB database is accessible by executing a ping command.
    /// </summary>
    /// <param name="callback">Optional callback to receive success or error message.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, returning true if ping successful.</returns>
    /// <remarks>
    /// This method provides an asynchronous way to check database connectivity with optional callback support.
    ///
    /// Example:
    /// <code>
    /// var mongo = new MongoHelper("mongodb://localhost:27017");
    /// var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
    ///
    /// bool isConnected = await mongo.IsConnectedAsync(
    ///     callback: message => Console.WriteLine(message),
    ///     cancellationToken: cts.Token
    /// );
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when the MongoHelper instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled through <paramref name="cancellationToken"/>.</exception>
    /// <seealso cref="MongoDB.Driver.IMongoDatabase.RunCommandAsync{TResult}"/>
    public async Task<bool> IsConnectedAsync(
        Action<string>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ThrowIfDisposed();
            _ = await _database.RunCommandAsync<BsonDocument>(
                command: new BsonDocument(name: "ping", value: 1),
                cancellationToken: cancellationToken
            );

            callback?.Invoke(obj: _messages.MSG_CONNECTION_ESTABLISHED);
            return true;
        }
        catch (Exception ex)
        {
            callback?.Invoke(obj: ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Creates a MongoDB session connection.
    /// </summary>
    /// <param name="disconnectFirst">If true, disconnects existing session before creating new one.</param>
    /// <returns>The current MongoHelper instance for method chaining.</returns>
    /// <remarks>
    /// Internal helper method that manages MongoDB session creation.
    ///
    /// Example:
    /// <code>
    /// var mongo = new MongoHelper("mongodb://localhost:27017");
    /// mongo.CreateConnection(disconnectFirst: true);
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when the MongoHelper instance has been disposed.</exception>
    /// <seealso cref="MongoDB.Driver.IMongoClient.StartSession"/>
    /// <seealso cref="Disconnect"/>
    private MongoHelper CreateConnection(bool disconnectFirst = false)
    {
        try
        {
            ThrowIfDisposed();

            if (disconnectFirst)
                Disconnect();

            _session ??= _client.StartSession();

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Creates an asynchronous MongoDB session connection.
    /// </summary>
    /// <param name="disconnectFirst">If true, disconnects existing session before creating new one.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, returning the MongoHelper instance.</returns>
    /// <remarks>
    /// Internal helper method that manages MongoDB session creation asynchronously.
    ///
    /// Example:
    /// <code>
    /// var mongo = new MongoHelper("mongodb://localhost:27017");
    /// await mongo.CreateConnectionAsync(
    ///     disconnectFirst: true,
    ///     cancellationToken: CancellationToken.None
    /// );
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when the MongoHelper instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled through <paramref name="cancellationToken"/>.</exception>
    /// <seealso cref="MongoDB.Driver.IMongoClient.StartSessionAsync"/>
    /// <seealso cref="DisconnectAsync"/>
    private async Task<MongoHelper> CreateConnectionAsync(
        bool disconnectFirst = false,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ThrowIfDisposed();

            if (disconnectFirst)
                await DisconnectAsync(cancellationToken: cancellationToken);

            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Gets the current MongoDB database instance.
    /// </summary>
    /// <returns>The <see cref="IMongoDatabase"/> instance.</returns>
    /// <remarks>
    /// Provides direct access to the underlying MongoDB database instance for advanced operations.
    ///
    /// Example:
    /// <code>
    /// var mongo = new MongoHelper("mongodb://localhost:27017/mydb");
    /// IMongoDatabase db = mongo.GetDatabase();
    /// var collection = db.GetCollection&lt;BsonDocument&gt;("mycollection");
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when the MongoHelper instance has been disposed.</exception>
    /// <seealso cref="MongoDB.Driver.IMongoDatabase"/>
    public IMongoDatabase GetDatabase()
    {
        ThrowIfDisposed();
        return _database;
    }

    /// <summary>
    /// Executes a query on the specified collection and returns results as JSON objects.
    /// </summary>
    /// <typeparam name="T">The type of documents in the collection.</typeparam>
    /// <param name="collection">The name of the collection to query.</param>
    /// <param name="filter">The filter definition for the query.</param>
    /// <param name="results">Output parameter containing the query results as JSON objects.</param>
    /// <returns>The current MongoHelper instance for method chaining.</returns>
    /// <remarks>
    /// Executes a MongoDB query and converts results to JSON format for easy manipulation.
    ///
    /// Example:
    /// <code>
    /// var mongo = new MongoHelper("mongodb://localhost:27017/mydb");
    /// var builder = Builders&lt;BsonDocument&gt;.Filter;
    /// var filter = builder.Eq("status", "active");
    ///
    /// mongo.ExecuteQuery("users", filter, out List&lt;JObject&gt; results);
    /// foreach (var user in results)
    /// {
    ///     Console.WriteLine($"User: {user["name"]}");
    /// }
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when the MongoHelper instance has been disposed.</exception>
    /// <exception cref="ArgumentException">Thrown when collection name is null or whitespace.</exception>
    /// <seealso cref="MongoDB.Driver.IMongoCollection{TDocument}.Find{TProjection}"/>
    /// <seealso cref="Newtonsoft.Json.Linq.JObject"/>
    public MongoHelper ExecuteQuery<T>(
        string collection,
        FilterDefinition<T> filter,
        out List<JObject> results
    )
        where T : class
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(argument: collection);

            results = [];

            foreach (
                var doc in _database
                    .GetCollection<T>(name: collection)
                    .Find(filter: filter ?? Builders<T>.Filter.Empty)
                    .ToList()
                    .Where(predicate: d => d is not null)
            )
            {
                var json = doc.ToJson();
                if (!string.IsNullOrEmpty(value: json))
                    results.Add(item: JObject.Parse(json: json));
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
    /// Asynchronously executes a query on the specified collection and returns results as JSON objects.
    /// </summary>
    /// <typeparam name="T">The type of documents in the collection.</typeparam>
    /// <param name="collection">The name of the collection to query.</param>
    /// <param name="filter">The filter definition for the query.</param>
    /// <param name="callback">Optional callback to receive the query results as JSON objects.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, returning the MongoHelper instance.</returns>
    /// <remarks>
    /// Provides an asynchronous way to execute MongoDB queries with optional callback support.
    ///
    /// Example:
    /// <code>
    /// var mongo = new MongoHelper("mongodb://localhost:27017/mydb");
    /// var builder = Builders&lt;BsonDocument&gt;.Filter;
    /// var filter = builder.Gt("age", 18);
    ///
    /// await mongo.ExecuteQueryAsync(
    ///     collection: "users",
    ///     filter: filter,
    ///     callback: results => {
    ///         foreach (var user in results)
    ///         {
    ///             Console.WriteLine($"Adult user: {user["name"]}");
    ///         }
    ///     },
    ///     cancellationToken: CancellationToken.None
    /// );
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when the MongoHelper instance has been disposed.</exception>
    /// <exception cref="ArgumentException">Thrown when collection name is null or whitespace.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled through <paramref name="cancellationToken"/>.</exception>
    /// <seealso cref="MongoDB.Driver.IMongoCollection{TDocument}.FindAsync{TProjection}"/>
    /// <seealso cref="Newtonsoft.Json.Linq.JObject"/>
    public async Task<MongoHelper> ExecuteQueryAsync<T>(
        string collection,
        FilterDefinition<T>? filter,
        Action<List<JObject>>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(argument: collection);

            var results = new List<JObject>();

            foreach (
                var doc in (
                    await _database
                        .GetCollection<T>(name: collection)
                        .Find(filter: filter ?? Builders<T>.Filter.Empty)
                        .ToListAsync(cancellationToken: cancellationToken)
                ).Where(predicate: d => d is not null)
            )
                results.Add(item: JObject.Parse(json: doc.ToJson()));

            callback?.Invoke(obj: results);
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Executes an upsert operation on the specified collection.
    /// </summary>
    /// <typeparam name="T">The type of documents in the collection.</typeparam>
    /// <param name="collection">The name of the collection.</param>
    /// <param name="filter">The filter to identify the document to update.</param>
    /// <param name="update">The update definition.</param>
    /// <param name="result">Output parameter containing the upserted document as JSON object.</param>
    /// <returns>The current MongoHelper instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the MongoHelper instance has been disposed.</exception>
    /// <exception cref="ArgumentException">Thrown when collection name is null or whitespace.</exception>
    public MongoHelper ExecuteUpsert<T>(
        string collection,
        FilterDefinition<T> filter,
        UpdateDefinition<T> update,
        out JObject result
    )
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(argument: collection);

            result = JObject.Parse(
                json: _database
                    .GetCollection<T>(name: collection)
                    .FindOneAndUpdate(
                        filter: filter,
                        update: update,
                        options: new FindOneAndUpdateOptions<T>
                        {
                            ReturnDocument = ReturnDocument.After,
                            IsUpsert = true
                        }
                    )
                    .ToJson()
            );

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Asynchronously executes an upsert operation on the specified collection.
    /// </summary>
    /// <typeparam name="T">The type of documents in the collection.</typeparam>
    /// <param name="collection">The name of the collection.</param>
    /// <param name="filter">The filter to identify the document to update.</param>
    /// <param name="update">The update definition.</param>
    /// <param name="callback">Optional callback to receive the upserted document as JSON object.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The current MongoHelper instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the MongoHelper instance has been disposed.</exception>
    /// <exception cref="ArgumentException">Thrown when collection name is null or whitespace.</exception>
    public async Task<MongoHelper> ExecuteUpsertAsync<T>(
        string collection,
        FilterDefinition<T> filter,
        UpdateDefinition<T> update,
        Action<JObject>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(argument: collection);

            var result = JObject.Parse(
                json: (
                    await _database
                        .GetCollection<T>(name: collection)
                        .FindOneAndUpdateAsync(
                            filter: filter,
                            update: update,
                            options: new FindOneAndUpdateOptions<T>
                            {
                                ReturnDocument = ReturnDocument.After,
                                IsUpsert = true
                            },
                            cancellationToken: cancellationToken
                        )
                ).ToJson()
            );
            callback?.Invoke(obj: result);

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Executes an update operation on the specified collection.
    /// </summary>
    /// <typeparam name="T">The type of documents in the collection.</typeparam>
    /// <param name="collection">The name of the collection.</param>
    /// <param name="filter">The filter to identify the document to update.</param>
    /// <param name="update">The update definition.</param>
    /// <param name="result">Output parameter containing the updated document as JSON object.</param>
    /// <returns>The current MongoHelper instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the MongoHelper instance has been disposed.</exception>
    /// <exception cref="ArgumentException">Thrown when collection name is null or whitespace.</exception>
    public MongoHelper ExecuteUpdate<T>(
        string collection,
        FilterDefinition<T> filter,
        UpdateDefinition<T> update,
        out JObject result
    )
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(argument: collection);

            var doc = _database
                .GetCollection<T>(name: collection)
                .FindOneAndUpdate(
                    filter: filter,
                    update: update,
                    options: new FindOneAndUpdateOptions<T>
                    {
                        ReturnDocument = ReturnDocument.After
                    }
                );
            result = doc is null ? [] : JObject.Parse(json: doc.ToJson());

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Asynchronously executes an update operation on the specified collection.
    /// </summary>
    /// <typeparam name="T">The type of documents in the collection.</typeparam>
    /// <param name="collection">The name of the collection.</param>
    /// <param name="filter">The filter to identify the document to update.</param>
    /// <param name="update">The update definition.</param>
    /// <param name="callback">Optional callback to receive the updated document as JSON object.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The current MongoHelper instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the MongoHelper instance has been disposed.</exception>
    /// <exception cref="ArgumentException">Thrown when collection name is null or whitespace.</exception>
    public async Task<MongoHelper> ExecuteUpdateAsync<T>(
        string collection,
        FilterDefinition<T> filter,
        UpdateDefinition<T> update,
        Action<JObject>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(argument: collection);

            var doc = await _database
                .GetCollection<T>(name: collection)
                .FindOneAndUpdateAsync(
                    filter: filter,
                    update: update,
                    options: new FindOneAndUpdateOptions<T>
                    {
                        ReturnDocument = ReturnDocument.After
                    },
                    cancellationToken: cancellationToken
                );

            var result = doc is null ? [] : JObject.Parse(json: doc.ToJson());
            callback?.Invoke(obj: result);

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Executes a delete operation on the specified collection.
    /// </summary>
    /// <typeparam name="T">The type of documents in the collection.</typeparam>
    /// <param name="collection">The name of the collection.</param>
    /// <param name="filter">The filter to identify documents to delete.</param>
    /// <param name="deletedCount">Output parameter containing the number of documents deleted.</param>
    /// <returns>The current MongoHelper instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the MongoHelper instance has been disposed.</exception>
    /// <exception cref="ArgumentException">Thrown when collection name is null or whitespace.</exception>
    public MongoHelper ExecuteDelete<T>(
        string collection,
        FilterDefinition<T> filter,
        out long deletedCount
    )
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(argument: collection);

            deletedCount = _database
                .GetCollection<T>(name: collection)
                .DeleteMany(filter)
                .DeletedCount;

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Asynchronously executes a delete operation on the specified collection.
    /// </summary>
    /// <typeparam name="T">The type of documents in the collection.</typeparam>
    /// <param name="collection">The name of the collection.</param>
    /// <param name="filter">The filter to identify documents to delete.</param>
    /// <param name="callback">Optional callback to receive the number of documents deleted.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The current MongoHelper instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the MongoHelper instance has been disposed.</exception>
    /// <exception cref="ArgumentException">Thrown when collection name is null or whitespace.</exception>
    public async Task<MongoHelper> ExecuteDeleteAsync<T>(
        string collection,
        FilterDefinition<T> filter,
        Action<long>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ThrowIfDisposed();
            ArgumentException.ThrowIfNullOrWhiteSpace(argument: collection);

            callback?.Invoke(
                obj: (
                    await _database
                        .GetCollection<T>(name: collection)
                        .DeleteManyAsync(filter: filter, cancellationToken: cancellationToken)
                ).DeletedCount
            );
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }
}
