using IDC.Template.Utilities;
using IDC.Utilities.Models.API;
using Microsoft.AspNetCore.Mvc;

namespace IDC.Template.Controllers;

public partial class DemoController
{
    /// <summary>
    /// Gets all available languages
    /// </summary>
    /// <returns>Array of language codes</returns>
    [HttpGet("Languages")]
    public APIResponseData<string[]> Get()
    {
        try
        {
            return new APIResponseData<string[]>().ChangeData(language.GetAvailableLanguages());
        }
        catch (Exception ex)
        {
            return new APIResponseData<string[]>()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Gets a message by path and language
    /// </summary>
    /// <param name="path">Message path in dot notation</param>
    /// <param name="lang">Optional language code</param>
    /// <returns>Localized message</returns>
    [HttpGet("Languages/{path}")]
    public APIResponseData<string> GetMessage(
        [FromRoute] string path,
        [FromQuery] string? lang = null
    )
    {
        try
        {
            return new APIResponseData<string>().ChangeData(
                language.GetMessage(path: path, language: lang)
            );
        }
        catch (Exception ex)
        {
            return new APIResponseData<string>()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Updates or adds a message
    /// </summary>
    /// <param name="lang">Language code</param>
    /// <param name="path">Message path</param>
    /// <param name="value">New message value</param>
    [HttpPut("Languages/{lang}/{path}")]
    public APIResponseData<bool> UpdateMessage(
        [FromRoute] string lang,
        [FromRoute] string path,
        [FromBody] string value
    )
    {
        try
        {
            return new APIResponseData<bool>().ChangeData(
                language.UpdateMessage(language: lang, path: path, value: value)
            );
        }
        catch (Exception ex)
        {
            return new APIResponseData<bool>()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Reloads messages from file
    /// </summary>
    [HttpPost("Languages/reload")]
    public APIResponseData<bool> Reload()
    {
        try
        {
            return new APIResponseData<bool>().ChangeData(language.Reload());
        }
        catch (Exception ex)
        {
            return new APIResponseData<bool>()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }
}
