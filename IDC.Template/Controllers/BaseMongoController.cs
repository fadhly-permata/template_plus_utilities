using IDC.Template.Utilities;
using IDC.Utilities;
using IDC.Utilities.Data;
using IDC.Utilities.Extensions;
using IDC.Utilities.Models.API;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace IDC.Template.Controllers;

/// <summary>
/// Base controller class for MongoDB operations providing common CRUD functionality.
/// </summary>
/// <remarks>
/// Provides a standardized implementation of common MongoDB operations with integrated logging and error handling.
/// This abstract class serves as a foundation for MongoDB-based controllers with the following features:
/// - Automatic request timeout handling
/// - Integrated system logging
/// - Standardized error responses
/// - Multi-language support
/// - Thread-safe operations
///
/// Example implementation:
/// <code>
/// [Route("api/[controller]")]
/// public class UsersController : BaseMongoController
/// {
///     public UsersController(
///         MongoHelper mongoHelper,
///         Language language,
///         SystemLogging systemLogging
///     ) : base(mongoHelper, language, systemLogging, "users") { }
/// }
/// </code>
/// </remarks>
/// <param name="mongoHelper">MongoDB helper instance for database operations.</param>
/// <param name="language">Language service for localized messages.</param>
/// <param name="systemLogging">Logging service for system operations.</param>
/// <seealso cref="MongoHelper"/>
/// <seealso cref="Language"/>
/// <seealso cref="SystemLogging"/>
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public abstract class BaseMongoController(
    MongoHelper mongoHelper,
    Language language,
    SystemLogging systemLogging
) : ControllerBase
{
    /// <summary>
    /// Service for handling localized messages and translations.
    /// </summary>
    private readonly Language _language = language;

    /// <summary>
    /// Service for system-wide logging operations.
    /// </summary>
    private readonly SystemLogging _systemLogging = systemLogging;

    /// <summary>
    /// Helper service for MongoDB database operations.
    /// </summary>
    private readonly MongoHelper _mongoHelper = mongoHelper;

    /// <summary>
    /// MongoDB repository instance for performing CRUD operations on documents.
    /// </summary>
    /// <remarks>
    /// This property is initialized in the constructor that takes a collection name.
    /// It provides type-safe access to MongoDB collections using <see cref="JObject"/> as the document type.
    /// </remarks>
    public required MongoRepository<JObject> Repository { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseMongoController"/> class with a specific collection name.
    /// </summary>
    /// <param name="mongoHelper">MongoDB helper instance for database operations.</param>
    /// <param name="language">Language service for localized messages.</param>
    /// <param name="systemLogging">Logging service for system operations.</param>
    /// <param name="collectionName">The name of the MongoDB collection to be used.</param>
    /// <remarks>
    /// This constructor initializes the <see cref="Repository"/> property with a new <see cref="MongoRepository{T}"/> instance.
    /// </remarks>
    protected BaseMongoController(
        MongoHelper mongoHelper,
        Language language,
        SystemLogging systemLogging,
        string collectionName
    )
        : this(mongoHelper: mongoHelper, language: language, systemLogging: systemLogging)
    {
        Repository = new(mongoHelper: _mongoHelper, collectionName: collectionName);
    }

    /// <summary>
    /// Creates a new document in the MongoDB collection.
    /// </summary>
    /// <param name="data">The document data to be inserted.</param>
    /// <param name="cancellationToken">See <see href="https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken">CancellationToken</see></param>
    /// <returns>See <see href="https://www.mongodb.com/docs/manual/crud/#write-operations">MongoDB Response</see> containing the inserted document with its new ID.</returns>
    /// <remarks>
    /// This method performs the following operations:
    /// 1. Attempts to insert the document into MongoDB collection
    /// 2. If successful, adds the generated ID to the document
    /// 3. Returns the complete document with ID
    ///
    /// > [!NOTE]
    /// > If the document contains an "_id" field, it will be used instead of generating a new one
    ///
    /// > [!IMPORTANT]
    /// > The operation can be cancelled using the cancellationToken parameter
    ///
    /// Example request:
    /// <code>
    /// POST /api/users
    /// Content-Type: application/json
    ///
    /// {
    ///     "name": "John Doe",
    ///     "email": "john@example.com",
    ///     "age": 30,
    ///     "isActive": true
    /// }
    /// </code>
    ///
    /// Example response:
    /// <code>
    /// {
    ///     "status": "success",
    ///     "message": null,
    ///     "data": {
    ///         "_id": "65f9a2e8b524d7c2a7c2f3d1",
    ///         "name": "John Doe",
    ///         "email": "john@example.com",
    ///         "age": 30,
    ///         "isActive": true
    ///     }
    /// }
    /// </code>
    /// </remarks>
    /// <exception cref="MongoDB.Driver.MongoWriteException">See <see href="https://www.mongodb.com/docs/manual/core/write-operations-atomicity/#errors-during-write-operations">MongoWriteException documentation</see> for details on write operation failures.</exception>
    /// <exception cref="MongoDB.Driver.MongoConnectionException">See <see href="https://www.mongodb.com/docs/manual/reference/connection-string/#connection-string-options">MongoConnectionException documentation</see> for details on connection failures.</exception>
    [HttpPost]
    public virtual async Task<APIResponseData<JObject>> Create(
        [FromBody] JObject data,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            data["_id"] = await Repository.InsertOneAsync(
                document: data,
                customId: data.PropGet<string?>(path: "_id", defaultValue: null),
                cancellationToken: cancellationToken,
                callback: insertedId =>
                    _systemLogging.LogInformation(
                        message: $"Document inserted with ID: {insertedId}"
                    )
            );
            return new APIResponseData<JObject>().ChangeData(data: data);
        }
        catch (Exception ex)
        {
            return new APIResponseData<JObject>()
                .ChangeStatus(language: _language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: _systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Retrieves all documents from the MongoDB collection.
    /// </summary>
    /// <param name="cancellationToken">See <see href="https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken">CancellationToken</see></param>
    /// <returns>See <see href="https://www.mongodb.com/docs/manual/crud/#read-operations">MongoDB Response</see> containing a list of all documents.</returns>
    /// <remarks>
    /// This endpoint retrieves all documents without any filtering.
    ///
    /// > [!CAUTION]
    /// > For large collections, consider implementing pagination
    ///
    /// > [!TIP]
    /// > Use query parameters for filtering and pagination in derived controllers
    ///
    /// Example request:
    /// <code>
    /// GET /api/users
    /// </code>
    ///
    /// Example response:
    /// <code>
    /// {
    ///     "status": "success",
    ///     "message": null,
    ///     "data": [
    ///         {
    ///             "_id": "65f9a2e8b524d7c2a7c2f3d1",
    ///             "name": "John Doe",
    ///             "email": "john@example.com"
    ///         },
    ///         {
    ///             "_id": "65f9a2e8b524d7c2a7c2f3d2",
    ///             "name": "Jane Smith",
    ///             "email": "jane@example.com"
    ///         }
    ///     ]
    /// }
    /// </code>
    /// </remarks>
    /// <exception cref="MongoDB.Driver.MongoConnectionException">See <see href="https://www.mongodb.com/docs/manual/reference/connection-string/#connection-string-options">MongoConnectionException documentation</see> for details on connection failures.</exception>
    [HttpGet]
    public virtual async Task<APIResponseData<List<JObject>>> GetAll(
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return new APIResponseData<List<JObject>>().ChangeData(
                data: (List<JObject>?)
                    await Repository.FindAsync(
                        filter: [],
                        cancellationToken: cancellationToken,
                        callback: results =>
                            _systemLogging.LogInformation(
                                message: $"Found {results.Count} documents"
                            )
                    )
            );
        }
        catch (Exception ex)
        {
            return new APIResponseData<List<JObject>>()
                .ChangeStatus(language: _language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: _systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Retrieves a specific document by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the document.</param>
    /// <param name="cancellationToken">See <see href="https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken">CancellationToken</see></param>
    /// <returns>See <see href="https://www.mongodb.com/docs/manual/crud/#read-operations">MongoDB Response</see> containing the requested document or null if not found.</returns>
    /// <remarks>
    /// Performs a direct lookup using the document's ID.
    ///
    /// > [!NOTE]
    /// > Returns null when document is not found instead of throwing an exception
    ///
    /// Example request:
    /// <code>
    /// GET /api/users/65f9a2e8b524d7c2a7c2f3d1
    /// </code>
    ///
    /// Example response:
    /// <code>
    /// {
    ///     "status": "success",
    ///     "message": null,
    ///     "data": {
    ///         "_id": "65f9a2e8b524d7c2a7c2f3d1",
    ///         "name": "John Doe",
    ///         "email": "john@example.com"
    ///     }
    /// }
    /// </code>
    /// </remarks>
    /// <exception cref="MongoDB.Driver.MongoConnectionException">See <see href="https://www.mongodb.com/docs/manual/reference/connection-string/#connection-string-options">MongoConnectionException documentation</see> for details on connection failures.</exception>
    [HttpGet("{id}")]
    public virtual async Task<APIResponseData<JObject?>> GetById(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return new APIResponseData<JObject?>().ChangeData(
                data: await Repository.FindOneAsync(
                    filter: new JObject { ["_id"] = id },
                    cancellationToken: cancellationToken,
                    callback: doc =>
                        _systemLogging.LogInformation(
                            message: $"Document {(doc != null ? "found" : "not found")}"
                        )
                )
            );
        }
        catch (Exception ex)
        {
            return new APIResponseData<JObject?>()
                .ChangeStatus(language: _language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: _systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Updates an entire document with new data.
    /// </summary>
    /// <param name="id">The unique identifier of the document to update.</param>
    /// <param name="data">See <see href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">JObject</see> containing the new document data.</param>
    /// <param name="cancellationToken">See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken">CancellationToken</see></param>
    /// <returns>See <see href="https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/responses/">MongoDB Response</see> containing the updated document.</returns>
    /// <remarks>
    /// Performs a full document replacement using $set operator.
    ///
    /// > [!WARNING]
    /// > This operation replaces all fields in the document
    ///
    /// Example request:
    /// <code>
    /// PUT /api/users/65f9a2e8b524d7c2a7c2f3d1
    /// Content-Type: application/json
    ///
    /// {
    ///     "name": "John Updated",
    ///     "email": "john.updated@example.com"
    /// }
    /// </code>
    ///
    /// Example response:
    /// <code>
    /// {
    ///     "status": "success",
    ///     "message": null,
    ///     "data": {
    ///         "_id": "65f9a2e8b524d7c2a7c2f3d1",
    ///         "name": "John Updated",
    ///         "email": "john.updated@example.com"
    ///     }
    /// }
    /// </code>
    /// </remarks>
    /// <exception href="https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/exceptions/#mongoconnectionexception">Thrown when connection to MongoDB fails.</exception>
    /// <seealso href="https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/exceptions/#mongoconnectionexception">MongoDB Connection Exceptions</seealso>
    [HttpPut("{id}")]
    public virtual async Task<APIResponseData<JObject?>> Update(
        string id,
        [FromBody] JObject data,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var filter = new JObject { ["_id"] = id };

            // Hapus _id dari data jika ada untuk menghindari error
            data.Remove("_id");

            await Repository.UpdateOneAsync(
                filter: filter,
                update: data,
                cancellationToken: cancellationToken,
                callback: count =>
                    _systemLogging.LogInformation(message: $"Updated {count} document(s)")
            );

            return new APIResponseData<JObject?>().ChangeData(
                data: await Repository.FindOneAsync(
                    filter: filter,
                    cancellationToken: cancellationToken
                )
            );
        }
        catch (Exception ex)
        {
            return new APIResponseData<JObject?>()
                .ChangeStatus(language: _language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: _systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Deletes a document by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the document to delete.</param>
    /// <param name="cancellationToken">See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken">CancellationToken</see></param>
    /// <returns>See <see href="https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/responses/">MongoDB Response</see> containing a boolean indicating deletion success.</returns>
    /// <remarks>
    /// Performs a permanent deletion of the document.
    ///
    /// > [!WARNING]
    /// > This operation cannot be undone
    ///
    /// Example request:
    /// <code>
    /// DELETE /api/users/65f9a2e8b524d7c2a7c2f3d1
    /// </code>
    ///
    /// Example response:
    /// <code>
    /// {
    ///     "status": "success",
    ///     "message": null,
    ///     "data": true
    /// }
    /// </code>
    /// </remarks>
    /// <exception href="https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/exceptions/#mongoconnectionexception">Thrown when connection to MongoDB fails.</exception>
    /// <seealso href="https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/exceptions/#mongoconnectionexception">MongoDB Connection Exceptions</seealso>
    [HttpDelete("{id}")]
    public virtual async Task<APIResponseData<bool>> Delete(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return new APIResponseData<bool>().ChangeData(
                data: await Repository.DeleteOneAsync(
                    filter: new JObject { ["_id"] = id },
                    cancellationToken: cancellationToken,
                    callback: deletedCount =>
                        _systemLogging.LogInformation($"Deleted {deletedCount} document(s)")
                ) > 0
            );
        }
        catch (Exception ex)
        {
            return new APIResponseData<bool>()
                .ChangeStatus(language: _language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: _systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Partially updates a document with specific fields.
    /// </summary>
    /// <param name="id">The unique identifier of the document to update.</param>
    /// <param name="data">See <see href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">JObject</see> containing the fields to update.</param>
    /// <param name="cancellationToken">See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken">CancellationToken</see></param>
    /// <returns>See <see href="https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/responses/">MongoDB Response</see> containing the updated document.</returns>
    /// <remarks>
    /// Updates only the specified fields while preserving others.
    ///
    /// > [!TIP]
    /// > Use this endpoint when you only need to update specific fields
    ///
    /// Example request:
    /// <code>
    /// PATCH /api/users/65f9a2e8b524d7c2a7c2f3d1
    /// Content-Type: application/json
    ///
    /// {
    ///     "name": "John Modified"
    /// }
    /// </code>
    ///
    /// Example response:
    /// <code>
    /// {
    ///     "status": "success",
    ///     "message": null,
    ///     "data": {
    ///         "_id": "65f9a2e8b524d7c2a7c2f3d1",
    ///         "name": "John Modified",
    ///         "email": "john@example.com"
    ///     }
    /// }
    /// </code>
    /// </remarks>
    /// <exception href="https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/exceptions/#mongoconnectionexception">Thrown when connection to MongoDB fails.</exception>
    /// <seealso href="https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/exceptions/#mongoconnectionexception">MongoDB Connection Exceptions</seealso>
    [HttpPatch("{id}")]
    public virtual async Task<APIResponseData<JObject?>> UpdateSomeProps(
        string id,
        [FromBody] JObject data,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var filter = new JObject { ["_id"] = id };

            var count = await Repository.UpdateSomePropsAsync(
                filter: filter,
                document: data,
                cancellationToken: cancellationToken,
                callback: updatedCount =>
                    _systemLogging.LogInformation($"Partially updated {updatedCount} document(s)")
            );

            if (count == 0)
                return new APIResponseData<JObject?>()
                    .ChangeStatus(language: _language, key: "api.status.not_found")
                    .ChangeMessage(language: _language, key: "api.message.document_not_found");

            var updatedDoc = await Repository.FindOneAsync(
                filter: filter,
                cancellationToken: cancellationToken
            );
            return new APIResponseData<JObject?>().ChangeData(data: updatedDoc);
        }
        catch (Exception ex)
        {
            return new APIResponseData<JObject?>()
                .ChangeStatus(language: _language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: _systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Adds a value to an array field in a document.
    /// </summary>
    /// <param name="id">The unique identifier of the document to update. See <see href="https://www.mongodb.com/docs/manual/reference/method/ObjectId/">MongoDB ObjectId</see></param>
    /// <param name="body">See <see href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">JObject</see> containing arrayPath and value</param>
    /// <param name="cancellationToken">See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken">CancellationToken</see></param>
    /// <returns>See <see cref="APIResponseData{T}"/> containing operation success status</returns>
    /// <remarks>
    /// Adds a new value to the end of an array field in a MongoDB document.
    ///
    /// > [!NOTE]
    /// > The array will be created if it doesn't exist
    ///
    /// > [!TIP]
    /// > Use this endpoint to append new elements to existing arrays
    ///
    /// Example request:
    /// <code>
    /// POST /api/users/65f9a2e8b524d7c2a7c2f3d1/array/push
    /// Content-Type: application/json
    ///
    /// {
    ///     "arrayPath": "tags",
    ///     "value": "new-tag"
    /// }
    /// </code>
    ///
    /// Example response:
    /// <code>
    /// {
    ///     "status": "success",
    ///     "message": null,
    ///     "data": true
    /// }
    /// </code>
    /// </remarks>
    /// <exception href="https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/exceptions/">MongoDB Exceptions</exception>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/push/">$push operator</seealso>
    [HttpPost("{id}/array/push")]
    public virtual async Task<APIResponseData<bool>> ArrayPush(
        string id,
        [FromBody] JObject body,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var filter = new JObject { ["_id"] = id };
            var result = await Repository.ArrayPushAsync(
                filter: filter,
                arrayPath: body["arrayPath"]!.ToString(),
                value: body["value"]!,
                cancellationToken: cancellationToken,
                callback: success =>
                    _systemLogging.LogInformation(
                        message: $"Array push operation {(success ? "succeeded" : "failed")}"
                    )
            );
            return new APIResponseData<bool>().ChangeData(data: result);
        }
        catch (Exception ex)
        {
            return new APIResponseData<bool>()
                .ChangeStatus(language: _language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: _systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Updates a specific index in an array field.
    /// </summary>
    /// <param name="id">The unique identifier of the document to update. See <see href="https://www.mongodb.com/docs/manual/reference/method/ObjectId/">MongoDB ObjectId</see></param>
    /// <param name="body">See <see href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">JObject</see> containing arrayPath, index and value</param>
    /// <param name="cancellationToken">See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken">CancellationToken</see></param>
    /// <returns>See <see cref="APIResponseData{T}"/> containing operation success status</returns>
    /// <remarks>
    /// Updates an element at a specific index in an array field.
    ///
    /// > [!IMPORTANT]
    /// > The index must be within the array bounds
    ///
    /// > [!CAUTION]
    /// > This operation will fail if the array doesn't exist
    ///
    /// Example request:
    /// <code>
    /// POST /api/users/65f9a2e8b524d7c2a7c2f3d1/array/set
    /// Content-Type: application/json
    ///
    /// {
    ///     "arrayPath": "contacts",
    ///     "index": 0,
    ///     "value": {
    ///         "type": "email",
    ///         "value": "new.email@example.com"
    ///     }
    /// }
    /// </code>
    /// </remarks>
    /// <exception href="https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/exceptions/">MongoDB Exceptions</exception>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/set/">$set operator</seealso>
    [HttpPost("{id}/array/set")]
    public virtual async Task<APIResponseData<bool>> ArraySet(
        string id,
        [FromBody] JObject body,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var filter = new JObject { ["_id"] = id };
            var result = await Repository.ArraySetAsync(
                filter: filter,
                arrayPath: body["arrayPath"]!.ToString(),
                index: body["index"]!.Value<int>(),
                value: body["value"]!,
                cancellationToken: cancellationToken,
                callback: success =>
                    _systemLogging.LogInformation(
                        message: $"Array set operation {(success ? "succeeded" : "failed")}"
                    )
            );
            return new APIResponseData<bool>().ChangeData(data: result);
        }
        catch (Exception ex)
        {
            return new APIResponseData<bool>()
                .ChangeStatus(language: _language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: _systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Updates elements in an array that match a filter condition.
    /// </summary>
    /// <param name="id">The unique identifier of the document to update. See <see href="https://www.mongodb.com/docs/manual/reference/method/ObjectId/">MongoDB ObjectId</see></param>
    /// <param name="body">See <see href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">JObject</see> containing arrayPath, arrayFilter and value</param>
    /// <param name="cancellationToken">See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken">CancellationToken</see></param>
    /// <returns>See <see cref="APIResponseData{T}"/> containing operation success status</returns>
    /// <remarks>
    /// Updates all array elements that match the specified filter condition.
    ///
    /// > [!NOTE]
    /// > Multiple elements may be updated if they match the filter
    ///
    /// > [!TIP]
    /// > Use this for bulk updates within arrays
    ///
    /// Example request:
    /// <code>
    /// POST /api/users/65f9a2e8b524d7c2a7c2f3d1/array/update
    /// Content-Type: application/json
    ///
    /// {
    ///     "arrayPath": "orders",
    ///     "arrayFilter": { "status": "pending" },
    ///     "value": { "status": "processed" }
    /// }
    /// </code>
    /// </remarks>
    /// <exception href="https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/exceptions/">MongoDB Exceptions</exception>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/positional-filtered/">$ positional filtered operator</seealso>
    [HttpPost("{id}/array/update")]
    public virtual async Task<APIResponseData<bool>> ArrayUpdate(
        string id,
        [FromBody] JObject body,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var filter = new JObject { ["_id"] = id };
            var result = await Repository.ArrayUpdateAsync(
                filter: filter,
                arrayPath: body["arrayPath"]!.ToString(),
                arrayFilter: body["arrayFilter"]!.ToObject<JObject>()!,
                value: body["value"]!,
                cancellationToken: cancellationToken,
                callback: success =>
                    _systemLogging.LogInformation(
                        message: $"Array update operation {(success ? "succeeded" : "failed")}"
                    )
            );
            return new APIResponseData<bool>().ChangeData(data: result);
        }
        catch (Exception ex)
        {
            return new APIResponseData<bool>()
                .ChangeStatus(language: _language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: _systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Removes elements from an array that match a condition.
    /// </summary>
    /// <param name="id">The unique identifier of the document to update. See <see href="https://www.mongodb.com/docs/manual/reference/method/ObjectId/">MongoDB ObjectId</see></param>
    /// <param name="body">See <see href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">JObject</see> containing arrayPath and condition</param>
    /// <param name="cancellationToken">See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken">CancellationToken</see></param>
    /// <returns>See <see cref="APIResponseData{T}"/> containing operation success status</returns>
    /// <remarks>
    /// Removes all array elements that match the specified condition.
    ///
    /// > [!WARNING]
    /// > This operation cannot be undone
    ///
    /// > [!NOTE]
    /// > Multiple elements may be removed if they match the condition
    ///
    /// Example request:
    /// <code>
    /// POST /api/users/65f9a2e8b524d7c2a7c2f3d1/array/pull
    /// Content-Type: application/json
    ///
    /// {
    ///     "arrayPath": "roles",
    ///     "condition": { "type": "temporary" }
    /// }
    /// </code>
    /// </remarks>
    /// <exception href="https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/exceptions/">MongoDB Exceptions</exception>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/pull/">$pull operator</seealso>
    [HttpPost("{id}/array/pull")]
    public virtual async Task<APIResponseData<bool>> ArrayPull(
        string id,
        [FromBody] JObject body,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var filter = new JObject { ["_id"] = id };
            var result = await Repository.ArrayPullAsync(
                filter: filter,
                arrayPath: body["arrayPath"]!.ToString(),
                condition: body["condition"]!.ToObject<JObject>()!,
                cancellationToken: cancellationToken,
                callback: success =>
                    _systemLogging.LogInformation(
                        message: $"Array pull operation {(success ? "succeeded" : "failed")}"
                    )
            );
            return new APIResponseData<bool>().ChangeData(data: result);
        }
        catch (Exception ex)
        {
            return new APIResponseData<bool>()
                .ChangeStatus(language: _language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: _systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Removes an element at a specific index from an array.
    /// </summary>
    /// <param name="id">The unique identifier of the document to update. See <see href="https://www.mongodb.com/docs/manual/reference/method/ObjectId/">MongoDB ObjectId</see></param>
    /// <param name="body">See <see href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm">JObject</see> containing arrayPath and index</param>
    /// <param name="cancellationToken">See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken">CancellationToken</see></param>
    /// <returns>See <see cref="APIResponseData{T}"/> containing operation success status</returns>
    /// <remarks>
    /// Removes a single element at the specified index from an array.
    ///
    /// > [!IMPORTANT]
    /// > The index must be within the array bounds
    ///
    /// > [!WARNING]
    /// > This operation cannot be undone
    ///
    /// Example request:
    /// <code>
    /// POST /api/users/65f9a2e8b524d7c2a7c2f3d1/array/remove
    /// Content-Type: application/json
    ///
    /// {
    ///     "arrayPath": "notifications",
    ///     "index": 0
    /// }
    /// </code>
    /// </remarks>
    /// <exception href="https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/exceptions/">MongoDB Exceptions</exception>
    /// <seealso href="https://www.mongodb.com/docs/manual/reference/operator/update/unset/">$unset operator</seealso>
    [HttpPost("{id}/array/remove")]
    public virtual async Task<APIResponseData<bool>> ArrayRemoveAt(
        string id,
        [FromBody] JObject body,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var filter = new JObject { ["_id"] = id };
            var result = await Repository.ArrayRemoveAtAsync(
                filter: filter,
                arrayPath: body["arrayPath"]!.ToString(),
                index: body["index"]!.Value<int>(),
                cancellationToken: cancellationToken,
                callback: success =>
                    _systemLogging.LogInformation(
                        message: $"Array remove operation {(success ? "succeeded" : "failed")}"
                    )
            );
            return new APIResponseData<bool>().ChangeData(data: result);
        }
        catch (Exception ex)
        {
            return new APIResponseData<bool>()
                .ChangeStatus(language: _language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: _systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }
}
