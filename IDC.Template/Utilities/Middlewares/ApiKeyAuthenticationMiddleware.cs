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
/// The middleware checks for the presence of an API key in the 'X-API-Key' header of incoming requests.
/// If the key is missing, invalid, or not registered, it returns an appropriate error response.
/// Otherwise, it allows the request to proceed to the next middleware in the pipeline.
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
///
/// Example response for invalid API key:
/// <code>
/// HTTP/1.1 401 Unauthorized
/// Content-Type: application/json
///
/// {
///   "status": "Failed",
///   "message": "Invalid API key"
/// }
/// </code>
/// </remarks>
/// <param name="appConfigs">The <see cref="AppConfigsHandler"/> for accessing application configurations.</param>
/// <param name="language">The <see cref="Language"/> service for localization.</param>
/// <seealso cref="IMiddleware"/>
/// <seealso cref="AppConfigsHandler"/>
/// <seealso cref="Language"/>
/// <seealso href="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/">ASP.NET Core Middleware</seealso>
/// <seealso href="https://docs.microsoft.com/en-us/aspnet/core/security/authentication/">Authentication in ASP.NET Core</seealso>
public class ApiKeyAuthenticationMiddleware(AppConfigsHandler appConfigs, Language language)
    : IMiddleware
{
    private readonly AppConfigsHandler _appConfigs = appConfigs;
    private readonly Language _language = language;
    private const string API_KEY_HEADER = "X-API-Key";

    /// <summary>
    /// Writes an error response to the HTTP context.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <param name="statusCode">The <see cref="HttpStatusCode"/> to be set in the response.</param>
    /// <param name="messageKey">The key for the error message to be localized.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method sets the response status code and writes a JSON response with a localized error message.
    ///
    /// Example usage:
    /// <code>
    /// await WriteErrorResponse(context, HttpStatusCode.Unauthorized, "missing");
    /// </code>
    ///
    /// Example response:
    /// <code>
    /// {
    ///   "status": "Failed",
    ///   "message": "API key is missing"
    /// }
    /// </code>
    /// </remarks>
    private async Task WriteErrorResponse(
        HttpContext context,
        HttpStatusCode statusCode,
        string messageKey
    )
    {
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsJsonAsync(
            new APIResponse()
                .ChangeStatus(language: _language, key: "api.status.failed")
                .ChangeMessage(language: _language, key: $"security.api_key.{messageKey}")
        );
    }

    /// <summary>
    /// Invokes the middleware to process the request.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <param name="next">The delegate representing the next middleware in the pipeline.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API key is missing, invalid, or not registered.</exception>
    /// <remarks>
    /// This method performs the following steps:
    /// 1. Checks for the presence of the API key in the request header.
    /// 2. Retrieves the list of registered API keys from the application configuration.
    /// 3. Validates the provided API key against the registered keys.
    /// 4. Calls the next middleware if the API key is valid, otherwise returns an error response.
    ///
    /// Example of a valid request flow:
    /// <code>
    /// GET /api/resource HTTP/1.1
    /// Host: example.com
    /// X-API-Key: valid-api-key
    ///
    /// // Middleware allows the request to proceed
    /// HTTP/1.1 200 OK
    /// ...
    /// </code>
    ///
    /// Example of an invalid API key:
    /// <code>
    /// GET /api/resource HTTP/1.1
    /// Host: example.com
    /// X-API-Key: invalid-api-key
    ///
    /// HTTP/1.1 401 Unauthorized
    /// Content-Type: application/json
    ///
    /// {
    ///   "status": "Failed",
    ///   "message": "Invalid API key"
    /// }
    /// </code>
    /// </remarks>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Skip authentication for static files
        if (
            context.Request.Path.StartsWithSegments("/themes")
            || context.Request.Path.StartsWithSegments("/images")
        )
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(API_KEY_HEADER, out var apiKeyHeader))
        {
            await WriteErrorResponse(
                context: context,
                statusCode: HttpStatusCode.Unauthorized,
                messageKey: "missing"
            );
            return;
        }

        string apiKey = apiKeyHeader.ToString();
        var registeredKeys =
            _appConfigs.Get<JArray>("Security.RegisteredApiKeyList")?.ToObject<string[]>() ?? [];

        if (registeredKeys.Length == 0)
        {
            await WriteErrorResponse(
                context: context,
                statusCode: HttpStatusCode.InternalServerError,
                messageKey: "not_registered"
            );
            return;
        }

        if (!registeredKeys.Contains(apiKey))
        {
            await WriteErrorResponse(
                context: context,
                statusCode: HttpStatusCode.Unauthorized,
                messageKey: "invalid"
            );
            return;
        }

        await next(context);
    }
}
