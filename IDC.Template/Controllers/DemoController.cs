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
/// <example>
/// <code>
/// var controller = new DemoController(new SystemLogging());
/// controller.LogInfo(message: "Test message");
/// </code>
/// </example>
[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Demo")]
public class DemoController() : ControllerBase { }
