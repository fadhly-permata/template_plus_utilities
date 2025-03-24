using IDC.Template.Utilities;
using IDC.Utilities;
using IDC.Utilities.Data;
using IDC.Utilities.Data.MongoDB;
using IDC.Utilities.Models.API;
using IDC.Utilities.Models.Data;
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
/// - Automatic resource cleanup
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
public abstract class MongoController(
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
    public required Repository<JObject> Repository { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoController"/> class with a specific collection name.
    /// </summary>
    /// <param name="mongoHelper">MongoDB helper instance for database operations.</param>
    /// <param name="language">Language service for localized messages.</param>
    /// <param name="systemLogging">Logging service for system operations.</param>
    /// <param name="collectionName">The name of the MongoDB collection to be used.</param>
    /// <remarks>
    /// This constructor initializes the <see cref="Repository"/> property with a new <see cref="Repository{T}"/> instance.
    /// </remarks>
    protected MongoController(
        MongoHelper mongoHelper,
        Language language,
        SystemLogging systemLogging,
        string collectionName
    )
        : this(mongoHelper, language, systemLogging)
    {
        var services = new ServiceCollection();
        services.AddSingleton(_ =>
        {
            Repository<JObject> repository =
                new(mongoHelper: _mongoHelper, collectionName: collectionName);

            repository.InitCollection(collectionName);
            return repository;
        });

        var provider = services.BuildServiceProvider();
        Repository = provider.GetRequiredService<Repository<JObject>>();
    }

    /// <summary>
    /// Retrieves all documents from the MongoDB collection with optional field selection.
    /// </summary>
    /// <param name="paths">Optional array of field paths to include in the result (e.g., "profile.name", "metadata.createdAt").</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>See <see href="https://www.mongodb.com/docs/manual/crud/#read-operations">MongoDB Response</see> containing a list of documents.</returns>
    /// <remarks>
    /// This endpoint retrieves all documents with optional field filtering.
    ///
    /// > [!CAUTION]
    /// > For large collections, consider implementing pagination
    ///
    /// > [!TIP]
    /// > Use paths parameter to reduce response payload size by selecting specific fields
    ///
    /// Example requests:
    /// <code>
    /// // Get all fields
    /// GET /api/[controller]
    ///
    /// // Get specific fields
    /// GET /api/[controller]?paths=_id&amp;paths=name&amp;paths=email
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
        [FromQuery] string[]? paths = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return new APIResponseData<List<JObject>>().ChangeData(
                data: (List<JObject>?)
                    await Repository.GetAllAsync(
                        paths: paths,
                        cancellationToken: cancellationToken,
                        callback: null
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
}
