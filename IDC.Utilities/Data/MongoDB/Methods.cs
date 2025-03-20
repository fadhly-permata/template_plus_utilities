using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace IDC.Utilities.Data;

public sealed partial class MongoHelper
{
    /// <summary>
    /// Inserts a single document into the specified MongoDB collection.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="document">The document to be inserted as a JObject.</param>
    /// <param name="insertedId">The unique identifier of the inserted document.</param>
    /// <returns>The current <see cref="MongoHelper"/> instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation exceeds the configured timeout.</exception>
    /// <remarks>
    /// Performs a single document insertion operation with automatic ID generation.
    ///
    /// > [!IMPORTANT]
    /// > The document must not contain an '_id' field as MongoDB will generate one automatically.
    ///
    /// > [!NOTE]
    /// > This method is thread-safe and uses the current session if transaction is active.
    ///
    /// Example usage:
    /// <code>
    /// var document = new JObject
    /// {
    ///     ["name"] = "John Doe",
    ///     ["email"] = "john@example.com",
    ///     ["age"] = 30,
    ///     ["isActive"] = true,
    ///     ["tags"] = new JArray { "user", "customer" }
    /// };
    ///
    /// mongoHelper
    ///     .InsertOne("users", document: document, out string id)
    ///     .CommitTransaction();
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.collection.insertOne/">MongoDB insertOne Documentation</seealso>
    /// <seealso href="https://mongodb.github.io/mongo-csharp-driver/2.19/reference/driver/crud/writing/">MongoDB C# Driver Writing Documentation</seealso>
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
    /// Updates a single document in the specified MongoDB collection.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the document to update.</param>
    /// <param name="update">The update operations to apply.</param>
    /// <param name="modifiedCount">The number of documents that were modified.</param>
    /// <returns>The current <see cref="MongoHelper"/> instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation exceeds the configured timeout.</exception>
    /// <remarks>
    /// Updates the first document that matches the specified filter criteria.
    ///
    /// > [!IMPORTANT]
    /// > The update document must include valid MongoDB update operators (e.g., $set, $unset).
    ///
    /// > [!WARNING]
    /// > Without update operators, the operation will replace the entire document.
    ///
    /// > [!NOTE]
    /// > This method is thread-safe and uses the current session if transaction is active.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject
    /// {
    ///     ["email"] = "john@example.com"
    /// };
    ///
    /// var update = new JObject
    /// {
    ///     ["$set"] = new JObject
    ///     {
    ///         ["age"] = 31,
    ///         ["lastUpdated"] = DateTime.UtcNow
    ///     },
    ///     ["$push"] = new JObject
    ///     {
    ///         ["tags"] = "premium"
    ///     }
    /// };
    ///
    /// mongoHelper
    ///     .UpdateOne("users", filter: filter, update: update, out long count)
    ///     .CommitTransaction();
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.collection.updateOne/">MongoDB updateOne Documentation</seealso>
    /// <seealso href="https://mongodb.github.io/mongo-csharp-driver/2.19/reference/driver/crud/writing/">MongoDB C# Driver Writing Documentation</seealso>
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
    /// Deletes a single document from the specified MongoDB collection.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the document to delete.</param>
    /// <param name="deletedCount">The number of documents that were deleted.</param>
    /// <returns>The current <see cref="MongoHelper"/> instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation exceeds the configured timeout.</exception>
    /// <remarks>
    /// Deletes the first document that matches the specified filter criteria.
    ///
    /// > [!CAUTION]
    /// > This operation is irreversible. Consider using soft delete patterns for critical data.
    ///
    /// > [!TIP]
    /// > For better performance with known _id, use: `new JObject { ["_id"] = ObjectId.Parse(id) }`
    ///
    /// > [!NOTE]
    /// > This method is thread-safe and uses the current session if transaction is active.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject
    /// {
    ///     ["$or"] = new JArray
    ///     {
    ///         new JObject { ["email"] = "john@example.com" },
    ///         new JObject { ["username"] = "johndoe" }
    ///     }
    /// };
    ///
    /// mongoHelper
    ///     .DeleteOne("users", filter: filter, out long count)
    ///     .CommitTransaction();
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.collection.deleteOne/">MongoDB deleteOne Documentation</seealso>
    /// <seealso href="https://mongodb.github.io/mongo-csharp-driver/2.19/reference/driver/crud/writing/">MongoDB C# Driver Writing Documentation</seealso>
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
    /// Finds documents in the specified MongoDB collection based on a filter criteria.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify matching documents.</param>
    /// <param name="results">The list of matching documents as JObjects.</param>
    /// <returns>The current <see cref="MongoHelper"/> instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation exceeds the configured timeout.</exception>
    /// <remarks>
    /// Retrieves all documents matching the specified filter criteria.
    ///
    /// > [!TIP]
    /// > Use pagination for large result sets to improve performance.
    ///
    /// > [!IMPORTANT]
    /// > The filter must use valid MongoDB query operators (e.g., $eq, $gt, $in).
    ///
    /// > [!NOTE]
    /// > This method is thread-safe and uses the current session if transaction is active.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject
    /// {
    ///     ["age"] = new JObject { ["$gte"] = 21 },
    ///     ["status"] = "active",
    ///     ["tags"] = new JObject
    ///     {
    ///         ["$in"] = new JArray { "premium", "vip" }
    ///     }
    /// };
    ///
    /// mongoHelper
    ///     .Find("users", filter: filter, out List&lt;JObject&gt; users)
    ///     .CommitTransaction();
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/query/">MongoDB Query Operators</seealso>
    /// <seealso href="https://mongodb.github.io/mongo-csharp-driver/2.19/reference/driver/crud/reading/">MongoDB C# Driver Reading Documentation</seealso>
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
    /// Finds a single document in the specified MongoDB collection based on a filter criteria.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the matching document.</param>
    /// <param name="result">The matching document as JObject, or null if no match found.</param>
    /// <returns>The current <see cref="MongoHelper"/> instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation exceeds the configured timeout.</exception>
    /// <remarks>
    /// Retrieves the first document that matches the specified filter criteria.
    ///
    /// > [!TIP]
    /// > For exact matches using _id, use: `new JObject { ["_id"] = ObjectId.Parse(id) }`
    ///
    /// > [!NOTE]
    /// > Returns null when no matching document is found.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject
    /// {
    ///     ["$or"] = new JArray
    ///     {
    ///         new JObject { ["email"] = "john@example.com" },
    ///         new JObject
    ///         {
    ///             ["phone"] = new JObject
    ///             {
    ///                 ["$regex"] = "^\\+1-555-",
    ///                 ["$options"] = "i"
    ///             }
    ///         }
    ///     }
    /// };
    ///
    /// mongoHelper
    ///     .FindOne("users", filter: filter, out JObject? user)
    ///     .CommitTransaction();
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/query/">MongoDB Query Operators</seealso>
    /// <seealso href="https://mongodb.github.io/mongo-csharp-driver/2.19/reference/driver/crud/reading/">MongoDB C# Driver Reading Documentation</seealso>
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
    /// Asynchronously inserts a single document into the specified MongoDB collection.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="document">The document to insert as a JObject.</param>
    /// <param name="callback">Optional callback receiving the inserted document's ID.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>A tuple containing the helper instance and inserted document's ID.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    /// <remarks>
    /// Performs an asynchronous document insertion with automatic ID generation.
    ///
    /// > [!IMPORTANT]
    /// > The document must not contain an '_id' field.
    ///
    /// > [!NOTE]
    /// > Automatically starts a new session if none exists.
    ///
    /// Example usage:
    /// <code>
    /// var document = new JObject
    /// {
    ///     ["name"] = "John Doe",
    ///     ["profile"] = new JObject
    ///     {
    ///         ["age"] = 30,
    ///         ["email"] = "john@example.com"
    ///     },
    ///     ["preferences"] = new JObject
    ///     {
    ///         ["theme"] = "dark",
    ///         ["notifications"] = true
    ///     },
    ///     ["lastLogin"] = DateTime.UtcNow
    /// };
    ///
    /// var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
    /// var (helper, id) = await mongoHelper.InsertOneAsync(
    ///     collection: "users",
    ///     document: document,
    ///     callback: insertedId => Console.WriteLine($"Inserted: {insertedId}"),
    ///     cancellationToken: cts.Token
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.collection.insertOne/">MongoDB insertOne Documentation</seealso>
    /// <seealso href="https://mongodb.github.io/mongo-csharp-driver/2.19/reference/driver/crud/writing/">MongoDB C# Driver Writing Documentation</seealso>
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
    /// Asynchronously updates a single document in the specified MongoDB collection.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the document to update.</param>
    /// <param name="update">The update operations to apply using MongoDB update operators.</param>
    /// <param name="callback">Optional callback receiving the number of modified documents.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>A tuple containing the helper instance and count of modified documents.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    /// <remarks>
    /// Performs an atomic update operation on a single document.
    ///
    /// > [!IMPORTANT]
    /// > Update operations must use valid MongoDB operators (e.g., $set, $inc, $push).
    ///
    /// > [!NOTE]
    /// > Returns modifiedCount=0 if no document matches the filter.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject
    /// {
    ///     ["email"] = "john@example.com",
    ///     ["status"] = "active"
    /// };
    ///
    /// var update = new JObject
    /// {
    ///     ["$set"] = new JObject
    ///     {
    ///         ["lastLogin"] = DateTime.UtcNow,
    ///         ["profile.verified"] = true
    ///     },
    ///     ["$inc"] = new JObject
    ///     {
    ///         ["loginCount"] = 1
    ///     },
    ///     ["$push"] = new JObject
    ///     {
    ///         ["loginHistory"] = DateTime.UtcNow
    ///     }
    /// };
    ///
    /// var (helper, count) = await mongoHelper.UpdateOneAsync(
    ///     collection: "users",
    ///     filter: filter,
    ///     update: update,
    ///     callback: modifiedCount => Console.WriteLine($"Updated: {modifiedCount} document")
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/">MongoDB Update Operators</seealso>
    /// <seealso href="https://mongodb.github.io/mongo-csharp-driver/2.19/reference/driver/crud/writing/">MongoDB C# Driver Writing Documentation</seealso>
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
    /// Asynchronously deletes a single document from the specified MongoDB collection.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the document to delete.</param>
    /// <param name="callback">Optional callback receiving the number of deleted documents.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>A tuple containing the helper instance and count of deleted documents.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    /// <remarks>
    /// Performs an atomic deletion of a single document.
    ///
    /// > [!WARNING]
    /// > This operation is irreversible. Consider using soft deletes for critical data.
    ///
    /// > [!NOTE]
    /// > Returns deletedCount=0 if no document matches the filter.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject
    /// {
    ///     ["_id"] = ObjectId.Parse(userId),
    ///     ["status"] = new JObject
    ///     {
    ///         ["$in"] = new JArray { "inactive", "suspended" }
    ///     }
    /// };
    ///
    /// var (helper, count) = await mongoHelper.DeleteOneAsync(
    ///     collection: "users",
    ///     filter: filter,
    ///     callback: deletedCount =>
    ///     {
    ///         if (deletedCount > 0)
    ///             Console.WriteLine("User successfully deleted");
    ///     }
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.collection.deleteOne/">MongoDB deleteOne Documentation</seealso>
    /// <seealso href="https://mongodb.github.io/mongo-csharp-driver/2.19/reference/driver/crud/writing/">MongoDB C# Driver Writing Documentation</seealso>
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
    /// Asynchronously retrieves documents from the specified MongoDB collection.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify matching documents.</param>
    /// <param name="callback">Optional callback receiving the list of matching documents.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>A tuple containing the helper instance and list of matching documents.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    /// <remarks>
    /// Retrieves all documents matching the specified filter criteria.
    ///
    /// > [!TIP]
    /// > Use pagination for large result sets to improve performance.
    ///
    /// > [!IMPORTANT]
    /// > Complex queries may impact performance. Consider indexing frequently queried fields.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject
    /// {
    ///     ["age"] = new JObject { ["$gte"] = 21 },
    ///     ["interests"] = new JObject
    ///     {
    ///         ["$all"] = new JArray { "coding", "mongodb" }
    ///     },
    ///     ["lastLogin"] = new JObject
    ///     {
    ///         ["$gte"] = DateTime.UtcNow.AddDays(-30)
    ///     }
    /// };
    ///
    /// var (helper, users) = await mongoHelper.FindAsync(
    ///     collection: "users",
    ///     filter: filter,
    ///     callback: results =>
    ///     {
    ///         foreach (var user in results)
    ///             Console.WriteLine($"Found: {user["name"]}");
    ///     }
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/query/">MongoDB Query Operators</seealso>
    /// <seealso href="https://mongodb.github.io/mongo-csharp-driver/2.19/reference/driver/crud/reading/">MongoDB C# Driver Reading Documentation</seealso>
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
    /// Asynchronously retrieves a single document from a MongoDB collection based on specified criteria.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the document.</param>
    /// <param name="callback">Optional callback receiving the found document or null if not found.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>A tuple containing the helper instance and the found document (or null).</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <exception cref="FormatException">Thrown when the filter JSON is invalid.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    /// <remarks>
    /// Retrieves the first document matching the specified filter criteria.
    ///
    /// > [!IMPORTANT]
    /// > If multiple documents match the filter, only the first one is returned.
    ///
    /// > [!TIP]
    /// > Use indexes on frequently queried fields to improve performance.
    ///
    /// > [!NOTE]
    /// > Returns null when no document matches the filter.
    ///
    /// Example usage with various query operators:
    /// <code>
    /// // Simple equality match
    /// var filter1 = new JObject
    /// {
    ///     ["email"] = "user@example.com",
    ///     ["isActive"] = true
    /// };
    ///
    /// // Complex query with multiple conditions
    /// var filter2 = new JObject
    /// {
    ///     ["age"] = new JObject { ["$gte"] = 18 },
    ///     ["lastLogin"] = new JObject
    ///     {
    ///         ["$gte"] = DateTime.UtcNow.AddDays(-30)
    ///     },
    ///     ["roles"] = new JObject
    ///     {
    ///         ["$in"] = new JArray { "admin", "moderator" }
    ///     },
    ///     ["$or"] = new JArray
    ///     {
    ///         new JObject { ["premium"] = true },
    ///         new JObject { ["credits"] = new JObject { ["$gt"] = 1000 } }
    ///     }
    /// };
    ///
    /// // Usage with callback
    /// var (helper, user) = await mongoHelper.FindOneAsync(
    ///     collection: "users",
    ///     filter: filter2,
    ///     callback: result =>
    ///     {
    ///         if (result != null)
    ///         {
    ///             var userId = result["_id"].ToString();
    ///             var username = result["username"].ToString();
    ///             Console.WriteLine($"Found user {username} with ID {userId}");
    ///         }
    ///     }
    /// );
    ///
    /// // Handling null result
    /// if (user == null)
    /// {
    ///     throw new KeyNotFoundException("User not found");
    /// }
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/query/">MongoDB Query Operators</seealso>
    /// <seealso href="https://www.mongodb.com/docs/manual/tutorial/query-documents/">MongoDB Query Documents</seealso>
    /// <seealso href="https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/crud/read-operations/">MongoDB C# Driver Read Operations</seealso>
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
    /// Gets or generates the next sequential ID for the specified collection.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <returns>The next available numeric ID.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <remarks>
    /// Provides auto-incrementing numeric IDs for collections.
    ///
    /// > [!IMPORTANT]
    /// > This method is not atomic. For high-concurrency scenarios, consider using MongoDB's ObjectId.
    ///
    /// > [!NOTE]
    /// > Returns 1 for empty collections or when no numeric IDs exist.
    ///
    /// Example usage:
    /// <code>
    /// var nextId = mongoHelper.GetIncrementId("orders");
    ///
    /// var order = new JObject
    /// {
    ///     ["_id"] = nextId,
    ///     ["userId"] = ObjectId.Parse(userId),
    ///     ["items"] = new JArray
    ///     {
    ///         new JObject
    ///         {
    ///             ["productId"] = 1001,
    ///             ["quantity"] = 2
    ///         }
    ///     },
    ///     ["status"] = "pending",
    ///     ["createdAt"] = DateTime.UtcNow
    /// };
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/ObjectId/">MongoDB ObjectId Documentation</seealso>
    /// <seealso href="https://www.mongodb.com/docs/manual/tutorial/create-an-auto-incrementing-field/">MongoDB Auto-Incrementing Field Tutorial</seealso>
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
    /// Asynchronously generates the next sequential numeric identifier for a MongoDB collection.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="callback">Optional callback to process the generated ID before returning.</param>
    /// <param name="cancellationToken">Token to cancel the async operation.</param>
    /// <returns>A tuple containing the current helper instance and the next available ID.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the helper instance has been disposed.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    /// <exception cref="FormatException">Thrown when existing IDs cannot be parsed as integers.</exception>
    /// <remarks>
    /// Provides auto-incrementing numeric IDs for MongoDB collections by finding the highest existing ID
    /// and incrementing it by one.
    ///
    /// > [!IMPORTANT]
    /// > This method is not atomic and may produce duplicate IDs in high-concurrency scenarios.
    /// > Consider using MongoDB's native ObjectId for production environments.
    ///
    /// > [!CAUTION]
    /// > Ensure all documents in the collection use consistent ID formats to prevent parsing errors.
    ///
    /// > [!TIP]
    /// > For better performance, create an index on the _id field: ```db.collection.createIndex({ "_id": 1 })```
    ///
    /// Example usage scenarios:
    /// <code>
    /// // Basic usage
    /// var (helper, nextId) = await mongoHelper.GetIncrementIdAsync(
    ///     collection: "orders",
    ///     callback: id => Console.WriteLine($"Generated ID: {id}")
    /// );
    ///
    /// // With error handling
    /// try
    /// {
    ///     var (_, orderId) = await mongoHelper.GetIncrementIdAsync(
    ///         collection: "orders",
    ///         callback: id =>
    ///         {
    ///             if (id > 1_000_000)
    ///             {
    ///                 throw new InvalidOperationException("ID limit exceeded");
    ///             }
    ///         }
    ///     );
    ///
    ///     var order = new JObject
    ///     {
    ///         ["_id"] = orderId,
    ///         ["customer"] = "CUST001",
    ///         ["items"] = new JArray
    ///         {
    ///             new JObject
    ///             {
    ///                 ["productId"] = "PROD123",
    ///                 ["quantity"] = 2
    ///             }
    ///         },
    ///         ["timestamp"] = DateTime.UtcNow
    ///     };
    /// }
    /// catch (Exception ex) when (ex is not OperationCanceledException)
    /// {
    ///     // Handle errors
    /// }
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/ObjectId/">MongoDB ObjectId</seealso>
    /// <seealso href="https://www.mongodb.com/docs/manual/core/indexes/">MongoDB Indexes</seealso>
    /// <seealso href="https://www.mongodb.com/docs/manual/tutorial/create-an-auto-incrementing-field/">Auto-Incrementing Field Tutorial</seealso>
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
