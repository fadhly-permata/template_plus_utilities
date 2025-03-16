using System.Net;
using IDC.Template.Utilities.DI;
using IDC.Utilities;
using IDC.Utilities.Models.API;
using Newtonsoft.Json.Linq;

namespace IDC.Template.Utilities.Middlewares;

/// <summary>
/// Middleware for API key authentication.
/// </summary>
/// <remarks>
/// This middleware validates the API key provided in the request header against a list of registered keys.
/// It handles various scenarios such as missing API key, empty registered key list, and invalid API key.
///
/// Example usage:
/// <code>
/// app.UseMiddleware&lt;ApiKeyAuthenticationMiddleware&gt;();
/// </code>
///
/// Example request:
/// <code>
/// GET /api/resource HTTP/1.1
/// Host: example.com
/// X-API-Key: your-api-key-here
/// </code>
/// </remarks>
/// <param name="appConfigs">The <see cref="AppConfigsHandler"/> for accessing application configurations.</param>
/// <param name="language">The <see cref="Language"/> service for localization.</param>
public class ApiKeyAuthenticationMiddleware(AppConfigsHandler appConfigs, Language language)
    : IMiddleware
{
    private readonly AppConfigsHandler _appConfigs = appConfigs;
    private readonly Language _language = language;
    private const string API_KEY_HEADER = "X-API-Key";

    /// <summary>
    /// Invokes the middleware to process the request.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <param name="next">The delegate representing the next middleware in the pipeline.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API key is missing, invalid, or not registered.</exception>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Headers.TryGetValue(API_KEY_HEADER, out var apiKeyHeader))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsJsonAsync(
                new APIResponse()
                    .ChangeStatus(language: _language, key: "api.status.failed")
                    .ChangeMessage(language: _language, key: "api_key.missing")
            );
            return;
        }

        string apiKey = apiKeyHeader.ToString();
        var registeredKeys =
            _appConfigs.Get<JArray>("Security.RegisteredApiKeyList")?.ToObject<string[]>() ?? [];

        if (registeredKeys.Length == 0)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(
                new APIResponse()
                    .ChangeStatus(language: _language, key: "api.status.failed")
                    .ChangeMessage(language: _language, key: "api_key.not_registered")
            );
            return;
        }

        if (!registeredKeys.Contains(apiKey))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsJsonAsync(
                new APIResponse()
                    .ChangeStatus(language: _language, key: "api.status.failed")
                    .ChangeMessage(language: _language, key: "api_key.invalid")
            );
            return;
        }

        await next(context);
    }
}
