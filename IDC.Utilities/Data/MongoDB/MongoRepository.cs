using Newtonsoft.Json.Linq;

namespace IDC.Utilities.Data;

/// <summary>
/// Generic repository class for MongoDB operations.
/// </summary>
/// <typeparam name="T">The type of documents stored in the collection.</typeparam>
/// <remarks>
/// Provides a high-level abstraction for MongoDB CRUD operations using <see cref="MongoHelper"/>.
/// Handles automatic ID generation and document serialization/deserialization.
///
/// Example:
/// <code>
/// var repo = new MongoRepository&lt;User&gt;(mongoHelper, "users");
/// var user = new User { Name = "John" };
/// string id = repo.InsertOne(user);
/// </code>
/// </remarks>
/// <seealso href="https://www.mongodb.com/docs/drivers/csharp/current/">MongoDB .NET Driver</seealso>
public partial class MongoRepository<T>(MongoHelper mongoHelper, string collectionName)
{
    private readonly MongoHelper _mongoHelper =
        mongoHelper ?? throw new ArgumentNullException(paramName: nameof(mongoHelper));
    private readonly string _collectionName =
        collectionName ?? throw new ArgumentNullException(paramName: nameof(collectionName));

    /// <summary>
    /// Inserts a single document into the collection.
    /// </summary>
    /// <param name="document">The document to insert.</param>
    /// <param name="customId">Optional custom ID for the document.</param>
    /// <returns>The ID of the inserted document.</returns>
    /// <exception cref="ArgumentNullException">Thrown when document is null.</exception>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.collection.insertOne/">MongoDB insertOne Operation</seealso>
    public string InsertOne(T document, string? customId = null)
    {
        ArgumentNullException.ThrowIfNull(argument: document);
        var jsonDoc = JObject.FromObject(o: document);

        if (customId != null)
        {
            jsonDoc["_id"] = customId;
            _mongoHelper.InsertOne(
                collection: _collectionName,
                document: jsonDoc,
                insertedId: out string insertedId1
            );
            return insertedId1;
        }

        var nextId = _mongoHelper.GetIncrementId(collection: _collectionName);
        jsonDoc["_id"] = nextId.ToString();
        _mongoHelper.InsertOne(
            collection: _collectionName,
            document: jsonDoc,
            insertedId: out string insertedId2
        );
        return insertedId2;
    }

    /// <summary>
    /// Updates a single document in the collection.
    /// </summary>
    /// <param name="filter">The filter to identify the document to update.</param>
    /// <param name="update">The update operations to apply.</param>
    /// <returns>The number of documents modified.</returns>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.collection.updateOne/">MongoDB updateOne Operation</seealso>
    public long UpdateOne(JObject filter, JObject update)
    {
        _mongoHelper.UpdateOne(
            collection: _collectionName,
            filter: filter,
            update: update,
            modifiedCount: out long modifiedCount
        );
        return modifiedCount;
    }

    /// <summary>
    /// Deletes a single document from the collection.
    /// </summary>
    /// <param name="filter">The filter to identify the document to delete.</param>
    /// <returns>The number of documents deleted.</returns>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.collection.deleteOne/">MongoDB deleteOne Operation</seealso>
    public long DeleteOne(JObject filter)
    {
        _mongoHelper.DeleteOne(
            collection: _collectionName,
            filter: filter,
            deletedCount: out long deletedCount
        );
        return deletedCount;
    }

    /// <summary>
    /// Finds all documents matching the filter.
    /// </summary>
    /// <param name="filter">The filter criteria.</param>
    /// <returns>List of matching documents.</returns>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.collection.find/">MongoDB find Operation</seealso>
    public List<T> Find(JObject filter)
    {
        _mongoHelper.Find(
            collection: _collectionName,
            filter: filter,
            results: out List<JObject> results
        );
        return
        [
            .. results
                .Select(selector: static x => x.ToObject<T>()!)
                .Where(predicate: static x => x is not null)
        ];
    }

    /// <summary>
    /// Finds a single document matching the filter.
    /// </summary>
    /// <param name="filter">The filter criteria.</param>
    /// <returns>The matching document or null if not found.</returns>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.collection.findOne/">MongoDB findOne Operation</seealso>
    public T? FindOne(JObject filter)
    {
        _mongoHelper.FindOne(
            collection: _collectionName,
            filter: filter,
            result: out JObject? result
        );
        return result != null ? result.ToObject<T>() : default;
    }

    /// <summary>
    /// Updates an existing document or inserts a new one if it doesn't exist.
    /// </summary>
    /// <param name="filter">The filter to identify the document.</param>
    /// <param name="document">The document to upsert.</param>
    /// <param name="customId">Optional custom ID for new documents.</param>
    /// <returns>True if document was inserted, false if updated.</returns>
    /// <exception cref="ArgumentNullException">Thrown when document is null.</exception>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.collection.update/#upsert-option">MongoDB Upsert Operation</seealso>
    public bool Upsert(JObject filter, T document, string? customId = null)
    {
        var existing = FindOne(filter: filter);
        if (existing == null)
        {
            InsertOne(document: document, customId: customId);
            return true;
        }

        ArgumentNullException.ThrowIfNull(argument: document);
        var update = new JObject { ["$set"] = JObject.FromObject(o: document) };
        UpdateOne(filter: filter, update: update);
        return false;
    }

    /// <summary>
    /// Creates a new collection.
    /// </summary>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.createCollection/">MongoDB createCollection Operation</seealso>
    public void CreateCollection()
    {
        _mongoHelper.CreateCollection(collectionName: _collectionName);
    }

    /// <summary>
    /// Drops the collection.
    /// </summary>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/method/db.collection.drop/">MongoDB drop Operation</seealso>
    public void DropCollection()
    {
        _mongoHelper.DropCollection(collectionName: _collectionName);
    }

    /// <summary>
    /// Checks if the collection exists.
    /// </summary>
    /// <returns>True if collection exists, false otherwise.</returns>
    public bool CollectionExists()
    {
        return _mongoHelper.CollectionExists(collectionName: _collectionName);
    }

    /// <summary>
    /// Begins a new transaction.
    /// </summary>
    /// <returns>Current repository instance for chaining.</returns>
    /// <seealso href="https://www.mongodb.com/docs/manual/core/transactions/">MongoDB Transactions</seealso>
    public MongoRepository<T> BeginTransaction()
    {
        _mongoHelper.TransactionBegin();
        return this;
    }

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    /// <returns>Current repository instance for chaining.</returns>
    /// <seealso href="https://www.mongodb.com/docs/manual/core/transactions/">MongoDB Transactions</seealso>
    public MongoRepository<T> CommitTransaction()
    {
        _mongoHelper.TransactionCommit();
        return this;
    }

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    /// <returns>Current repository instance for chaining.</returns>
    /// <seealso href="https://www.mongodb.com/docs/manual/core/transactions/">MongoDB Transactions</seealso>
    public MongoRepository<T> RollbackTransaction()
    {
        _mongoHelper.TransactionRollback();
        return this;
    }
}
