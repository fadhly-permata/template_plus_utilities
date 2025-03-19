using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace IDC.Utilities.Data;

public sealed partial class MongoHelper
{
    /// <summary>
    /// Inserts a single document into the specified collection.
    /// </summary>
    /// <param name="collection">The name of the collection.</param>
    /// <param name="document">The document to insert.</param>
    /// <param name="insertedId">The ID of the inserted document.</param>
    /// <returns>The current MongoHelper instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the helper has been disposed.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during insertion.</exception>
    /// <remarks>
    /// This method inserts a single document into the specified MongoDB collection.
    /// It converts the JObject to a BsonDocument before insertion.
    /// The inserted document's ID is returned via the out parameter.
    /// <para>
    /// Example:
    /// <code>
    /// var document = new JObject { ["name"] = "John Doe", ["age"] = 30 };
    /// mongoHelper.InsertOne("users", document, out string id);
    /// Console.WriteLine($"Inserted document ID: {id}");
    /// </code>
    /// </para>
    /// </remarks>
    /// <seealso href="https://mongodb.github.io/mongo-csharp-driver/2.19/apidocs/html/M_MongoDB_Driver_IMongoCollection_1_InsertOne.htm">MongoDB.Driver.IMongoCollection.InsertOne Method</seealso>
    /// <seealso href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">Newtonsoft.Json.Linq.JObject Class</seealso>
    public MongoHelper InsertOne(string collection, JObject document, out string insertedId)
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            var bsonDoc = BsonDocument.Parse(document.ToString());
            _database.GetCollection<BsonDocument>(collection).InsertOne(_session, bsonDoc);
            insertedId = bsonDoc["_id"]?.ToString() ?? string.Empty;
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Updates a single document in the specified collection.
    /// </summary>
    /// <param name="collection">The name of the collection.</param>
    /// <param name="filter">The filter to identify the document to update.</param>
    /// <param name="update">The update operations to apply.</param>
    /// <param name="modifiedCount">The number of documents modified.</param>
    /// <returns>The current MongoHelper instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the helper has been disposed.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during update.</exception>
    /// <remarks>
    /// This method updates a single document in the specified MongoDB collection that matches the given filter.
    /// It converts both the filter and update JObjects to BsonDocuments before performing the update.
    /// The number of modified documents is returned via the out parameter.
    /// <para>
    /// Example:
    /// <code>
    /// var filter = new JObject { ["name"] = "John Doe" };
    /// var update = new JObject { ["$set"] = new JObject { ["age"] = 31 } };
    /// mongoHelper.UpdateOne("users", filter, update, out long count);
    /// Console.WriteLine($"Modified {count} document(s)");
    /// </code>
    /// </para>
    /// </remarks>
    /// <seealso href="https://mongodb.github.io/mongo-csharp-driver/2.19/apidocs/html/M_MongoDB_Driver_IMongoCollection_1_UpdateOne.htm">MongoDB.Driver.IMongoCollection.UpdateOne Method</seealso>
    /// <seealso href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">Newtonsoft.Json.Linq.JObject Class</seealso>
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

    /// <summary>
    /// Deletes a single document from the specified collection.
    /// </summary>
    /// <param name="collection">The name of the collection.</param>
    /// <param name="filter">The filter to identify the document to delete.</param>
    /// <param name="deletedCount">The number of documents deleted.</param>
    /// <returns>The current MongoHelper instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the helper has been disposed.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during deletion.</exception>
    /// <remarks>
    /// This method deletes a single document from the specified MongoDB collection that matches the given filter.
    /// It converts the filter JObject to a BsonDocument before performing the deletion.
    /// The number of deleted documents is returned via the out parameter.
    /// <para>
    /// Example:
    /// <code>
    /// var filter = new JObject { ["name"] = "John Doe" };
    /// mongoHelper.DeleteOne("users", filter, out long count);
    /// Console.WriteLine($"Deleted {count} document(s)");
    /// </code>
    /// </para>
    /// </remarks>
    /// <seealso href="https://mongodb.github.io/mongo-csharp-driver/2.19/apidocs/html/M_MongoDB_Driver_IMongoCollection_1_DeleteOne.htm">MongoDB.Driver.IMongoCollection.DeleteOne Method</seealso>
    /// <seealso href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">Newtonsoft.Json.Linq.JObject Class</seealso>
    public MongoHelper DeleteOne(string collection, JObject filter, out long deletedCount)
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
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

    /// <summary>
    /// Finds documents in the specified collection that match the given filter.
    /// </summary>
    /// <param name="collection">The name of the collection.</param>
    /// <param name="filter">The filter to apply to the query.</param>
    /// <param name="results">The list of documents that match the filter.</param>
    /// <returns>The current MongoHelper instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the helper has been disposed.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during the query.</exception>
    /// <remarks>
    /// This method finds all documents in the specified MongoDB collection that match the given filter.
    /// It converts the filter JObject to a BsonDocument before performing the query.
    /// The results are returned as a list of JObjects via the out parameter.
    /// <para>
    /// Example:
    /// <code>
    /// var filter = new JObject { ["age"] = new JObject { ["$gt"] = 30 } };
    /// mongoHelper.Find("users", filter, out List&lt;JObject&gt; results);
    /// foreach (var user in results)
    /// {
    ///     Console.WriteLine($"Found user: {user["name"]}");
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    /// <seealso href="https://mongodb.github.io/mongo-csharp-driver/2.19/apidocs/html/M_MongoDB_Driver_IMongoCollection_1_Find.htm">MongoDB.Driver.IMongoCollection.Find Method</seealso>
    /// <seealso href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">Newtonsoft.Json.Linq.JObject Class</seealso>
    public MongoHelper Find(string collection, JObject filter, out List<JObject> results)
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            var docs = _database
                .GetCollection<BsonDocument>(collection)
                .Find(_session, BsonDocument.Parse(filter.ToString()))
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

    /// <summary>
    /// Finds a single document in the specified collection that matches the given filter.
    /// </summary>
    /// <param name="collection">The name of the collection.</param>
    /// <param name="filter">The filter to apply to the query.</param>
    /// <param name="result">The matching document, or null if no match is found.</param>
    /// <returns>The current MongoHelper instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the helper has been disposed.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during the query.</exception>
    /// <remarks>
    /// This method finds a single document in the specified MongoDB collection that matches the given filter.
    /// It converts the filter JObject to a BsonDocument before performing the query.
    /// The result is returned as a JObject (or null if no match is found) via the out parameter.
    /// <para>
    /// Example:
    /// <code>
    /// var filter = new JObject { ["name"] = "John Doe" };
    /// mongoHelper.FindOne("users", filter, out JObject? user);
    /// if (user != null)
    /// {
    ///     Console.WriteLine($"Found user: {user["name"]}, Age: {user["age"]}");
    /// }
    /// else
    /// {
    ///     Console.WriteLine("User not found");
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    /// <seealso href="https://mongodb.github.io/mongo-csharp-driver/2.19/apidocs/html/M_MongoDB_Driver_IMongoCollection_1_Find.htm">MongoDB.Driver.IMongoCollection.Find Method</seealso>
    /// <seealso href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">Newtonsoft.Json.Linq.JObject Class</seealso>
    public MongoHelper FindOne(string collection, JObject filter, out JObject? result)
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
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

    /// <summary>
    /// Asynchronously inserts a single document into the specified collection.
    /// </summary>
    /// <param name="collection">The name of the collection.</param>
    /// <param name="document">The document to insert.</param>
    /// <param name="callback">An optional callback action to be invoked with the inserted document's ID.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A tuple containing the current MongoHelper instance and the inserted document's ID.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the helper has been disposed.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during insertion.</exception>
    /// <remarks>
    /// This method asynchronously inserts a single document into the specified MongoDB collection.
    /// It converts the JObject to a BsonDocument before insertion.
    /// <para>
    /// Example:
    /// <code>
    /// var document = new JObject { ["name"] = "John Doe", ["age"] = 30 };
    /// var (helper, id) = await mongoHelper.InsertOneAsync("users", document);
    /// Console.WriteLine($"Inserted document ID: {id}");
    /// </code>
    /// </para>
    /// </remarks>
    /// <seealso href="https://mongodb.github.io/mongo-csharp-driver/2.19/apidocs/html/M_MongoDB_Driver_IMongoCollection_1_InsertOneAsync.htm">MongoDB.Driver.IMongoCollection.InsertOneAsync Method</seealso>
    /// <seealso href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">Newtonsoft.Json.Linq.JObject Class</seealso>
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

            // Gunakan StartSession jika _session null
            if (_session == null)
            {
                _session = await _client.StartSessionAsync(cancellationToken: cancellationToken);
            }

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

    /// <summary>
    /// Asynchronously updates a single document in the specified collection.
    /// </summary>
    /// <param name="collection">The name of the collection.</param>
    /// <param name="filter">The filter to identify the document to update.</param>
    /// <param name="update">The update operations to apply.</param>
    /// <param name="callback">An optional callback action to be invoked with the number of modified documents.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A tuple containing the current MongoHelper instance and the number of documents modified.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the helper has been disposed.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during update.</exception>
    /// <remarks>
    /// This method asynchronously updates a single document in the specified MongoDB collection that matches the given filter.
    /// It converts both the filter and update JObjects to BsonDocuments before performing the update.
    /// <para>
    /// Example:
    /// <code>
    /// var filter = new JObject { ["name"] = "John Doe" };
    /// var update = new JObject { ["$set"] = new JObject { ["age"] = 31 } };
    /// var (helper, count) = await mongoHelper.UpdateOneAsync("users", filter, update);
    /// Console.WriteLine($"Modified {count} document(s)");
    /// </code>
    /// </para>
    /// </remarks>
    /// <seealso href="https://mongodb.github.io/mongo-csharp-driver/2.19/apidocs/html/M_MongoDB_Driver_IMongoCollection_1_UpdateOneAsync.htm">MongoDB.Driver.IMongoCollection.UpdateOneAsync Method</seealso>
    /// <seealso href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">Newtonsoft.Json.Linq.JObject Class</seealso>
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

            // Gunakan StartSession jika _session null
            if (_session == null)
            {
                _session = await _client.StartSessionAsync(cancellationToken: cancellationToken);
            }

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

    /// <summary>
    /// Asynchronously deletes a single document from the specified collection.
    /// </summary>
    /// <param name="collection">The name of the collection.</param>
    /// <param name="filter">The filter to identify the document to delete.</param>
    /// <param name="callback">An optional callback action to be invoked with the number of deleted documents.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A tuple containing the current MongoHelper instance and the number of documents deleted.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the helper has been disposed.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during deletion.</exception>
    /// <remarks>
    /// This method asynchronously deletes a single document from the specified MongoDB collection that matches the given filter.
    /// It converts the filter JObject to a BsonDocument before performing the deletion.
    /// <para>
    /// Example:
    /// <code>
    /// var filter = new JObject { ["name"] = "John Doe" };
    /// var (helper, count) = await mongoHelper.DeleteOneAsync("users", filter);
    /// Console.WriteLine($"Deleted {count} document(s)");
    /// </code>
    /// </para>
    /// </remarks>
    /// <seealso href="https://mongodb.github.io/mongo-csharp-driver/2.19/apidocs/html/M_MongoDB_Driver_IMongoCollection_1_DeleteOneAsync.htm">MongoDB.Driver.IMongoCollection.DeleteOneAsync Method</seealso>
    /// <seealso href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">Newtonsoft.Json.Linq.JObject Class</seealso>
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

            // Gunakan StartSession jika _session null
            if (_session == null)
            {
                _session = await _client.StartSessionAsync(cancellationToken: cancellationToken);
            }

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

    /// <summary>
    /// Asynchronously finds documents in the specified collection that match the given filter.
    /// </summary>
    /// <param name="collection">The name of the collection.</param>
    /// <param name="filter">The filter to apply to the query.</param>
    /// <param name="callback">An optional callback action to be invoked with the list of documents that match the filter.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A tuple containing the current MongoHelper instance and the list of documents that match the filter.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the helper has been disposed.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during the query.</exception>
    /// <remarks>
    /// This method asynchronously finds all documents in the specified MongoDB collection that match the given filter.
    /// It converts the filter JObject to a BsonDocument before performing the query.
    /// The results are returned as a list of JObjects.
    /// <para>
    /// Example:
    /// <code>
    /// var filter = new JObject { ["age"] = new JObject { ["$gt"] = 30 } };
    /// var (helper, results) = await mongoHelper.FindAsync("users", filter);
    /// foreach (var user in results)
    /// {
    ///     Console.WriteLine($"Found user: {user["name"]}");
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    /// <seealso href="https://mongodb.github.io/mongo-csharp-driver/2.19/apidocs/html/M_MongoDB_Driver_IMongoCollection_1_FindAsync.htm">MongoDB.Driver.IMongoCollection.FindAsync Method</seealso>
    /// <seealso href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">Newtonsoft.Json.Linq.JObject Class</seealso>
    public async Task<(MongoHelper helper, List<JObject> results)> FindAsync(
        string collection,
        JObject filter,
        Action<List<JObject>>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            var docs = await _database
                .GetCollection<BsonDocument>(collection)
                .Find(filter: BsonDocument.Parse(filter.ToString()), options: null)
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

    /// <summary>
    /// Asynchronously finds a single document in the specified collection that matches the given filter.
    /// </summary>
    /// <param name="collection">The name of the collection.</param>
    /// <param name="filter">The filter to apply to the query.</param>
    /// <param name="callback">An optional callback action to be invoked with the matching document, or null if no match is found.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A tuple containing the current MongoHelper instance and the matching document, or null if no match is found.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the helper has been disposed.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during the query.</exception>
    /// <remarks>
    /// This method asynchronously finds a single document in the specified MongoDB collection that matches the given filter.
    /// It converts the filter JObject to a BsonDocument before performing the query.
    /// The result is returned as a JObject (or null if no match is found).
    /// <para>
    /// Example:
    /// <code>
    /// var filter = new JObject { ["name"] = "John Doe" };
    /// var (helper, user) = await mongoHelper.FindOneAsync("users", filter);
    /// if (user != null)
    /// {
    ///     Console.WriteLine($"Found user: {user["name"]}, Age: {user["age"]}");
    /// }
    /// else
    /// {
    ///     Console.WriteLine("User not found");
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    /// <seealso href="https://mongodb.github.io/mongo-csharp-driver/2.19/apidocs/html/M_MongoDB_Driver_IMongoCollection_1_FindAsync.htm">MongoDB.Driver.IMongoCollection.FindAsync Method</seealso>
    /// <seealso href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">Newtonsoft.Json.Linq.JObject Class</seealso>
    public async Task<(MongoHelper helper, JObject? result)> FindOneAsync(
        string collection,
        JObject filter,
        Action<JObject?>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            // Gunakan StartSession jika _session null
            if (_session == null)
            {
                _session = await _client.StartSessionAsync(cancellationToken: cancellationToken);
            }

            var doc = await _database
                .GetCollection<BsonDocument>(collection)
                .Find(_session, BsonDocument.Parse(filter.ToString()))
                .FirstOrDefaultAsync(cancellationToken);

            var result = doc is not null ? JObject.Parse(doc.ToJson()) : null;
            callback?.Invoke(result);
            return (helper: this, result);
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Gets the next incremental ID for a collection.
    /// </summary>
    /// <param name="collection">The name of the collection.</param>
    /// <returns>The next available incremental ID.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the helper has been disposed.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during the operation.</exception>
    /// <remarks>
    /// This method finds the highest numeric ID in the collection and returns the next available number.
    /// If no numeric IDs exist, returns 1 as the first ID.
    /// <para>
    /// Example:
    /// <code>
    /// var nextId = mongoHelper.GetIncrementId("users");
    /// var document = new JObject { ["_id"] = nextId, ["name"] = "John Doe" };
    /// </code>
    /// </para>
    /// </remarks>
    public int GetIncrementId(string collection)
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

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

    /// <summary>
    /// Asynchronously gets the next incremental ID for a collection.
    /// </summary>
    /// <param name="collection">The name of the collection.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <param name="callback">An optional callback action to be invoked with the next ID value.</param>
    /// <returns>A tuple containing the current MongoHelper instance and the next available incremental ID.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the helper has been disposed.</exception>
    /// <exception cref="Exception">Rethrows any exceptions that occur during the operation.</exception>
    /// <remarks>
    /// This method asynchronously finds the highest numeric ID in the collection and returns the next available number.
    /// If no numeric IDs exist, returns 1 as the first ID.
    /// <para>
    /// Example:
    /// <code>
    /// var (helper, nextId) = await mongoHelper.GetIncrementIdAsync(
    ///     collection: "users",
    ///     callback: id => Console.WriteLine($"Next ID: {id}"),
    ///     cancellationToken: token
    /// );
    /// var document = new JObject { ["_id"] = nextId, ["name"] = "John Doe" };
    /// </code>
    /// </para>
    /// </remarks>
    public async Task<(MongoHelper helper, int id)> GetIncrementIdAsync(
        string collection,
        Action<int>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            var maxIdDoc = await _database
                .GetCollection<BsonDocument>(collection)
                .Find(new BsonDocument())
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
}
