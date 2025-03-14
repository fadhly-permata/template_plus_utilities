using IDC.Utilities.Models.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json.Linq;

namespace IDC.Utilities.Data;

/// <summary>
/// Provides CRUD operations and pagination functionality for MongoDB collections.
/// </summary>
/// <remarks>
/// This repository class simplifies MongoDB operations by providing high-level methods for common database operations.
/// It handles document creation, retrieval, updates, and deletion using JObject for flexible document structure.
///
/// Example usage:
/// <code>
/// var mongo = new MongoHelper("connectionString", "databaseName");
/// var repository = new MongoRepository(mongo, "users");
///
/// // Create document
/// var user = new JObject {
///     ["name"] = "John Doe",
///     ["email"] = "john@example.com"
/// };
/// var created = repository.Create(user);
///
/// // Query documents
/// var filter = new JObject { ["email"] = "john@example.com" };
/// var result = repository.GetOne(filter);
/// </code>
/// </remarks>
public sealed class MongoRepository
{
    private readonly MongoHelper _mongo;
    private readonly string _collection;

    public MongoRepository(MongoHelper mongo, string collection)
    {
        ArgumentNullException.ThrowIfNull(argument: mongo);
        ArgumentException.ThrowIfNullOrWhiteSpace(argument: collection);

        _mongo = mongo;
        _collection = collection;
    }

    /// <summary>
    /// Creates a new document in the MongoDB collection.
    /// </summary>
    /// <param name="data">The document data as a <see cref="JObject"/>.</param>
    /// <returns>The created document with generated ObjectId.</returns>
    /// <remarks>
    /// Generates a new ObjectId and inserts the document into the collection.
    /// The method uses upsert operation to ensure document creation.
    ///
    /// Example:
    /// <code>
    /// var data = new JObject {
    ///     ["name"] = "John Doe",
    ///     ["age"] = 30
    /// };
    /// var result = repository.Create(data);
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when data is null.</exception>
    /// <see href="https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/insertOne/"/>
    public JObject Create(JObject data)
    {
        ArgumentNullException.ThrowIfNull(argument: data);

        var doc = BsonDocument.Parse(json: data.ToString());

        _mongo.ExecuteUpsert(
            collection: _collection,
            filter: Builders<BsonDocument>.Filter.Eq(field: "_id", value: ObjectId.GenerateNewId()),
            update: Builders<BsonDocument>.Update.Combine(
                updates: doc.Names.Select(selector: name =>
                    Builders<BsonDocument>.Update.Set(field: name, value: doc[name])
                )
            ),
            result: out JObject result
        );

        return result;
    }

    /// <summary>
    /// Creates a new document asynchronously in the MongoDB collection.
    /// </summary>
    /// <param name="data">The document data as a <see cref="JObject"/>.</param>
    /// <param name="callback">Optional callback action to execute after creation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The created document with generated ObjectId.</returns>
    /// <remarks>
    /// Asynchronously generates a new ObjectId and inserts the document into the collection.
    /// Supports operation cancellation and callback execution.
    ///
    /// Example:
    /// <code>
    /// var data = new JObject {
    ///     ["name"] = "John Doe",
    ///     ["age"] = 30
    /// };
    /// var result = await repository.CreateAsync(
    ///     data,
    ///     callback: doc => Console.WriteLine($"Created: {doc["_id"]}")
    /// );
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when data is null.</exception>
    /// <see href="https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/insertOne/"/>
    public async Task<JObject> CreateAsync(
        JObject data,
        Action<JObject>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(argument: data);
        var doc = BsonDocument.Parse(json: data.ToString());

        var result = new JObject();
        await _mongo.ExecuteUpsertAsync(
            collection: _collection,
            filter: Builders<BsonDocument>.Filter.Eq(field: "_id", value: ObjectId.GenerateNewId()),
            update: Builders<BsonDocument>.Update.Combine(
                updates: doc.Names.Select(selector: name =>
                    Builders<BsonDocument>.Update.Set(field: name, value: doc[name])
                )
            ),
            callback: r =>
            {
                result = r;
                callback?.Invoke(obj: r);
            },
            cancellationToken: cancellationToken
        );

        return result;
    }

    /// <summary>
    /// Retrieves all documents from the collection that match the optional filter.
    /// </summary>
    /// <param name="filter">Optional filter criteria as a <see cref="JObject"/>.</param>
    /// <returns>List of matching documents.</returns>
    /// <remarks>
    /// Returns all documents if no filter is provided, or filtered results if criteria specified.
    ///
    /// Example:
    /// <code>
    /// // Get all documents
    /// var allDocs = repository.GetAll();
    ///
    /// // Get filtered documents
    /// var filter = new JObject { ["age"] = new JObject { ["$gt"] = 25 } };
    /// var filtered = repository.GetAll(filter);
    /// </code>
    /// </remarks>
    /// <see href="https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/find/"/>
    public List<JObject> GetAll(JObject? filter = null)
    {
        var mongoFilter =
            filter == null
                ? Builders<BsonDocument>.Filter.Empty
                : BsonDocument.Parse(json: filter.ToString());

        _mongo.ExecuteQuery(
            collection: _collection,
            filter: mongoFilter,
            results: out List<JObject> results
        );

        return results;
    }

    /// <summary>
    /// Asynchronously retrieves all documents from the collection that match the optional filter.
    /// </summary>
    /// <param name="filter">Optional filter criteria as a <see cref="JObject"/>.</param>
    /// <param name="callback">Optional callback action to execute with results.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>List of matching documents.</returns>
    /// <remarks>
    /// Asynchronously returns all documents if no filter is provided, or filtered results if criteria specified.
    /// Supports operation cancellation and callback execution.
    ///
    /// Example:
    /// <code>
    /// var filter = new JObject { ["age"] = new JObject { ["$gt"] = 25 } };
    /// var results = await repository.GetAllAsync(
    ///     filter,
    ///     callback: docs => Console.WriteLine($"Found {docs.Count} documents")
    /// );
    /// </code>
    /// </remarks>
    /// <see href="https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/find/"/>
    public async Task<List<JObject>> GetAllAsync(
        JObject? filter = null,
        Action<List<JObject>>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        var mongoFilter =
            filter == null
                ? Builders<BsonDocument>.Filter.Empty
                : BsonDocument.Parse(json: filter.ToString());

        var results = new List<JObject>();
        await _mongo.ExecuteQueryAsync<BsonDocument>(
            collection: _collection,
            filter: mongoFilter,
            callback: r =>
            {
                results = r;
                callback?.Invoke(obj: r);
            },
            cancellationToken: cancellationToken
        );

        return results;
    }

    /// <summary>
    /// Retrieves a single document matching the specified filter.
    /// </summary>
    /// <param name="filter">Filter criteria as a <see cref="JObject"/>.</param>
    /// <returns>Matching document or null if not found.</returns>
    /// <remarks>
    /// Returns the first document that matches the filter criteria.
    ///
    /// Example:
    /// <code>
    /// var filter = new JObject { ["email"] = "john@example.com" };
    /// var user = repository.GetOne(filter);
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when filter is null.</exception>
    /// <see href="https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/findOne/"/>
    public JObject? GetOne(JObject filter)
    {
        ArgumentNullException.ThrowIfNull(argument: filter);

        _mongo.ExecuteQuery<BsonDocument>(
            collection: _collection,
            filter: BsonDocument.Parse(json: filter.ToString()),
            results: out List<JObject> results
        );

        return results.FirstOrDefault();
    }

    /// <summary>
    /// Asynchronously retrieves a single document matching the specified filter.
    /// </summary>
    /// <param name="filter">Filter criteria as a <see cref="JObject"/>.</param>
    /// <param name="callback">Optional callback action to execute with result.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Matching document or null if not found.</returns>
    /// <remarks>
    /// Asynchronously returns the first document that matches the filter criteria.
    /// Supports operation cancellation and callback execution.
    ///
    /// Example:
    /// <code>
    /// var filter = new JObject { ["email"] = "john@example.com" };
    /// var user = await repository.GetOneAsync(
    ///     filter,
    ///     callback: doc => Console.WriteLine(doc?["name"])
    /// );
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when filter is null.</exception>
    /// <see href="https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/findOne/"/>
    public async Task<JObject?> GetOneAsync(
        JObject filter,
        Action<JObject?>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(argument: filter);

        var results = new List<JObject>();
        await _mongo.ExecuteQueryAsync<BsonDocument>(
            collection: _collection,
            filter: BsonDocument.Parse(json: filter.ToString()),
            callback: r =>
            {
                results = r;
                callback?.Invoke(obj: r.FirstOrDefault());
            },
            cancellationToken: cancellationToken
        );

        return results.FirstOrDefault();
    }

    /// <summary>
    /// Updates a document matching the specified filter.
    /// </summary>
    /// <param name="filter">Filter to identify the document to update.</param>
    /// <param name="update">Update operations to apply.</param>
    /// <returns>The updated document.</returns>
    /// <remarks>
    /// Updates fields in the matching document according to the update specification.
    ///
    /// Example:
    /// <code>
    /// var filter = new JObject { ["_id"] = id };
    /// var update = new JObject { ["status"] = "active" };
    /// var updated = repository.Update(filter, update);
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when filter or update is null.</exception>
    /// <see href="https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/updateOne/"/>
    public JObject Update(JObject filter, JObject update)
    {
        ArgumentNullException.ThrowIfNull(argument: filter);
        ArgumentNullException.ThrowIfNull(argument: update);

        var doc = BsonDocument.Parse(json: update.ToString());

        _mongo.ExecuteUpdate(
            collection: _collection,
            filter: BsonDocument.Parse(json: filter.ToString()),
            update: Builders<BsonDocument>.Update.Combine(
                updates: doc.Names.Select(selector: name =>
                    Builders<BsonDocument>.Update.Set(field: name, value: doc[name])
                )
            ),
            result: out JObject result
        );

        return result;
    }

    /// <summary>
    /// Asynchronously updates a document matching the specified filter.
    /// </summary>
    /// <param name="filter">Filter to identify the document to update.</param>
    /// <param name="update">Update operations to apply.</param>
    /// <param name="callback">Optional callback action to execute with updated document.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The updated document.</returns>
    /// <remarks>
    /// Asynchronously updates fields in the matching document according to the update specification.
    /// Supports operation cancellation and callback execution.
    ///
    /// Example:
    /// <code>
    /// var filter = new JObject { ["_id"] = id };
    /// var update = new JObject { ["status"] = "active" };
    /// var updated = await repository.UpdateAsync(
    ///     filter,
    ///     update,
    ///     callback: doc => Console.WriteLine($"Updated: {doc["_id"]}")
    /// );
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when filter or update is null.</exception>
    /// <see href="https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/updateOne/"/>
    public async Task<JObject> UpdateAsync(
        JObject filter,
        JObject update,
        Action<JObject>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(argument: filter);
        ArgumentNullException.ThrowIfNull(argument: update);

        var doc = BsonDocument.Parse(json: update.ToString());
        var result = new JObject();

        await _mongo.ExecuteUpdateAsync(
            collection: _collection,
            filter: BsonDocument.Parse(json: filter.ToString()),
            update: Builders<BsonDocument>.Update.Combine(
                updates: doc.Names.Select(selector: name =>
                    Builders<BsonDocument>.Update.Set(field: name, value: doc[name])
                )
            ),
            callback: r =>
            {
                result = r;
                callback?.Invoke(obj: r);
            },
            cancellationToken: cancellationToken
        );

        return result;
    }

    /// <summary>
    /// Deletes a document matching the specified filter.
    /// </summary>
    /// <param name="filter">Filter to identify the document to delete.</param>
    /// <returns>True if document was deleted, false otherwise.</returns>
    /// <remarks>
    /// Removes the first document that matches the filter criteria.
    ///
    /// Example:
    /// <code>
    /// var filter = new JObject { ["_id"] = id };
    /// var deleted = repository.Delete(filter);
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when filter is null.</exception>
    /// <see href="https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/deleteOne/"/>
    public bool Delete(JObject filter)
    {
        ArgumentNullException.ThrowIfNull(argument: filter);

        _mongo.ExecuteDelete<BsonDocument>(
            collection: _collection,
            filter: BsonDocument.Parse(json: filter.ToString()),
            deletedCount: out long deletedCount
        );

        return deletedCount > 0;
    }

    /// <summary>
    /// Asynchronously deletes a document matching the specified filter.
    /// </summary>
    /// <param name="filter">Filter to identify the document to delete.</param>
    /// <param name="callback">Optional callback action to execute with deletion result.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if document was deleted, false otherwise.</returns>
    /// <remarks>
    /// Asynchronously removes the first document that matches the filter criteria.
    /// Supports operation cancellation and callback execution.
    ///
    /// Example:
    /// <code>
    /// var filter = new JObject { ["_id"] = id };
    /// var deleted = await repository.DeleteAsync(
    ///     filter,
    ///     callback: result => Console.WriteLine($"Deletion successful: {result}")
    /// );
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when filter is null.</exception>
    /// <see href="https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/deleteOne/"/>
    public async Task<bool> DeleteAsync(
        JObject filter,
        Action<bool>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(argument: filter);

        var deletedCount = 0L;
        await _mongo.ExecuteDeleteAsync<BsonDocument>(
            collection: _collection,
            filter: BsonDocument.Parse(json: filter.ToString()),
            callback: count =>
            {
                deletedCount = count;
                callback?.Invoke(obj: count > 0);
            },
            cancellationToken: cancellationToken
        );

        return deletedCount > 0;
    }

    /// <summary>
    /// Retrieves documents with pagination support.
    /// </summary>
    /// <param name="request">Pagination parameters including offset, limit, and sorting.</param>
    /// <param name="filter">Optional filter criteria as a <see cref="JObject"/>.</param>
    /// <returns>Paginated response containing total counts and matching documents.</returns>
    /// <remarks>
    /// Provides paginated access to collection documents with optional filtering and sorting.
    ///
    /// Example:
    /// <code>
    /// var request = new PaginationRequest {
    ///     Offset = 0,
    ///     Limit = 10,
    ///     Sort = "name",
    ///     Order = "asc"
    /// };
    /// var filter = new JObject { ["age"] = new JObject { ["$gt"] = 25 } };
    /// var result = repository.GetWithPaging(request, filter);
    /// </code>
    /// </remarks>
    /// <see href="https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/find/"/>
    /// <see href="https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/count/"/>
    public PaginationResponse<JObject> GetWithPaging(
        PaginationRequest request,
        JObject? filter = null
    )
    {
        var mongoFilter =
            filter == null
                ? Builders<BsonDocument>.Filter.Empty
                : BsonDocument.Parse(json: filter.ToString());

        var collection = _mongo.GetDatabase().GetCollection<BsonDocument>(name: _collection);
        var query = collection
            .Find(filter: mongoFilter)
            .Skip(skip: request.Offset)
            .Limit(limit: request.Limit);

        if (!string.IsNullOrEmpty(value: request.Sort))
        {
            query = query.Sort(
                sort: new BsonDocument(
                    name: request.Sort,
                    value: (
                        request.Order?.ToLower() == "desc"
                            ? SortDirection.Descending
                            : SortDirection.Ascending
                    ) == SortDirection.Descending
                        ? -1
                        : 1
                )
            );
        }

        return new PaginationResponse<JObject>
        {
            Total = (int)collection.CountDocuments(filter: mongoFilter),
            TotalNotFiltered = (int)
                collection.CountDocuments(filter: Builders<BsonDocument>.Filter.Empty),
            Rows =
            [
                .. query.ToList().Select(selector: static doc => JObject.Parse(json: doc.ToJson()))
            ]
        };
    }

    /// <summary>
    /// Asynchronously retrieves documents with pagination support.
    /// </summary>
    /// <param name="request">Pagination parameters including offset, limit, and sorting.</param>
    /// <param name="filter">Optional filter criteria as a <see cref="JObject"/>.</param>
    /// <param name="callback">Optional callback action to execute with paginated response.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Paginated response containing total counts and matching documents.</returns>
    /// <remarks>
    /// Asynchronously provides paginated access to collection documents with optional filtering and sorting.
    /// Supports operation cancellation and callback execution.
    ///
    /// Example:
    /// <code>
    /// var request = new PaginationRequest {
    ///     Offset = 0,
    ///     Limit = 10,
    ///     Sort = "name",
    ///     Order = "asc"
    /// };
    /// var filter = new JObject { ["age"] = new JObject { ["$gt"] = 25 } };
    /// var result = await repository.GetWithPagingAsync(
    ///     request,
    ///     filter,
    ///     callback: response => Console.WriteLine($"Found {response.Rows.Count} documents")
    /// );
    /// </code>
    /// </remarks>
    /// <see href="https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/find/"/>
    /// <see href="https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/count/"/>
    public async Task<PaginationResponse<JObject>> GetWithPagingAsync(
        PaginationRequest request,
        JObject? filter = null,
        Action<PaginationResponse<JObject>>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        var mongoFilter =
            filter == null
                ? Builders<BsonDocument>.Filter.Empty
                : BsonDocument.Parse(json: filter.ToString());

        var collection = _mongo.GetDatabase().GetCollection<BsonDocument>(name: _collection);

        var query = collection
            .Find(filter: mongoFilter)
            .Skip(skip: request.Offset)
            .Limit(limit: request.Limit);

        if (!string.IsNullOrEmpty(value: request.Sort))
        {
            query = query.Sort(
                sort: new BsonDocument(
                    name: request.Sort,
                    value: (
                        request.Order?.ToLower() == "desc"
                            ? SortDirection.Descending
                            : SortDirection.Ascending
                    ) == SortDirection.Descending
                        ? -1
                        : 1
                )
            );
        }

        var result = new PaginationResponse<JObject>
        {
            Total = (int)
                await collection.CountDocumentsAsync(
                    filter: mongoFilter,
                    cancellationToken: cancellationToken
                ),
            TotalNotFiltered = (int)
                await collection.CountDocumentsAsync(
                    filter: Builders<BsonDocument>.Filter.Empty,
                    cancellationToken: cancellationToken
                ),
            Rows =
            [
                .. (await query.ToListAsync(cancellationToken: cancellationToken)).Select(
                    selector: static doc => JObject.Parse(json: doc.ToJson())
                )
            ]
        };

        callback?.Invoke(obj: result);
        return result;
    }

    /// <summary>
    /// Creates multiple documents in bulk in the MongoDB collection.
    /// </summary>
    /// <param name="data">List of document data as <see cref="JObject"/>.</param>
    /// <returns>List of created documents with generated ObjectIds.</returns>
    /// <remarks>
    /// Generates new ObjectIds and inserts multiple documents into the collection in a single operation.
    /// The method uses bulk upsert operations to ensure document creation.
    ///
    /// Example:
    /// <code>
    /// var documents = new List&lt;JObject&gt; {
    ///     new() { ["name"] = "John Doe", ["age"] = 30 },
    ///     new() { ["name"] = "Jane Smith", ["age"] = 25 }
    /// };
    /// var results = repository.CreateBulk(documents);
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when data is null.</exception>
    /// <see href="https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/bulkWrite/"/>
    public List<JObject> CreateBulk(List<JObject> data)
    {
        ArgumentNullException.ThrowIfNull(argument: data);

        if (data.Count == 0)
            return [];

        var results = new List<JObject>();

        var collection = _mongo.GetDatabase().GetCollection<BsonDocument>(name: _collection);
        collection
            .Find(
                filter: Builders<BsonDocument>.Filter.Or(
                    filters: collection
                        .BulkWrite(
                            requests: data.Select(selector: item =>
                            {
                                var doc = BsonDocument.Parse(json: item.ToString());

                                return new UpdateOneModel<BsonDocument>(
                                    filter: Builders<BsonDocument>.Filter.Eq(
                                        field: "_id",
                                        value: ObjectId.GenerateNewId()
                                    ),
                                    update: Builders<BsonDocument>.Update.Combine(
                                        updates: doc.Names.Select(selector: name =>
                                            Builders<BsonDocument>.Update.Set(
                                                field: name,
                                                value: doc[name]
                                            )
                                        )
                                    )
                                )
                                {
                                    IsUpsert = true
                                };
                            })
                        )
                        .Upserts.Select(selector: upsert =>
                            Builders<BsonDocument>.Filter.Eq(field: "_id", value: upsert.Id)
                        )
                )
            )
            .ToList()
            .ForEach(action: doc => results.Add(item: JObject.Parse(json: doc.ToJson())));

        return results;
    }

    /// <summary>
    /// Creates multiple documents in bulk asynchronously in the MongoDB collection.
    /// </summary>
    /// <param name="data">List of document data as <see cref="JObject"/>.</param>
    /// <param name="callback">Optional callback action to execute with created documents.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>List of created documents with generated ObjectIds.</returns>
    /// <remarks>
    /// Asynchronously generates new ObjectIds and inserts multiple documents into the collection.
    /// Supports operation cancellation and callback execution.
    ///
    /// Example:
    /// <code>
    /// var documents = new List&lt;JObject&gt; {
    ///     new() { ["name"] = "John Doe", ["age"] = 30 },
    ///     new() { ["name"] = "Jane Smith", ["age"] = 25 }
    /// };
    /// var results = await repository.CreateBulkAsync(
    ///     documents,
    ///     callback: docs => Console.WriteLine($"Created {docs.Count} documents")
    /// );
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when data is null.</exception>
    /// <see href="https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/bulkWrite/"/>
    public async Task<List<JObject>> CreateBulkAsync(
        List<JObject> data,
        Action<List<JObject>>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(argument: data);

        if (data.Count == 0)
            return [];

        var results = new List<JObject>();
        var collection = _mongo.GetDatabase().GetCollection<BsonDocument>(name: _collection);

        results.AddRange(
            (
                await collection
                    .Find(
                        filter: Builders<BsonDocument>.Filter.Or(
                            filters: (
                                await collection.BulkWriteAsync(
                                    requests: data.Select(selector: item =>
                                    {
                                        var doc = BsonDocument.Parse(json: item.ToString());

                                        return new UpdateOneModel<BsonDocument>(
                                            filter: Builders<BsonDocument>.Filter.Eq(
                                                field: "_id",
                                                value: ObjectId.GenerateNewId()
                                            ),
                                            update: Builders<BsonDocument>.Update.Combine(
                                                updates: doc.Names.Select(selector: name =>
                                                    Builders<BsonDocument>.Update.Set(
                                                        field: name,
                                                        value: doc[name]
                                                    )
                                                )
                                            )
                                        )
                                        {
                                            IsUpsert = true
                                        };
                                    }),
                                    cancellationToken: cancellationToken
                                )
                            ).Upserts.Select(selector: upsert =>
                                Builders<BsonDocument>.Filter.Eq(field: "_id", value: upsert.Id)
                            )
                        )
                    )
                    .ToListAsync(cancellationToken: cancellationToken)
            ).Select(selector: static doc => JObject.Parse(json: doc.ToJson()))
        );
        callback?.Invoke(obj: results);

        return results;
    }

    /// <summary>
    /// Deletes multiple documents matching the specified filters in bulk.
    /// </summary>
    /// <param name="filters">List of filter criteria as <see cref="JObject"/>.</param>
    /// <returns>Number of documents deleted.</returns>
    /// <remarks>
    /// Removes multiple documents that match the filter criteria in a single operation.
    ///
    /// Example:
    /// <code>
    /// var filters = new List&lt;JObject&gt; {
    ///     new() { ["status"] = "inactive" },
    ///     new() { ["age"] = new JObject { ["$lt"] = 18 } }
    /// };
    /// var deletedCount = repository.DeleteBulk(filters);
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when filters is null.</exception>
    /// <see href="https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/bulkWrite/"/>
    public long DeleteBulk(List<JObject> filters)
    {
        ArgumentNullException.ThrowIfNull(argument: filters);

        if (filters.Count == 0)
            return 0;

        return _mongo
            .GetDatabase()
            .GetCollection<BsonDocument>(name: _collection)
            .BulkWrite(
                requests: filters.Select(selector: filter =>
                {
                    var mongoFilter = BsonDocument.Parse(json: filter.ToString());
                    return new DeleteManyModel<BsonDocument>(filter: mongoFilter);
                }),
                options: new BulkWriteOptions { IsOrdered = false }
            )
            .DeletedCount;
    }

    /// <summary>
    /// Deletes multiple documents matching the specified filters in bulk asynchronously.
    /// </summary>
    /// <param name="filters">List of filter criteria as <see cref="JObject"/>.</param>
    /// <param name="callback">Optional callback action to execute with deletion count.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Number of documents deleted.</returns>
    /// <remarks>
    /// Asynchronously removes multiple documents that match the filter criteria.
    /// Supports operation cancellation and callback execution.
    ///
    /// Example:
    /// <code>
    /// var filters = new List&lt;JObject&gt; {
    ///     new() { ["status"] = "inactive" },
    ///     new() { ["age"] = new JObject { ["$lt"] = 18 } }
    /// };
    /// var deletedCount = await repository.DeleteBulkAsync(
    ///     filters,
    ///     callback: count => Console.WriteLine($"Deleted {count} documents")
    /// );
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when filters is null.</exception>
    /// <see href="https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/bulkWrite/"/>
    public async Task<long> DeleteBulkAsync(
        List<JObject> filters,
        Action<long>? callback = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(argument: filters);

        if (filters.Count == 0)
            return 0;

        var deletedCount = (
            await _mongo
                .GetDatabase()
                .GetCollection<BsonDocument>(name: _collection)
                .BulkWriteAsync(
                    requests: filters.Select(selector: filter =>
                    {
                        var mongoFilter = BsonDocument.Parse(json: filter.ToString());
                        return new DeleteManyModel<BsonDocument>(filter: mongoFilter);
                    }),
                    cancellationToken: cancellationToken
                )
        ).DeletedCount;

        callback?.Invoke(obj: deletedCount);
        return deletedCount;
    }
}
