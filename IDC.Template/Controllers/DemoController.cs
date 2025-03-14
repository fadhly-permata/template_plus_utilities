using IDC.Template.Utilities.DI;
using IDC.Utilities;
using IDC.Utilities.Data;
using Microsoft.AspNetCore.Mvc;

namespace IDC.Template.Controllers;

/// <summary>
/// Controller for managing demo operations
/// </summary>
/// <remarks>
/// Provides endpoints for system logging and other demo functionalities
/// </remarks>
/// <param name="systemLogging">System logging service for handling log operations</param>
/// <param name="language">Language service for handling localization</param>
/// <param name="appConfigs">Application configuration handler</param>
/// <param name="cache">Optional caching service for handling cached data</param>
/// <param name="sqliteHelper">Optional SQLite database helper for data operations</param>
/// <example>
/// <code>
/// var controller = new DemoController(new SystemLogging());
/// controller.LogInfo(message: "Test message");
/// </code>
/// </example>
[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Demo")]
public partial class DemoController(
    AppConfigsHandler appConfigs,
    Language language,
    SystemLogging systemLogging,
    Caching? cache = null,
    SQLiteHelper? sqliteHelper = null
) : ControllerBase { }
