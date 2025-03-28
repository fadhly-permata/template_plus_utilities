using Newtonsoft.Json.Linq;

namespace IDC.Utilities.Data;

public partial class MongoRepository<T>
{
    /// <summary>
    /// Asynchronously inserts a single document into the collection.
    /// </summary>
    /// <param name="document">The document to insert.</param>
    /// <param name="customId">Optional custom ID for the document. If not provided, an auto-incremented ID will be used.</param>
    /// <param name="callback">Optional callback action that receives the inserted document's ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The ID of the inserted document.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
    /// <remarks>
    /// This method provides two ways to handle document IDs:
    /// 1. Using a custom ID provided by the caller
    /// 2. Using an auto-incremented ID generated by the system
    ///
    /// Example:
    /// <code>
    /// var user = new User { Name = "John Doe" };
    /// string id = await repo.InsertOneAsync(document: user);
    ///
    /// // With custom ID
    /// string customId = await repo.InsertOneAsync(
    ///     document: user,
    ///     customId: "user123"
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.collection.insertOne/">MongoDB insertOne Operation</seealso>
    /// <seealso href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">Newtonsoft.Json.Linq.JObject Class</seealso>
    public async Task<string> InsertOneAsync(
        T document,
        string? customId = null,
        Action<string>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(argument: document);
        var jsonDoc = JObject.FromObject(o: document);

        if (customId != null)
        {
            jsonDoc["_id"] = customId;
        }
        else
        {
            var (_, nextId) = await _mongoHelper.GetIncrementIdAsync(
                collection: _collectionName,
                cancellationToken: cancellationToken
            );
            jsonDoc["_id"] = nextId.ToString();
        }

        var (_, insertedId) = await _mongoHelper.InsertOneAsync(
            collection: _collectionName,
            document: jsonDoc,
            cancellationToken: cancellationToken
        );

        callback?.Invoke(obj: insertedId);
        return insertedId;
    }

    /// <summary>
    /// Asynchronously updates a single document in the collection.
    /// </summary>
    /// <param name="filter">The filter to identify the document to update.</param>
    /// <param name="update">The update operations to apply.</param>
    /// <param name="callback">Optional callback action that receives the count of modified documents.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The number of documents modified.</returns>
    /// <remarks>
    /// This method uses MongoDB's update operators to modify documents.
    ///
    /// Example:
    /// <code>
    /// var filter = new JObject { ["Name"] = "John" };
    /// var update = new JObject
    /// {
    ///     ["$set"] = new JObject { ["Age"] = 30 }
    /// };
    /// long modifiedCount = await repo.UpdateOneAsync(
    ///     filter: filter,
    ///     update: update
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.collection.updateOne/">MongoDB updateOne Operation</seealso>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/">Update Operators</seealso>
    public async Task<long> UpdateOneAsync(
        JObject filter,
        JObject update,
        Action<long>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        var (_, modifiedCount) = await _mongoHelper.UpdateOneAsync(
            collection: _collectionName,
            filter: filter,
            update: update,
            cancellationToken: cancellationToken
        );
        callback?.Invoke(obj: modifiedCount);
        return modifiedCount;
    }

    /// <summary>
    /// Asynchronously deletes a single document from the collection.
    /// </summary>
    /// <param name="filter">The filter to identify the document to delete.</param>
    /// <param name="callback">Optional callback action that receives the count of deleted documents.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The number of documents deleted.</returns>
    /// <remarks>
    /// This method removes the first document that matches the provided filter.
    ///
    /// Example:
    /// <code>
    /// var filter = new JObject { ["Name"] = "John" };
    /// long deletedCount = await repo.DeleteOneAsync(filter: filter);
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.collection.deleteOne/">MongoDB deleteOne Operation</seealso>
    public async Task<long> DeleteOneAsync(
        JObject filter,
        Action<long>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        var (_, deletedCount) = await _mongoHelper.DeleteOneAsync(
            collection: _collectionName,
            filter: filter,
            cancellationToken: cancellationToken
        );
        callback?.Invoke(obj: deletedCount);
        return deletedCount;
    }

    /// <summary>
    /// Asynchronously finds all documents in the collection that match the specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply to the query.</param>
    /// <param name="callback">Optional callback action that receives the list of found documents.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of documents that match the filter.</returns>
    /// <remarks>
    /// This method retrieves all documents matching the filter and converts them to the generic type T.
    ///
    /// Example:
    /// <code>
    /// var filter = new JObject { ["Age"] = new JObject { ["$gt"] = 25 } };
    /// List&lt;User&gt; users = await repo.FindAsync(filter: filter);
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.collection.find/">MongoDB find Operation</seealso>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/query/">Query Operators</seealso>
    public async Task<List<T>> FindAsync(
        JObject filter,
        Action<List<T>>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        var (_, results) = await _mongoHelper.FindAsync(
            collection: _collectionName,
            filter: filter,
            cancellationToken: cancellationToken
        );
        var typedResults = results.Select(x => x.ToObject<T>()!).Where(x => x is not null).ToList();
        callback?.Invoke(obj: typedResults);
        return typedResults;
    }

    /// <summary>
    /// Asynchronously finds a single document in the collection that matches the specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply to the query.</param>
    /// <param name="callback">Optional callback action that receives the found document.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The first document that matches the filter, or null if no match is found.</returns>
    /// <remarks>
    /// This method retrieves the first document matching the filter and converts it to the generic type T.
    ///
    /// Example:
    /// <code>
    /// var filter = new JObject { ["Email"] = "john@example.com" };
    /// User user = await repo.FindOneAsync(filter: filter);
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.collection.findOne/">MongoDB findOne Operation</seealso>
    public async Task<T?> FindOneAsync(
        JObject filter,
        Action<T?>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        var (_, result) = await _mongoHelper.FindOneAsync(
            collection: _collectionName,
            filter: filter,
            cancellationToken: cancellationToken
        );
        var typedResult = result is null ? default : result.ToObject<T>();
        callback?.Invoke(obj: typedResult);
        return typedResult;
    }

    /// <summary>
    /// Asynchronously inserts a new document or updates an existing one based on the filter.
    /// </summary>
    /// <param name="filter">The filter to identify the document to update.</param>
    /// <param name="document">The document to insert or update.</param>
    /// <param name="customId">Optional custom ID for new documents.</param>
    /// <param name="callback">Optional callback action that receives a boolean indicating if insertion occurred.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>True if a new document was inserted, false if an existing document was updated.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
    /// <remarks>
    /// This method first checks if a document matching the filter exists:
    /// - If no match is found, inserts a new document
    /// - If a match is found, updates the existing document
    ///
    /// Example:
    /// <code>
    /// var filter = new JObject { ["Email"] = "john@example.com" };
    /// var user = new User { Email = "john@example.com", Name = "John" };
    /// bool wasInserted = await repo.UpsertAsync(
    ///     filter: filter,
    ///     document: user
    /// );
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.collection.updateOne/">MongoDB updateOne Operation</seealso>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.collection.insertOne/">MongoDB insertOne Operation</seealso>
    public async Task<bool> UpsertAsync(
        JObject filter,
        T document,
        string? customId = null,
        Action<bool>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(argument: document);
        var existing = await FindOneAsync(filter, callback: null, cancellationToken);
        bool result;

        if (existing == null)
        {
            await InsertOneAsync(
                document: document,
                customId: customId,
                callback: null,
                cancellationToken: cancellationToken
            );
            result = true;
        }
        else
        {
            var update = new JObject { ["$set"] = JObject.FromObject(o: document) };
            await UpdateOneAsync(
                filter: filter,
                update: update,
                callback: null,
                cancellationToken: cancellationToken
            );
            result = false;
        }

        callback?.Invoke(obj: result);
        return result;
    }

    public async Task<long> UpdateSomePropsAsync(
        JObject filter,
        T document,
        Action<long>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(argument: document);
        var jsonDoc = JObject.FromObject(o: document);
        var flattenedUpdates = new Dictionary<string, object?>();

        void FlattenObject(JObject obj, string prefix = "")
        {
            foreach (var prop in obj.Properties())
            {
                var key = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";
                if (prop.Value is JObject nested)
                    FlattenObject(nested, key);
                else
                    flattenedUpdates[key] = prop.Value?.ToObject<object>();
            }
        }

        FlattenObject(jsonDoc);
        var update = new JObject
        {
            ["$set"] = new JObject(
                flattenedUpdates.Select(x => new JProperty(
                    x.Key,
                    x.Value != null ? JToken.FromObject(x.Value) : null
                ))
            )
        };

        return await UpdateOneAsync(
            filter: filter,
            update: update,
            callback: callback,
            cancellationToken: cancellationToken
        );
    }

    public async Task<bool> ArrayPushAsync(
        JObject filter,
        string arrayPath,
        JToken value,
        Action<bool>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        var (_, modifiedCount) = await _mongoHelper.ArrayPushAsync(
            collection: _collectionName,
            filter: filter,
            arrayPath: arrayPath,
            value: value,
            cancellationToken: cancellationToken
        );
        var result = modifiedCount > 0;
        callback?.Invoke(obj: result);
        return result;
    }

    public async Task<bool> ArraySetAsync(
        JObject filter,
        string arrayPath,
        int index,
        JToken value,
        Action<bool>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        var (_, modifiedCount) = await _mongoHelper.ArraySetAsync(
            collection: _collectionName,
            filter: filter,
            arrayPath: arrayPath,
            index: index,
            value: value,
            cancellationToken: cancellationToken
        );
        var result = modifiedCount > 0;
        callback?.Invoke(obj: result);
        return result;
    }

    public async Task<bool> ArrayUpdateAsync(
        JObject filter,
        string arrayPath,
        JObject arrayFilter,
        JToken value,
        Action<bool>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        var (_, modifiedCount) = await _mongoHelper.ArrayUpdateAsync(
            collection: _collectionName,
            filter: filter,
            arrayPath: arrayPath,
            arrayFilter: arrayFilter,
            value: value,
            cancellationToken: cancellationToken
        );
        var result = modifiedCount > 0;
        callback?.Invoke(obj: result);
        return result;
    }

    public async Task<bool> ArrayPullAsync(
        JObject filter,
        string arrayPath,
        JObject condition,
        Action<bool>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        var (_, modifiedCount) = await _mongoHelper.ArrayPullAsync(
            collection: _collectionName,
            filter: filter,
            arrayPath: arrayPath,
            condition: condition,
            cancellationToken: cancellationToken
        );
        var result = modifiedCount > 0;
        callback?.Invoke(obj: result);
        return result;
    }

    public async Task<bool> ArrayRemoveAtAsync(
        JObject filter,
        string arrayPath,
        int index,
        Action<bool>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        var (_, modifiedCount) = await _mongoHelper.ArrayRemoveAtAsync(
            collection: _collectionName,
            filter: filter,
            arrayPath: arrayPath,
            index: index,
            cancellationToken: cancellationToken
        );
        var result = modifiedCount > 0;
        callback?.Invoke(obj: result);
        return result;
    }
}
