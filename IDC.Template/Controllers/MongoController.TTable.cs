using IDC.Utilities;
using IDC.Utilities.Data;
using Microsoft.AspNetCore.Mvc;

namespace IDC.Template.Controllers;

/// <summary>
/// Controller for managing temporary tables in MongoDB
/// </summary>
/// <remarks>
/// Provides CRUD operations for temporary table management with integrated logging and error handling.
/// Inherits base functionality from <see cref="MongoController"/>.
///
/// Example usage:
/// <code>
/// POST api/TTable
/// {
///   "name": "tempTable1",
///   "data": [
///     { "id": 1, "value": "test" }
///   ]
/// }
/// </code>
/// </remarks>
/// <param name="mongoHelper">MongoDB helper instance for database operations</param>
/// <param name="language">Language service for localized messages</param>
/// <param name="systemLogging">Logging service for system operations</param>
/// <seealso cref="MongoController"/>
/// <seealso cref="MongoHelper"/>
[Route("api/[controller]")]
[Tags(tags: "Temp Table Managements")]
[ApiExplorerSettings(IgnoreApi = false)]
public class TTableController(
    MongoHelper mongoHelper,
    Language language,
    SystemLogging systemLogging
) : MongoController(mongoHelper, language, systemLogging, "TTable") { }
