using IDC.Template.Utilities;
using IDC.Utilities;
using IDC.Utilities.Data;
using IDC.Utilities.Data.MongoDB;
using IDC.Utilities.Extensions;
using IDC.Utilities.Models.API;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Main", IgnoreApi = false)]
[Tags(tags: "MongoDB")]
public class MongoController(
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
    /// Finds documents in MongoDB collection with specified field projections
    /// </summary>
    /// <remarks>
    /// Retrieves documents from a MongoDB collection with the ability to filter results and specify which fields to include.
    /// The _id field is automatically excluded from the results.
    ///
    /// Example request:
    /// <code>
    /// POST /api/Mongo/Collection/users/paths
    /// {
    ///   "filter": { "age": { "$gt": 25 } },
    ///   "paths": ["name", "email", "age"]
    /// }
    /// </code>
    /// </remarks>
    /// <param name="collectionName">Name of the MongoDB collection</param>
    /// <param name="id"></param>
    /// <param name="request">Request object containing filter and paths for document projection</param>
    /// <param name="cancellationToken">Token for cancelling the operation</param>
    /// <returns>List of documents matching the filter with specified field projections</returns>
    /// <exception cref="ArgumentNullException">Thrown when collection name is null or empty</exception>
    /// <exception cref="ArgumentException">Thrown when a path is null or whitespace</exception>
    /// <exception cref="ObjectDisposedException">Thrown when MongoDB helper is disposed</exception>
    [HttpPost("{collectionName}/{id}/GetProps")]
    public async Task<APIResponseData<List<JObject>?>> FindPaths(
        [FromRoute] string collectionName,
        [FromRoute] int id,
        [FromBody] FindPathsRequest? request,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            if (request == null)
                request = new(Filter: [], Paths: null);

            request = request with { Filter = request.Filter ?? [] };
            request.Filter["_id"] = id.ToString();

            var (helper, results) = await _mongoHelper
                .InitCollection(collectionName)
                .FindPathsAsync(
                    filter: request?.Filter != null
                        ? BsonDocument.Parse(request.Filter.ToString())
                        : null,
                    paths: request?.Paths,
                    cancellationToken: cancellationToken
                );

            return new APIResponseData<List<JObject>?>()
                .ChangeStatus(language: _language, key: "api.status.success")
                .ChangeData(
                    data: results != null ? await results.ToJObject(cancellationToken) : null
                );
        }
        catch (Exception ex)
        {
            return new APIResponseData<List<JObject>?>()
                .ChangeStatus(language: _language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: _systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Request model for finding documents with field projections
    /// </summary>
    /// <param name="Filter"> MongoDB filter in BSON format </param>
    /// <param name="Paths"> Array of field paths to include in the results </param>
    public record FindPathsRequest(JObject? Filter, string[]? Paths);
}
