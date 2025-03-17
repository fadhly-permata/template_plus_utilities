using IDC.Template.Utilities;
using IDC.Utilities.Models.API;
using Microsoft.AspNetCore.Mvc;

namespace IDC.Template.Controllers;

public partial class DemoController
{
    /// <summary>
    /// Gets a cached value by key, if not exists creates new one
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="value">Value to cache if key not exists</param>
    /// <returns>Cached value</returns>
    [Tags(tags: "Caches"), HttpPost(template: "Cache/{key}")]
    public APIResponseData<object?> Get([FromRoute] string key, [FromBody] object? value)
    {
        try
        {
            return new APIResponseData<object?>().ChangeData(
                cache?.Get<object?>(key: key) ?? cache?.Set(key: key, value: value)
            );
        }
        catch (Exception ex)
        {
            return new APIResponseData<object?>()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Gets a cached value by key
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <returns>Cached value if exists</returns>
    [Tags(tags: "Caches"), HttpGet(template: "Cache/{key}")]
    public APIResponseData<object?> Get([FromRoute] string key)
    {
        try
        {
            return new APIResponseData<object?>().ChangeData(cache?.Get<object?>(key: key));
        }
        catch (Exception ex)
        {
            return new APIResponseData<object?>()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    /// <summary>
    /// Updates or creates a cached value
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="value">Value to cache</param>
    /// <returns>True if operation successful</returns>
    [Tags(tags: "Caches"), HttpPut(template: "Cache/{key}")]
    public APIResponseData<bool> Upsert([FromRoute] string key, [FromBody] object value)
    {
        try
        {
            return new APIResponseData<bool>().ChangeData(
                cache?.Set(key: key, value: value) != null
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
    /// Removes a cached value
    /// </summary>
    /// <param name="key">Cache key to remove</param>
    /// <returns>True if removal successful</returns>
    [Tags(tags: "Caches"), HttpDelete(template: "Cache/{key}")]
    public APIResponseData<bool> Remove([FromRoute] string key)
    {
        try
        {
            return new APIResponseData<bool>().ChangeData(cache?.Remove(key: key) ?? false);
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
    /// Gets all cached key-value pairs that haven't expired
    /// </summary>
    /// <returns>Dictionary of cache keys and their values</returns>
    [Tags(tags: "Caches"), HttpGet(template: "Cache")]
    public APIResponseData<Dictionary<string, object?>> GetAll()
    {
        try
        {
            return new APIResponseData<Dictionary<string, object?>>().ChangeData(
                cache?.GetAll() ?? []
            );
        }
        catch (Exception ex)
        {
            return new APIResponseData<Dictionary<string, object?>>()
                .ChangeStatus(language: language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }
}
