using IDC.Utilities.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IDC.Utilities.Data;

public sealed partial class MongoHelper
{
    public IMongoCollection<TDocument> GetCollection<TDocument>(string collectionName)
    {
        ArgumentException.ThrowIfNullOrEmpty(collectionName);
        ObjectDisposedException.ThrowIf(_disposed, this);

        return _database.GetCollection<TDocument>(collectionName);
    }

    public MongoHelper Find(
        IMongoCollection<BsonDocument> collection,
        JObject filter,
        out IFindFluent<BsonDocument, BsonDocument> results,
        FindOptions? options = null
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= _client.StartSession();

            // Optimize filter parsing
            var bsonFilter = BsonDocument.Parse(filter.ToString());

            // Use batch processing and async enumeration for better performance
            var findOptions =
                options
                ?? new FindOptions
                {
                    BatchSize = 1000, // Adjust batch size based on your needs
                    AllowDiskUse = true // For large result sets
                };

            // Stream results instead of loading all into memory at once
            results = collection.Find(_session, bsonFilter, findOptions);

            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public async Task<(MongoHelper helper, IAsyncCursor<BsonDocument>? results)> FindAsync(
        string collectionName,
        JObject filter,
        Action<IAsyncCursor<BsonDocument>?>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);

            var results = await _database
                .GetCollection<BsonDocument>(collectionName)
                .FindAsync(
                    session: _session,
                    filter: BsonDocument.Parse(filter.ToString()),
                    cancellationToken: cancellationToken
                );

            callback?.Invoke(results);
            return (helper: this, results);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public async Task<(MongoHelper helper, IAsyncCursor<BsonDocument>? results)> FindAsync(
        IMongoCollection<BsonDocument> collection,
        JObject filter,
        Action<IAsyncCursor<BsonDocument>?>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);

            var results = await collection.FindAsync(
                session: _session,
                filter: BsonDocument.Parse(filter.ToString()),
                cancellationToken: cancellationToken
            );

            callback?.Invoke(results);
            return (helper: this, results);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public MongoHelper FindPaths(
        string collectionName,
        JObject filter,
        string[] paths,
        out List<JObject> results
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= _client.StartSession();

            var projection = new JObject();
            foreach (var path in paths)
            {
                projection[path] = 1;
            }

            var docs = _database
                .GetCollection<BsonDocument>(collectionName)
                .Find(_session, BsonDocument.Parse(filter.ToString()))
                .Project(BsonDocument.Parse(projection.ToString()))
                .ToList();

            results = docs.Select(doc => JObject.Parse(doc.ToJson())).ToList();
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public MongoHelper FindPaths(
        IMongoCollection<BsonDocument> collection,
        JObject filter,
        string[] paths,
        out List<JObject> results
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= _client.StartSession();
            var projection = new JObject();
            foreach (var path in paths)
            {
                projection[path] = 1;
            }

            var docs = collection
                .Find(_session, BsonDocument.Parse(filter.ToString()))
                .Project(BsonDocument.Parse(projection.ToString()))
                .ToList();

            results = docs.Select(doc => JObject.Parse(doc.ToJson())).ToList();
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public async Task<(MongoHelper helper, List<JObject> results)> FindPathsAsync(
        string collectionName,
        JObject filter,
        string[] paths,
        Action<List<JObject>>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);

            var projection = new JObject();
            foreach (var path in paths)
            {
                projection[path] = 1;
            }

            var docs = await _database
                .GetCollection<BsonDocument>(collectionName)
                .Find(_session, BsonDocument.Parse(filter.ToString()))
                .Project(BsonDocument.Parse(projection.ToString()))
                .ToListAsync(cancellationToken);

            var results = docs.Select(doc => JObject.Parse(doc.ToJson())).ToList();
            callback?.Invoke(results);
            return (helper: this, results);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public async Task<(MongoHelper helper, List<JObject> results)> FindPathsAsync(
        IMongoCollection<BsonDocument> collection,
        JObject filter,
        string[] paths,
        Action<List<JObject>>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);

            var projection = new JObject();
            foreach (var path in paths)
            {
                projection[path] = 1;
            }

            var docs = await collection
                .Find(_session, BsonDocument.Parse(filter.ToString()))
                .Project(BsonDocument.Parse(projection.ToString()))
                .ToListAsync(cancellationToken);

            var results = docs.Select(doc => JObject.Parse(doc.ToJson())).ToList();
            callback?.Invoke(results);
            return (helper: this, results);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public async Task<(MongoHelper helper, long modifiedCount)> CountAsync(
        string collectionName,
        JObject filter,
        Action<long>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);

            var count = await _database
                .GetCollection<BsonDocument>(collectionName)
                .CountDocumentsAsync(
                    session: _session,
                    filter: BsonDocument.Parse(filter.ToString()),
                    options: null,
                    cancellationToken: cancellationToken
                );

            callback?.Invoke(count);
            return (helper: this, modifiedCount: count);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public async Task<(MongoHelper helper, long modifiedCount)> CountAsync(
        IMongoCollection<BsonDocument> collection,
        JObject filter,
        Action<long>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);

            var count = await collection.CountDocumentsAsync(
                session: _session,
                filter: BsonDocument.Parse(filter.ToString()),
                options: null,
                cancellationToken: cancellationToken
            );

            callback?.Invoke(count);
            return (helper: this, modifiedCount: count);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public MongoHelper InsertOne(string collection, JObject document, out string insertedId)
    {
        ArgumentNullException.ThrowIfNull(argument: collection);
        ArgumentNullException.ThrowIfNull(argument: document);
        ObjectDisposedException.ThrowIf(condition: _disposed, instance: this);

        try
        {
            _session ??= _client.StartSession();
            var bsonDoc = BsonDocument.Parse(json: document.ToString());

            _database
                .GetCollection<BsonDocument>(name: collection)
                .InsertOne(
                    session: _session,
                    document: bsonDoc,
                    options: new InsertOneOptions { BypassDocumentValidation = true }
                );

            insertedId = bsonDoc["_id"]?.ToString() ?? string.Empty;
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public MongoHelper InsertOne(
        IMongoCollection<BsonDocument> collection,
        JObject document,
        out string insertedId
    )
    {
        ArgumentNullException.ThrowIfNull(argument: collection);
        ArgumentNullException.ThrowIfNull(argument: document);
        ObjectDisposedException.ThrowIf(condition: _disposed, instance: this);

        try
        {
            _session ??= _client.StartSession();

            var bsonDoc = BsonDocument.Parse(json: document.ToString(formatting: Formatting.None));

            collection.InsertOne(
                session: _session,
                document: bsonDoc,
                options: new InsertOneOptions { BypassDocumentValidation = true }
            );

            insertedId = bsonDoc["_id"]?.ToString() ?? string.Empty;
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public MongoHelper UpdateOne(
        string collection,
        JObject filter,
        JObject update,
        out long modifiedCount
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= _client.StartSession();

            var existingDoc = _database
                .GetCollection<BsonDocument>(collection)
                .Find(BsonDocument.Parse(filter.ToString()))
                .FirstOrDefault();

            if (existingDoc != null)
            {
                var existingJson = JObject.Parse(existingDoc.ToJson());
                existingJson = existingJson.PropMerge(
                    other: update,
                    mergeArrays: true,
                    deepMerge: true
                );

                existingJson.Remove("_id");

                var result = _database
                    .GetCollection<BsonDocument>(collection)
                    .ReplaceOne(
                        session: _session,
                        filter: BsonDocument.Parse(filter.ToString()),
                        replacement: BsonDocument.Parse(existingJson.ToString()),
                        options: new ReplaceOptions { IsUpsert = true }
                    );

                modifiedCount = result.ModifiedCount;
                return this;
            }
            else
            {
                var result = _database
                    .GetCollection<BsonDocument>(collection)
                    .ReplaceOne(
                        session: _session,
                        filter: BsonDocument.Parse(filter.ToString()),
                        replacement: BsonDocument.Parse(update.ToString()),
                        options: new ReplaceOptions { IsUpsert = true }
                    );

                modifiedCount = result.ModifiedCount;
                return this;
            }
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public MongoHelper DeleteOne(string collection, JObject filter, out long deletedCount)
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= _client.StartSession();
            var result = _database
                .GetCollection<BsonDocument>(collection)
                .DeleteOne(session: _session, filter: BsonDocument.Parse(filter.ToString()));
            deletedCount = result.DeletedCount;
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public MongoHelper FindOne(string collection, JObject filter, out JObject? result)
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= _client.StartSession();
            var doc = _database
                .GetCollection<BsonDocument>(collection)
                .Find(_session, BsonDocument.Parse(filter.ToString()))
                .FirstOrDefault();

            result = doc is not null ? JObject.Parse(doc.ToJson()) : null;
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public int GetIncrementId(string collection)
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= _client.StartSession();

            var maxIdDoc = _database
                .GetCollection<BsonDocument>(collection)
                .Find(_session, new BsonDocument())
                .Sort(Builders<BsonDocument>.Sort.Descending("_id"))
                .Limit(1)
                .FirstOrDefault();

            if (maxIdDoc != null && maxIdDoc["_id"] != null)
            {
                var currentId = maxIdDoc["_id"].ToString();
                if (int.TryParse(currentId, out int parsedId))
                {
                    return parsedId + 1;
                }
            }

            return 1;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public MongoHelper ArrayPush(
        string collection,
        JObject filter,
        string arrayPath,
        JToken value,
        out long modifiedCount
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= _client.StartSession();
            var update = new JObject { ["$push"] = new JObject { [arrayPath] = value } };

            var result = _database
                .GetCollection<BsonDocument>(collection)
                .UpdateOne(
                    session: _session,
                    filter: BsonDocument.Parse(filter.ToString()),
                    update: BsonDocument.Parse(update.ToString())
                );
            modifiedCount = result.ModifiedCount;
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public MongoHelper ArraySet(
        string collection,
        JObject filter,
        string arrayPath,
        int index,
        JToken value,
        out long modifiedCount
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= _client.StartSession();
            var update = new JObject
            {
                ["$set"] = new JObject { [$"{arrayPath}.{index}"] = value }
            };

            var result = _database
                .GetCollection<BsonDocument>(collection)
                .UpdateOne(
                    session: _session,
                    filter: BsonDocument.Parse(filter.ToString()),
                    update: BsonDocument.Parse(update.ToString())
                );
            modifiedCount = result.ModifiedCount;
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public MongoHelper ArrayUpdate(
        string collection,
        JObject filter,
        string arrayPath,
        JObject arrayFilter,
        JToken value,
        out long modifiedCount
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            var update = new JObject
            {
                ["$set"] = new JObject { [arrayPath + ".$[elem]"] = value }
            };

            var arrayFilters = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    BsonDocument.Parse(arrayFilter.ToString())
                )
            };

            var result = _database
                .GetCollection<BsonDocument>(collection)
                .UpdateOne(
                    session: _session,
                    filter: BsonDocument.Parse(filter.ToString()),
                    update: BsonDocument.Parse(update.ToString()),
                    options: new UpdateOptions { ArrayFilters = arrayFilters }
                );
            modifiedCount = result.ModifiedCount;
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public MongoHelper ArrayPull(
        string collection,
        JObject filter,
        string arrayPath,
        JObject condition,
        out long modifiedCount
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            var update = new JObject { ["$pull"] = new JObject { [arrayPath] = condition } };

            var result = _database
                .GetCollection<BsonDocument>(collection)
                .UpdateOne(
                    session: _session,
                    filter: BsonDocument.Parse(filter.ToString()),
                    update: BsonDocument.Parse(update.ToString())
                );
            modifiedCount = result.ModifiedCount;
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public MongoHelper ArrayRemoveAt(
        string collection,
        JObject filter,
        string arrayPath,
        int index,
        out long modifiedCount
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            var update = new JObject { ["$unset"] = new JObject { [$"{arrayPath}.{index}"] = 1 } };

            var pullUpdate = new JObject { ["$pull"] = new JObject { [arrayPath] = null } };

            var result1 = _database
                .GetCollection<BsonDocument>(collection)
                .UpdateOne(
                    session: _session,
                    filter: BsonDocument.Parse(filter.ToString()),
                    update: BsonDocument.Parse(update.ToString())
                );

            var result2 = _database
                .GetCollection<BsonDocument>(collection)
                .UpdateOne(
                    session: _session,
                    filter: BsonDocument.Parse(filter.ToString()),
                    update: BsonDocument.Parse(pullUpdate.ToString())
                );

            modifiedCount = result1.ModifiedCount + result2.ModifiedCount;
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public MongoHelper ArrayReplaceAll(
        string collection,
        JObject filter,
        string arrayPath,
        JToken value,
        out long modifiedCount
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            var existingDoc = _database
                .GetCollection<BsonDocument>(collection)
                .Find(filter: BsonDocument.Parse(filter.ToString()))
                .FirstOrDefault();

            if (existingDoc == null)
            {
                modifiedCount = 0;
                return this;
            }

            var existingJson = JObject.Parse(existingDoc.ToJson());
            var targetField = existingJson.SelectToken(arrayPath);

            if (targetField == null || targetField.Type != JTokenType.Array)
            {
                throw new InvalidOperationException($"Field '{arrayPath}' is not an array");
            }

            var update = new JObject { ["$set"] = new JObject { [arrayPath] = value } };

            var result = _database
                .GetCollection<BsonDocument>(collection)
                .UpdateOne(
                    session: _session,
                    filter: BsonDocument.Parse(filter.ToString()),
                    update: BsonDocument.Parse(update.ToString())
                );

            modifiedCount = result.ModifiedCount;
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public async Task<(MongoHelper helper, string insertedId)> InsertOneAsync(
        string collection,
        JObject document,
        Action<string>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            var bsonDoc = BsonDocument.Parse(document.ToString());
            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);

            await _database
                .GetCollection<BsonDocument>(collection)
                .InsertOneAsync(_session, bsonDoc, cancellationToken: cancellationToken);

            var id = bsonDoc["_id"]?.ToString() ?? string.Empty;
            callback?.Invoke(id);
            return (helper: this, insertedId: id);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public async Task<(MongoHelper helper, long modifiedCount)> UpdateOneAsync(
        string collection,
        JObject filter,
        JObject update,
        Action<long>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);

            var existingDoc = await _database
                .GetCollection<BsonDocument>(collection)
                .Find(BsonDocument.Parse(filter.ToString()))
                .FirstOrDefaultAsync(cancellationToken);

            if (existingDoc != null)
            {
                var existingJson = JObject.Parse(existingDoc.ToJson());
                existingJson = existingJson.PropMerge(
                    other: update,
                    mergeArrays: false,
                    deepMerge: true
                );

                existingJson.Remove("_id");

                var result = await _database
                    .GetCollection<BsonDocument>(collection)
                    .ReplaceOneAsync(
                        session: _session,
                        filter: BsonDocument.Parse(filter.ToString()),
                        replacement: BsonDocument.Parse(existingJson.ToString()),
                        options: new ReplaceOptions { IsUpsert = true },
                        cancellationToken: cancellationToken
                    );

                callback?.Invoke(result.ModifiedCount);
                return (helper: this, modifiedCount: result.ModifiedCount);
            }
            else
            {
                var result = await _database
                    .GetCollection<BsonDocument>(collection)
                    .ReplaceOneAsync(
                        session: _session,
                        filter: BsonDocument.Parse(filter.ToString()),
                        replacement: BsonDocument.Parse(update.ToString()),
                        options: new ReplaceOptions { IsUpsert = true },
                        cancellationToken: cancellationToken
                    );

                callback?.Invoke(result.ModifiedCount);
                return (helper: this, modifiedCount: result.ModifiedCount);
            }
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public async Task<(MongoHelper helper, long deletedCount)> DeleteOneAsync(
        string collection,
        JObject filter,
        Action<long>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);

            var result = await _database
                .GetCollection<BsonDocument>(collection)
                .DeleteOneAsync(
                    session: _session,
                    filter: BsonDocument.Parse(filter.ToString()),
                    cancellationToken: cancellationToken
                );
            callback?.Invoke(result.DeletedCount);
            return (helper: this, deletedCount: result.DeletedCount);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public async Task<(MongoHelper helper, JObject? result)> FindOneAsync(
        string collection,
        JObject filter,
        Action<JObject?>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var startTime = DateTime.UtcNow;
            Console.WriteLine($"FindOneAsync started at: {startTime}");

            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);

            var doc = await _database
                .GetCollection<BsonDocument>(collection)
                .Find(session: _session, filter: BsonDocument.Parse(filter.ToString()))
                .Limit(1)
                .FirstOrDefaultAsync(cancellationToken);

            var result = doc is not null ? JObject.Parse(doc.ToJson()) : null;
            callback?.Invoke(result);

            var endTime = DateTime.UtcNow;
            Console.WriteLine($"FindOneAsync completed at: {endTime}");
            Console.WriteLine(
                $"FindOneAsync duration: {(endTime - startTime).TotalMilliseconds} ms"
            );

            return (helper: this, result);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public async Task<(MongoHelper helper, int id)> GetIncrementIdAsync(
        string collection,
        Action<int>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);

            var maxIdDoc = await _database
                .GetCollection<BsonDocument>(collection)
                .Find(_session, new BsonDocument())
                .Sort(Builders<BsonDocument>.Sort.Descending("_id"))
                .Limit(1)
                .FirstOrDefaultAsync(cancellationToken);

            var nextId = 1;
            if (maxIdDoc != null && maxIdDoc["_id"] != null)
            {
                var currentId = maxIdDoc["_id"].ToString();
                if (int.TryParse(currentId, out int parsedId))
                {
                    nextId = parsedId + 1;
                }
            }

            callback?.Invoke(nextId);
            return (helper: this, id: nextId);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public async Task<(MongoHelper helper, long modifiedCount)> ArrayPushAsync(
        string collection,
        JObject filter,
        string arrayPath,
        JToken value,
        Action<long>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);

            var update = new JObject { ["$push"] = new JObject { [arrayPath] = value } };

            var result = await _database
                .GetCollection<BsonDocument>(collection)
                .UpdateOneAsync(
                    _session,
                    BsonDocument.Parse(filter.ToString()),
                    BsonDocument.Parse(update.ToString()),
                    cancellationToken: cancellationToken
                );

            callback?.Invoke(result.ModifiedCount);
            return (helper: this, modifiedCount: result.ModifiedCount);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public async Task<(MongoHelper helper, long modifiedCount)> ArraySetAsync(
        string collection,
        JObject filter,
        string arrayPath,
        int index,
        JToken value,
        Action<long>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);
            var update = new JObject
            {
                ["$set"] = new JObject { [$"{arrayPath}.{index}"] = value }
            };

            var result = await _database
                .GetCollection<BsonDocument>(collection)
                .UpdateOneAsync(
                    session: _session,
                    filter: BsonDocument.Parse(filter.ToString()),
                    update: BsonDocument.Parse(update.ToString()),
                    cancellationToken: cancellationToken
                );

            callback?.Invoke(result.ModifiedCount);
            return (helper: this, modifiedCount: result.ModifiedCount);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public async Task<(MongoHelper helper, long modifiedCount)> ArrayUpdateAsync(
        string collection,
        JObject filter,
        string arrayPath,
        JObject arrayFilter,
        JToken value,
        Action<long>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);
            var update = new JObject
            {
                ["$set"] = new JObject { [arrayPath + ".$[elem]"] = value }
            };

            var arrayFilters = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    BsonDocument.Parse(arrayFilter.ToString())
                )
            };

            var result = await _database
                .GetCollection<BsonDocument>(collection)
                .UpdateOneAsync(
                    session: _session,
                    filter: BsonDocument.Parse(filter.ToString()),
                    update: BsonDocument.Parse(update.ToString()),
                    options: new UpdateOptions { ArrayFilters = arrayFilters },
                    cancellationToken: cancellationToken
                );

            callback?.Invoke(result.ModifiedCount);
            return (helper: this, modifiedCount: result.ModifiedCount);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public async Task<(MongoHelper helper, long modifiedCount)> ArrayPullAsync(
        string collection,
        JObject filter,
        string arrayPath,
        JObject condition,
        Action<long>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            var update = new JObject { ["$pull"] = new JObject { [arrayPath] = condition } };

            var result = await _database
                .GetCollection<BsonDocument>(collection)
                .UpdateOneAsync(
                    session: _session,
                    filter: BsonDocument.Parse(filter.ToString()),
                    update: BsonDocument.Parse(update.ToString()),
                    cancellationToken: cancellationToken
                );

            callback?.Invoke(result.ModifiedCount);
            return (helper: this, modifiedCount: result.ModifiedCount);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public async Task<(MongoHelper helper, long modifiedCount)> ArrayRemoveAtAsync(
        string collection,
        JObject filter,
        string arrayPath,
        int index,
        Action<long>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            var update = new JObject { ["$unset"] = new JObject { [$"{arrayPath}.{index}"] = 1 } };
            var pullUpdate = new JObject { ["$pull"] = new JObject { [arrayPath] = null } };

            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);

            var result1 = await _database
                .GetCollection<BsonDocument>(collection)
                .UpdateOneAsync(
                    session: _session,
                    filter: BsonDocument.Parse(filter.ToString()),
                    update: BsonDocument.Parse(update.ToString()),
                    cancellationToken: cancellationToken
                );

            var result2 = await _database
                .GetCollection<BsonDocument>(collection)
                .UpdateOneAsync(
                    session: _session,
                    filter: BsonDocument.Parse(filter.ToString()),
                    update: BsonDocument.Parse(pullUpdate.ToString()),
                    cancellationToken: cancellationToken
                );

            var totalModified = result1.ModifiedCount + result2.ModifiedCount;
            callback?.Invoke(totalModified);
            return (helper: this, modifiedCount: totalModified);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public async Task<(MongoHelper helper, long modifiedCount)> ArrayReplaceAllAsync(
        string collection,
        JObject filter,
        string arrayPath,
        JToken value,
        Action<long>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);

            var existingDoc = await _database
                .GetCollection<BsonDocument>(collection)
                .Find(filter: BsonDocument.Parse(filter.ToString()))
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (existingDoc == null)
            {
                callback?.Invoke(0);
                return (helper: this, modifiedCount: 0);
            }

            var existingJson = JObject.Parse(existingDoc.ToJson());
            var targetField = existingJson.SelectToken(arrayPath);

            if (targetField == null || targetField.Type != JTokenType.Array)
            {
                throw new InvalidOperationException($"Field '{arrayPath}' is not an array");
            }

            var update = new JObject { ["$set"] = new JObject { [arrayPath] = value } };

            var result = await _database
                .GetCollection<BsonDocument>(collection)
                .UpdateOneAsync(
                    session: _session,
                    filter: BsonDocument.Parse(filter.ToString()),
                    update: BsonDocument.Parse(update.ToString()),
                    cancellationToken: cancellationToken
                );

            callback?.Invoke(result.ModifiedCount);
            return (helper: this, modifiedCount: result.ModifiedCount);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public (MongoHelper helper, long modifiedCount) RemoveFields(
        string collection,
        JObject filter,
        string[] fields,
        Action<long>? callback = null
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= _client.StartSession();

            var unsetFields = new JObject();
            foreach (var field in fields)
            {
                unsetFields[field] = 1;
            }

            var update = new JObject { ["$unset"] = unsetFields };

            var result = _database
                .GetCollection<BsonDocument>(collection)
                .UpdateOne(
                    session: _session,
                    filter: BsonDocument.Parse(filter.ToString()),
                    update: BsonDocument.Parse(update.ToString())
                );

            callback?.Invoke(result.ModifiedCount);
            return (helper: this, modifiedCount: result.ModifiedCount);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    public async Task<(MongoHelper helper, long modifiedCount)> RemoveFieldsAsync(
        string collection,
        JObject filter,
        string[] fields,
        Action<long>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);

            var unsetFields = new JObject();
            foreach (var field in fields)
            {
                unsetFields[field] = 1;
            }

            var update = new JObject { ["$unset"] = unsetFields };

            var result = await _database
                .GetCollection<BsonDocument>(collection)
                .UpdateOneAsync(
                    session: _session,
                    filter: BsonDocument.Parse(filter.ToString()),
                    update: BsonDocument.Parse(update.ToString()),
                    cancellationToken: cancellationToken
                );

            callback?.Invoke(result.ModifiedCount);
            return (helper: this, modifiedCount: result.ModifiedCount);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }
}
