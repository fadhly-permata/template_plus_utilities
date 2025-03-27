using IDC.Utilities;
using IDC.Utilities.Data;
using IDC.Utilities.Data.MongoDB;
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
}
