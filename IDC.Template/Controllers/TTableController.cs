using IDC.Template.Utilities;
using IDC.Utilities;
using IDC.Utilities.Data;
using Microsoft.AspNetCore.Mvc;

namespace IDC.Template.Controllers;

[Route("api/[controller]")]
[Tags(tags: "Temp Table Managements")]
[ApiExplorerSettings(IgnoreApi = false)]
public class TTableController(
    MongoHelper mongoHelper,
    Language language,
    SystemLogging systemLogging
) : BaseMongoController(mongoHelper, language, systemLogging, "TTable") { }
