using IDC.Utilities.Extensions;
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
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(document);
        ObjectDisposedException.ThrowIf(_disposed, this);

        try
        {
            _session ??= _client.StartSession();
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
            _session ??= _client.StartSession();

            // Ambil dokumen yang ada
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

                // Hapus _id dari existingJson untuk menghindari error
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
            _session ??= _client.StartSession();
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

    /// <summary>
    /// Appends a value to an array field in a document.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the document.</param>
    /// <param name="arrayPath">The path to the array field.</param>
    /// <param name="value">The value to append to the array.</param>
    /// <param name="modifiedCount">The number of documents modified. For details, see <see href="https://mongodb.github.io/mongo-csharp-driver/2.24/apidocs/html/P_MongoDB_Driver_UpdateResult_ModifiedCount.htm">MongoDB UpdateResult.ModifiedCount</see>.</param>
    /// <returns>The current <see cref="MongoHelper"/> instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <exception cref="FormatException">Thrown when the filter or value cannot be parsed to BSON.</exception>
    /// <remarks>
    /// Uses MongoDB's $push operator to append elements to an array field. This method is atomic and thread-safe.
    ///
    /// > [!NOTE]
    /// > If the array field doesn't exist, MongoDB will automatically create it.
    ///
    /// > [!IMPORTANT]
    /// > Ensure the filter matches exactly one document to avoid unintended modifications.
    ///
    /// Example usage:
    /// <code>
    /// // Adding a simple value
    /// var simpleFilter = new JObject { ["username"] = "johndoe" };
    /// mongoHelper.ArrayPush(
    ///     collection: "users",
    ///     filter: simpleFilter,
    ///     arrayPath: "tags",
    ///     value: "premium",
    ///     out long simpleModifiedCount
    /// );
    ///
    /// // Adding a complex object
    /// var complexFilter = new JObject { ["_id"] = ObjectId.Parse(userId) };
    /// var hobby = new JObject
    /// {
    ///     ["name"] = "reading",
    ///     ["level"] = "intermediate",
    ///     ["categories"] = new JArray { "fiction", "science" },
    ///     ["metadata"] = new JObject
    ///     {
    ///         ["addedAt"] = DateTime.UtcNow,
    ///         ["source"] = "user_input"
    ///     }
    /// };
    ///
    /// mongoHelper.ArrayPush(
    ///     collection: "users",
    ///     filter: complexFilter,
    ///     arrayPath: "profile.hobbies",
    ///     value: hobby,
    ///     out long complexModifiedCount
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/push/">MongoDB $push Documentation</seealso>
    /// <seealso href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">Newtonsoft.Json.Linq.JObject Documentation</seealso>
    /// <seealso href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JToken.htm">Newtonsoft.Json.Linq.JToken Documentation</seealso>
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

    /// <summary>
    /// Updates an array element at a specific index in a MongoDB document.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the document.</param>
    /// <param name="arrayPath">The path to the array field (e.g., "items" or "profile.hobbies").</param>
    /// <param name="index">The zero-based index of the element to update.</param>
    /// <param name="value">The new value for the array element.</param>
    /// <param name="modifiedCount">The number of documents modified by the operation.</param>
    /// <returns>The current <see cref="MongoHelper"/> instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <remarks>
    /// Uses MongoDB's positional $ operator for direct index access.
    ///
    /// > [!IMPORTANT]
    /// > Ensure the index exists to avoid creating sparse arrays.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject { ["_id"] = ObjectId.Parse(userId) };
    /// var updatedPhone = new JObject
    /// {
    ///     ["type"] = "mobile",
    ///     ["number"] = "+1-555-0123",
    ///     ["verified"] = true,
    ///     ["updatedAt"] = DateTime.UtcNow
    /// };
    ///
    /// mongoHelper.ArraySet(
    ///     collection: "users",
    ///     filter: filter,
    ///     arrayPath: "contact.phones",
    ///     index: 0,
    ///     value: updatedPhone,
    ///     out long modifiedCount
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/positional/">MongoDB Positional $ Operator</seealso>
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

    /// <summary>
    /// Updates array elements that match specific criteria in a MongoDB document.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the document.</param>
    /// <param name="arrayPath">The path to the array field (e.g., "items" or "profile.hobbies").</param>
    /// <param name="arrayFilter">The criteria to identify array elements to update.</param>
    /// <param name="value">The new value for matching array elements.</param>
    /// <param name="modifiedCount">The number of documents modified by the operation.</param>
    /// <returns>The current <see cref="MongoHelper"/> instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <remarks>
    /// Uses MongoDB's arrayFilters for conditional array updates.
    ///
    /// > [!TIP]
    /// > Use dot notation in arrayFilter to match nested properties.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject { ["_id"] = ObjectId.Parse(userId) };
    /// var arrayFilter = new JObject { ["elem.type"] = "work" };
    /// var updatedAddress = new JObject
    /// {
    ///     ["type"] = "work",
    ///     ["street"] = "123 Business Ave",
    ///     ["city"] = "Enterprise City",
    ///     ["verified"] = true,
    ///     ["updatedAt"] = DateTime.UtcNow
    /// };
    ///
    /// mongoHelper.ArrayUpdate(
    ///     collection: "users",
    ///     filter: filter,
    ///     arrayPath: "addresses",
    ///     arrayFilter: arrayFilter,
    ///     value: updatedAddress,
    ///     out long modifiedCount
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/positional-filtered/">MongoDB Filtered Positional Operator</seealso>
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

    /// <summary>
    /// Removes elements from an array that match specific criteria in a MongoDB document.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the document.</param>
    /// <param name="arrayPath">The path to the array field (e.g., "items" or "profile.hobbies").</param>
    /// <param name="condition">The criteria to identify array elements to remove.</param>
    /// <param name="modifiedCount">The number of documents modified by the operation.</param>
    /// <returns>The current <see cref="MongoHelper"/> instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <remarks>
    /// Uses MongoDB's $pull operator for conditional array element removal.
    ///
    /// > [!NOTE]
    /// > Removes all matching elements in a single operation.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject { ["_id"] = ObjectId.Parse(userId) };
    /// var condition = new JObject
    /// {
    ///     ["type"] = "temporary",
    ///     ["expiresAt"] = new JObject { ["$lt"] = DateTime.UtcNow }
    /// };
    ///
    /// mongoHelper.ArrayPull(
    ///     collection: "users",
    ///     filter: filter,
    ///     arrayPath: "accessTokens",
    ///     condition: condition,
    ///     out long modifiedCount
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/pull/">MongoDB $pull Documentation</seealso>
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

    /// <summary>
    /// Removes an array element at a specific index in a MongoDB document.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the document.</param>
    /// <param name="arrayPath">The path to the array field (e.g., "items" or "profile.hobbies").</param>
    /// <param name="index">The zero-based index of the element to remove.</param>
    /// <param name="modifiedCount">The number of documents modified by the operation.</param>
    /// <returns>The current <see cref="MongoHelper"/> instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <remarks>
    /// Uses a two-step process: unset the element, then pull null values.
    ///
    /// > [!IMPORTANT]
    /// > This operation maintains array integrity by removing the gap.
    ///
    /// > [!WARNING]
    /// > Ensure the index exists to avoid errors.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject { ["_id"] = ObjectId.Parse(userId) };
    ///
    /// mongoHelper.ArrayRemoveAt(
    ///     collection: "users",
    ///     filter: filter,
    ///     arrayPath: "notifications",
    ///     index: 0,
    ///     out long modifiedCount
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/unset/">MongoDB $unset Documentation</seealso>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/pull/">MongoDB $pull Documentation</seealso>
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

    /// <summary>
    /// Updates all elements in an array field with a new value in a MongoDB document.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the document.</param>
    /// <param name="arrayPath">The path to the array field (e.g., "items" or "profile.hobbies").</param>
    /// <param name="value">The new value to replace all array elements.</param>
    /// <param name="modifiedCount">The number of documents modified by the operation.</param>
    /// <returns>The current <see cref="MongoHelper"/> instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the target field is not an array.</exception>
    /// <remarks>
    /// Replaces all elements in the target array with the provided value.
    ///
    /// > [!IMPORTANT]
    /// > This operation will fail if the target field is not an array.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject { ["_id"] = ObjectId.Parse(userId) };
    /// var newPermissions = new JArray { "read", "write", "admin" };
    ///
    /// mongoHelper.ArrayReplaceAll(
    ///     collection: "users",
    ///     filter: filter,
    ///     arrayPath: "permissions",
    ///     value: newPermissions,
    ///     out long modifiedCount
    /// );
    /// </code>
    /// </remarks>
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
            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);

            // Ambil dokumen yang ada
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

                // Hapus _id dari existingJson untuk menghindari error
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
            _session ??= await _client.StartSessionAsync(cancellationToken: cancellationToken);

            var docs = await _database
                .GetCollection<BsonDocument>(collection)
                .Find(_session, BsonDocument.Parse(filter.ToString()))
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

    /// <summary>
    /// Asynchronously appends a value to an array field in a document.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the document.</param>
    /// <param name="arrayPath">The path to the array field.</param>
    /// <param name="value">The value to append to the array.</param>
    /// <param name="callback">Optional callback to process the modified count before returning.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>A tuple containing the helper instance and the modified count.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <exception cref="FormatException">Thrown when the filter or value cannot be parsed to BSON.</exception>
    /// <remarks>
    /// Uses MongoDB's $push operator to append elements to an array field. This method is atomic and thread-safe.
    ///
    /// > [!NOTE]
    /// > If the array field doesn't exist, MongoDB will automatically create it.
    ///
    /// > [!IMPORTANT]
    /// > Ensure the filter matches exactly one document to avoid unintended modifications.
    ///
    /// Example usage:
    /// <code>
    /// // Adding a simple value
    /// var simpleFilter = new JObject { ["username"] = "johndoe" };
    /// mongoHelper.ArrayPush(
    ///     collection: "users",
    ///     filter: simpleFilter,
    ///     arrayPath: "tags",
    ///     value: "premium",
    ///     out long simpleModifiedCount
    /// );
    ///
    /// // Adding a complex object
    /// var complexFilter = new JObject { ["_id"] = ObjectId.Parse(userId) };
    /// var hobby = new JObject
    /// {
    ///     ["name"] = "reading",
    ///     ["level"] = "intermediate",
    ///     ["categories"] = new JArray { "fiction", "science" },
    ///     ["metadata"] = new JObject
    ///     {
    ///         ["addedAt"] = DateTime.UtcNow,
    ///         ["source"] = "user_input"
    ///     }
    /// };
    ///
    /// mongoHelper.ArrayPush(
    ///     collection: "users",
    ///     filter: complexFilter,
    ///     arrayPath: "profile.hobbies",
    ///     value: hobby,
    ///     out long complexModifiedCount
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/push/">MongoDB $push Documentation</seealso>
    /// <seealso href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">Newtonsoft.Json.Linq.JObject Documentation</seealso>
    /// <seealso href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JToken.htm">Newtonsoft.Json.Linq.JToken Documentation</seealso>
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

    /// <summary>
    /// Asynchronously updates an array element at a specific index in a MongoDB document.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the document.</param>
    /// <param name="arrayPath">The path to the array field (e.g., "items" or "profile.hobbies").</param>
    /// <param name="index">The zero-based index of the element to update.</param>
    /// <param name="value">The new value for the array element.</param>
    /// <param name="callback">Optional callback to process the modified count before returning.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>A tuple containing the helper instance and the modified count.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <remarks>
    /// Uses MongoDB's positional $ operator for direct index access.
    ///
    /// > [!IMPORTANT]
    /// > Ensure the index exists to avoid creating sparse arrays.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject { ["_id"] = ObjectId.Parse(userId) };
    /// var updatedPhone = new JObject
    /// {
    ///     ["type"] = "mobile",
    ///     ["number"] = "+1-555-0123",
    ///     ["verified"] = true,
    ///     ["updatedAt"] = DateTime.UtcNow
    /// };
    ///
    /// mongoHelper.ArraySet(
    ///     collection: "users",
    ///     filter: filter,
    ///     arrayPath: "contact.phones",
    ///     index: 0,
    ///     value: updatedPhone,
    ///     out long modifiedCount
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/positional/">MongoDB Positional $ Operator</seealso>
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

    /// <summary>
    /// Asynchronously updates array elements that match specific criteria in a MongoDB document.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the document.</param>
    /// <param name="arrayPath">The path to the array field (e.g., "items" or "profile.hobbies").</param>
    /// <param name="arrayFilter">The criteria to identify array elements to update.</param>
    /// <param name="value">The new value for matching array elements.</param>
    /// <param name="callback">Optional callback to process the modified count before returning.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>A tuple containing the helper instance and the modified count.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <remarks>
    /// Uses MongoDB's arrayFilters for conditional array updates.
    ///
    /// > [!TIP]
    /// > Use dot notation in arrayFilter to match nested properties.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject { ["_id"] = ObjectId.Parse(userId) };
    /// var arrayFilter = new JObject { ["elem.type"] = "work" };
    /// var updatedAddress = new JObject
    /// {
    ///     ["type"] = "work",
    ///     ["street"] = "123 Business Ave",
    ///     ["city"] = "Enterprise City",
    ///     ["verified"] = true,
    ///     ["updatedAt"] = DateTime.UtcNow
    /// };
    ///
    /// mongoHelper.ArrayUpdate(
    ///     collection: "users",
    ///     filter: filter,
    ///     arrayPath: "addresses",
    ///     arrayFilter: arrayFilter,
    ///     value: updatedAddress,
    ///     out long modifiedCount
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/positional-filtered/">MongoDB Filtered Positional Operator</seealso>
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

    /// <summary>
    /// Asynchronously removes elements from an array that match specific criteria in a MongoDB document.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the document.</param>
    /// <param name="arrayPath">The path to the array field (e.g., "items" or "profile.hobbies").</param>
    /// <param name="condition">The criteria to identify array elements to remove.</param>
    /// <param name="callback">Optional callback to process the modified count before returning.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>A tuple containing the helper instance and the modified count.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <remarks>
    /// Uses MongoDB's $pull operator for conditional array element removal.
    ///
    /// > [!NOTE]
    /// > Removes all matching elements in a single operation.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject { ["_id"] = ObjectId.Parse(userId) };
    /// var condition = new JObject
    /// {
    ///     ["type"] = "temporary",
    ///     ["expiresAt"] = new JObject { ["$lt"] = DateTime.UtcNow }
    /// };
    ///
    /// mongoHelper.ArrayPull(
    ///     collection: "users",
    ///     filter: filter,
    ///     arrayPath: "accessTokens",
    ///     condition: condition,
    ///     out long modifiedCount
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/pull/">MongoDB $pull Documentation</seealso>
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

    /// <summary>
    /// Asynchronously removes an array element at a specific index in a MongoDB document.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the document.</param>
    /// <param name="arrayPath">The path to the array field (e.g., "items" or "profile.hobbies").</param>
    /// <param name="index">The zero-based index of the element to remove.</param>
    /// <param name="callback">Optional callback to process the modified count before returning.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>A tuple containing the helper instance and the modified count.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <remarks>
    /// Uses a two-step process: unset the element, then pull null values.
    ///
    /// > [!IMPORTANT]
    /// > This operation maintains array integrity by removing the gap.
    ///
    /// > [!WARNING]
    /// > Ensure the index exists to avoid errors.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject { ["_id"] = ObjectId.Parse(userId) };
    ///
    /// mongoHelper.ArrayRemoveAt(
    ///     collection: "users",
    ///     filter: filter,
    ///     arrayPath: "notifications",
    ///     index: 0,
    ///     out long modifiedCount
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/unset/">MongoDB $unset Documentation</seealso>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/pull/">MongoDB $pull Documentation</seealso>
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

    /// <summary>
    /// Asynchronously updates all elements in an array field with a new value in a MongoDB document.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the document.</param>
    /// <param name="arrayPath">The path to the array field (e.g., "items" or "profile.hobbies").</param>
    /// <param name="value">The new value to replace all array elements.</param>
    /// <param name="callback">Optional callback to process the modified count before returning.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>A tuple containing the helper instance and the modified count.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the target field is not an array.</exception>
    /// <remarks>
    /// Replaces all elements in the target array with the provided value.
    ///
    /// > [!IMPORTANT]
    /// > This operation will fail if the target field is not an array.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject { ["_id"] = ObjectId.Parse(userId) };
    /// var newPermissions = new JArray { "read", "write", "admin" };
    ///
    /// await mongoHelper.ArrayReplaceAllAsync(
    ///     collection: "users",
    ///     filter: filter,
    ///     arrayPath: "permissions",
    ///     value: newPermissions
    /// );
    /// </code>
    /// </remarks>
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

    /// <summary>
    /// Synchronously removes specified fields from documents in a MongoDB collection.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the documents.</param>
    /// <param name="fields">Array of field paths to remove (e.g., "profile.address", "metadata.createdAt").</param>
    /// <param name="callback">Optional callback to process the modified count before returning.</param>
    /// <returns>A tuple containing the helper instance and the modified count.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <remarks>
    /// Uses MongoDB's $unset operator to remove specified fields.
    ///
    /// > [!NOTE]
    /// > Removing non-existent fields will not cause an error.
    ///
    /// > [!TIP]
    /// > Use dot notation for nested fields.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject { ["_id"] = ObjectId.Parse(userId) };
    /// var fieldsToRemove = new[] {
    ///     "temporaryData",
    ///     "profile.oldAddress",
    ///     "metadata.deletedAt"
    /// };
    ///
    /// var (helper, count) = mongoHelper.RemoveFields(
    ///     collection: "users",
    ///     filter: filter,
    ///     fields: fieldsToRemove,
    ///     callback: count => Console.WriteLine($"Modified {count} documents")
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/unset/">MongoDB $unset Documentation</seealso>
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

    /// <summary>
    /// Asynchronously removes specified fields from documents in a MongoDB collection.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the documents.</param>
    /// <param name="fields">Array of field paths to remove (e.g., "profile.address", "metadata.createdAt").</param>
    /// <param name="callback">Optional callback to process the modified count before returning.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>A tuple containing the helper instance and the modified count.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <remarks>
    /// Uses MongoDB's $unset operator to remove specified fields.
    ///
    /// > [!NOTE]
    /// > Removing non-existent fields will not cause an error.
    ///
    /// > [!TIP]
    /// > Use dot notation for nested fields.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject { ["_id"] = ObjectId.Parse(userId) };
    /// var fieldsToRemove = new[] {
    ///     "temporaryData",
    ///     "profile.oldAddress",
    ///     "metadata.deletedAt"
    /// };
    ///
    /// var (helper, count) = await mongoHelper.RemoveFieldsAsync(
    ///     collection: "users",
    ///     filter: filter,
    ///     fields: fieldsToRemove,
    ///     callback: count => Console.WriteLine($"Modified {count} documents")
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/unset/">MongoDB $unset Documentation</seealso>
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

    /// <summary>
    /// Synchronously retrieves specific fields from documents matching the filter criteria.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the documents.</param>
    /// <param name="paths">Array of field paths to include in the result (e.g., "profile.name", "metadata.createdAt").</param>
    /// <param name="results">Output parameter containing the retrieved documents with specified fields.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <remarks>
    /// Uses MongoDB's projection to retrieve only specified fields.
    ///
    /// > [!TIP]
    /// > Include "_id" in paths if you need document identifiers.
    ///
    /// > [!NOTE]
    /// > Non-existent paths will be excluded from results.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject { ["status"] = "active" };
    /// var paths = new[] {
    ///     "_id",
    ///     "profile.name",
    ///     "contact.email",
    ///     "metadata.lastLogin"
    /// };
    ///
    /// mongoHelper.FindPaths(
    ///     collection: "users",
    ///     filter: filter,
    ///     paths: paths,
    ///     out List&lt;JObject&gt; results
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/tutorial/project-fields-from-query-results/">MongoDB Projection Documentation</seealso>
    public MongoHelper FindPaths(
        string collection,
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
                .GetCollection<BsonDocument>(collection)
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

    /// <summary>
    /// Asynchronously retrieves specific fields from documents matching the filter criteria.
    /// </summary>
    /// <param name="collection">The name of the target collection.</param>
    /// <param name="filter">The query filter to identify the documents.</param>
    /// <param name="paths">Array of field paths to include in the result (e.g., "profile.name", "metadata.createdAt").</param>
    /// <param name="callback">Optional callback to process the results before returning.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>A tuple containing the helper instance and the retrieved documents.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when attempting to use a disposed instance.</exception>
    /// <exception cref="MongoException">Thrown when MongoDB operations fail.</exception>
    /// <remarks>
    /// Uses MongoDB's projection to retrieve only specified fields.
    ///
    /// > [!TIP]
    /// > Include "_id" in paths if you need document identifiers.
    ///
    /// > [!NOTE]
    /// > Non-existent paths will be excluded from results.
    ///
    /// Example usage:
    /// <code>
    /// var filter = new JObject { ["status"] = "active" };
    /// var paths = new[] {
    ///     "_id",
    ///     "profile.name",
    ///     "contact.email",
    ///     "metadata.lastLogin"
    /// };
    ///
    /// var (helper, results) = await mongoHelper.FindPathsAsync(
    ///     collection: "users",
    ///     filter: filter,
    ///     paths: paths,
    ///     callback: docs => Console.WriteLine($"Retrieved {docs.Count} documents")
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/tutorial/project-fields-from-query-results/">MongoDB Projection Documentation</seealso>
    public async Task<(MongoHelper helper, List<JObject> results)> FindPathsAsync(
        string collection,
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
                .GetCollection<BsonDocument>(collection)
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
}
