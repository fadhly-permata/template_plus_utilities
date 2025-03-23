using MongoDB.Bson;
using MongoDB.Driver;
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
public partial class MongoRepository<T>(
    MongoHelper mongoHelper,
    string collectionName,
    IMongoCollection<BsonDocument>? collection = null
)
{
    private readonly MongoHelper _mongoHelper =
        mongoHelper ?? throw new ArgumentNullException(paramName: nameof(mongoHelper));
    private readonly string _collectionName =
        collectionName ?? throw new ArgumentNullException(paramName: nameof(collectionName));
    private readonly IMongoCollection<BsonDocument>? collection = collection;

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

    /// <summary>
    /// Updates specific fields of a document while preserving other fields.
    /// </summary>
    /// <param name="filter">The filter criteria to identify the document to update (e.g., {"_id": "2"}).</param>
    /// <param name="document">The document containing only the fields to update.</param>
    /// <returns>The number of documents that were modified by the update operation.</returns>
    /// <remarks>
    /// This method allows partial updates to documents by only modifying specified fields while preserving all other existing data.
    ///
    /// Key features:
    /// - Supports nested document updates (e.g., updating specific fields within subdocuments)
    /// - Preserves existing array values
    /// - Maintains document structure
    /// - Uses MongoDB's $set operator for atomic updates
    ///
    /// Example:
    /// <code>
    /// // Original document
    /// {
    ///   "_id": "2",
    ///   "nama": "Cepot Setiawan",
    ///   "alamat": {
    ///     "jalan": "Jl. Merdeka No. 10",
    ///     "kota": "Jakarta"
    ///   }
    /// }
    ///
    /// // Update request
    /// var filter = new JObject { ["_id"] = "2" };
    /// var update = new Student {
    ///   alamat = new Alamat {
    ///     kabupaten = "Jakarta"
    ///   }
    /// };
    ///
    /// repository.UpdateSomeProps(filter, update);
    ///
    /// // Result
    /// {
    ///   "_id": "2",
    ///   "nama": "Cepot Setiawan",
    ///   "alamat": {
    ///     "jalan": "Jl. Merdeka No. 10",
    ///     "kota": "Jakarta",
    ///     "kabupaten": "Jakarta"
    ///   }
    /// }
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when document parameter is null.</exception>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/set/">MongoDB $set Operator</seealso>
    /// <seealso href="https://www.mongodb.com/docs/manual/tutorial/update-documents/">MongoDB Update Documents</seealso>
    public long UpdateSomeProps(JObject filter, T document)
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

        return UpdateOne(filter: filter, update: update);
    }

    public bool ArrayPush(JObject filter, string arrayPath, JToken value)
    {
        _mongoHelper.ArrayPush(
            collection: _collectionName,
            filter: filter,
            arrayPath: arrayPath,
            value: value,
            out long modifiedCount
        );
        return modifiedCount > 0;
    }

    public bool ArraySet(JObject filter, string arrayPath, int index, JToken value)
    {
        _mongoHelper.ArraySet(
            collection: _collectionName,
            filter: filter,
            arrayPath: arrayPath,
            index: index,
            value: value,
            out long modifiedCount
        );
        return modifiedCount > 0;
    }

    public bool ArrayUpdate(JObject filter, string arrayPath, JObject arrayFilter, JToken value)
    {
        _mongoHelper.ArrayUpdate(
            collection: _collectionName,
            filter: filter,
            arrayPath: arrayPath,
            arrayFilter: arrayFilter,
            value: value,
            out long modifiedCount
        );
        return modifiedCount > 0;
    }

    public bool ArrayPull(JObject filter, string arrayPath, JObject condition)
    {
        _mongoHelper.ArrayPull(
            collection: _collectionName,
            filter: filter,
            arrayPath: arrayPath,
            condition: condition,
            out long modifiedCount
        );
        return modifiedCount > 0;
    }

    public bool ArrayRemoveAt(JObject filter, string arrayPath, int index)
    {
        _mongoHelper.ArrayRemoveAt(
            collection: _collectionName,
            filter: filter,
            arrayPath: arrayPath,
            index: index,
            out long modifiedCount
        );
        return modifiedCount > 0;
    }
}
