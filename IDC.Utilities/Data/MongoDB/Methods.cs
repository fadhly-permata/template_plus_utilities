using MongoDB.Bson;
using MongoDB.Driver;

namespace IDC.Utilities.Data;

public sealed partial class MongoHelper
{
    public MongoHelper InitCollection(string collectionName)
    {
        ArgumentException.ThrowIfNullOrEmpty(collectionName);
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_collection?.CollectionNamespace.CollectionName == collectionName)
            return this;

        _collection = _database.GetCollection<BsonDocument>(collectionName);
        return this;
    }

    public MongoHelper GetCollection(
        string collectionName,
        out IMongoCollection<BsonDocument> collection
    )
    {
        ArgumentException.ThrowIfNullOrEmpty(collectionName);
        ObjectDisposedException.ThrowIf(_disposed, this);

        collection = _database.GetCollection<BsonDocument>(collectionName);
        return this;
    }

    public async Task<(MongoHelper helper, IAsyncCursor<BsonDocument>? results)> FindAsync(
        IMongoCollection<BsonDocument>? collection = null,
        BsonDocument? filter = null,
        Action<IAsyncCursor<BsonDocument>?>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(condition: _disposed, instance: this);
            ArgumentNullException.ThrowIfNull(argument: collection ?? _collection);

            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);

            var results = await (
                collection
                ?? _collection
                ?? throw new InvalidOperationException(
                    message: "No collection has been initialized or provided."
                )
            ).FindAsync(
                session: _session,
                filter: filter ?? [],
                cancellationToken: cancellationToken
            );

            callback?.Invoke(obj: results);
            return (helper: this, results);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public async Task<(MongoHelper helper, IAsyncCursor<BsonDocument>? results)> FindPathsAsync(
        IMongoCollection<BsonDocument>? collection = null,
        BsonDocument? filter = null,
        string[]? paths = null,
        Action<IAsyncCursor<BsonDocument>?>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(condition: _disposed, instance: this);

            var startTime = DateTime.UtcNow;
            _logging?.LogInformation($"FindPathsAsync started at {startTime}");

            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);

            var projection = new BsonDocument("_id", 0);
            if (paths?.Length > 0)
                foreach (var path in paths)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(path);
                    projection[path] = 1;
                }

            var results = await (
                collection
                ?? _collection
                ?? throw new InvalidOperationException(
                    message: "No collection has been initialized or provided."
                )
            ).FindAsync(
                session: _session,
                filter: filter ?? [],
                cancellationToken: cancellationToken
            );

            callback?.Invoke(obj: results);

            var endTime = DateTime.UtcNow;
            _logging?.LogInformation(
                $"FindPathsAsync completed at {endTime}. Duration: {(endTime - startTime).TotalMilliseconds}ms"
            );
            return (helper: this, results);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }
}
